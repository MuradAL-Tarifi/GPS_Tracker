using AutoMapper;
using GPS.DataAccess.Repository.UnitOfWork;
using GPS.Domain.DTO;
using GPS.Domain.Models;
using GPS.Domain.Views;
using GPS.Integration.WaslModels;
using GPS.Integration.WaslServices.Inventories;
using GPS.Resources;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GPS.Services.Inventorys
{
    public class InventoryService : IInventoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<InventoryService> _logger;
        private readonly IStringLocalizer<SharedResources> _sharedLocalizer;
        private readonly IWaslInventoryService _waslInventoryService;

        public InventoryService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<InventoryService> logger,
            IStringLocalizer<SharedResources> sharedLocalizer,
            IWaslInventoryService waslInventoryService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _sharedLocalizer = sharedLocalizer;
            _waslInventoryService = waslInventoryService;
        }

        public async Task<ReturnResult<PagedResult<InventoryView>>> SearchAsync(long? FleetId = null, long? WarehouseId = null, int? waslLinkStatus = null, int? isActive = null, string SearchString = "", int PageNumber = 1, int pageSize = 100)
        {
            var result = new ReturnResult<PagedResult<InventoryView>>();
            try
            {
                var pagedResult = await _unitOfWork.InventoryRepository.SearchAsync(FleetId, WarehouseId, waslLinkStatus, isActive, SearchString, PageNumber, pageSize);

                var pagedListView = new PagedResult<InventoryView>
                {
                    TotalRecords = pagedResult.TotalRecords,
                    List = _mapper.Map<List<InventoryView>>(pagedResult.List)
                };

                result.Success(pagedListView);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }

        public async Task<ReturnResult<InventoryView>> FindByIdAsync(long? Id)
        {
            var result = new ReturnResult<InventoryView>();

            try
            {
                var inventory = await _unitOfWork.InventoryRepository.FindByIdAsync(Id);

                if (inventory == null || inventory.IsDeleted)
                {
                    result.NotFound(_sharedLocalizer["InventoryNotExists"]);
                    return result;
                }

                var sensorList = await _unitOfWork.InventorySensorRepository.GetBasicByInventoryId(Id.Value);
                var inventoryView = _mapper.Map<InventoryView>(inventory);
                inventoryView.InventorySensors = _mapper.Map<List<InventorySensorView>>(sensorList);

                result.Success(inventoryView);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }

        public async Task<ReturnResult<InventoryView>> SaveAsync(InventoryView model)
        {
            if (model.Id > 0)
            {
                return await UpdateAsync(model);
            }
            else
            {
                return await AddAsync(model);
            }
        }

        private async Task<ReturnResult<InventoryView>> AddAsync(InventoryView model)
        {
            var result = new ReturnResult<InventoryView>();
            try
            {
                var inventory = _mapper.Map<Inventory>(model);
                var inventorySensors = _mapper.Map<List<InventorySensor>>(model.InventorySensors);

                var entity = await _unitOfWork.InventoryRepository.AddAsync(inventory, inventorySensors);

                await _unitOfWork.EventLogRepository.LogEventAsync(Event.create, entity.Id, entity, entity.CreatedBy);

                result.Success(_mapper.Map<InventoryView>(inventory));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }

        private async Task<ReturnResult<InventoryView>> UpdateAsync(InventoryView model)
        {
            var result = new ReturnResult<InventoryView>();
            try
            {
                var entity = await _unitOfWork.InventoryRepository.UpdateAsync(model);
                await _unitOfWork.EventLogRepository.LogEventAsync(Event.update, entity.Id, entity, entity.UpdatedBy);
                result.Success(_mapper.Map<InventoryView>(entity));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }

        public async Task<ReturnResult<bool>> DeleteAsync(long Id, string UpdatedBy)
        {
            var result = new ReturnResult<bool>();
            try
            {
                var inventory = await _unitOfWork.InventoryRepository.DeleteAsync(Id, UpdatedBy);

                await _unitOfWork.EventLogRepository.LogEventAsync(Event.delete, inventory.Id, inventory, UpdatedBy);

                result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
                result.Data = false;
            }
            return result;
        }
        public async Task<ReturnResult<bool>> IsInventoryNumberExists(long fleetId, string inventoryNumber)
        {
            var result = new ReturnResult<bool>();

            try
            {
                var exists = await _unitOfWork.InventoryRepository.IsInventoryNumberExists(fleetId, inventoryNumber);
                result.Success(exists);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }

        public async Task<ReturnResult<bool>> LinkWithWasl(long id, string updatedBy)
        {
            var result = new ReturnResult<bool>();
            try
            {
                var inventory = await _unitOfWork.InventoryRepository.FindByIdAsync(id);

                if (inventory == null)
                {
                    result.NotFound(_sharedLocalizer["InventoryNotExists"]);
                    result.Data = false;
                    return result;
                }

                var fleetDetails = await _unitOfWork.FleetRepository.FindDetailsByFleetIdAsync(inventory.Warehouse.FleetId);

                // التحقق من توفر بيانات الارتباط
                List<string> missingData = new List<string>();

                //if (string.IsNullOrEmpty(warehouse.WaslActivityType))
                //{
                //    missingData.Add("نوع النشاط");
                //}

                if (string.IsNullOrEmpty(inventory.Name))
                {
                    missingData.Add("اسم المخزن");
                }

                if (string.IsNullOrEmpty(inventory.InventoryNumber))
                {
                    missingData.Add("رقم المخزن");
                }

                if (string.IsNullOrEmpty(inventory.SFDAStoringCategory))
                {
                    missingData.Add("نوع التخزين");
                }

                if (!inventory.Warehouse.IsLinkedWithWasl || string.IsNullOrEmpty(inventory.Warehouse.ReferenceKey))
                {
                    missingData.Add("المستودع غير مرتبط");
                }

                if (fleetDetails == null || !fleetDetails.IsLinkedWithWasl || string.IsNullOrEmpty(fleetDetails.ReferanceNumber))
                {
                    missingData.Add("الأسطول غير مرتبط");
                }

                if (missingData.Count > 0)
                {
                    result.BadRequest($"بيانات الربط غير كاملة. يرجى التحقق من [{string.Join(", ", missingData)}]");
                    result.Data = false;
                    return result;
                }

                var waslModel = new WaslInventoryModel()
                {
                    //Activity = inventory.WaslActivityType,
                    Name = inventory.Name,
                    InventoryNumber = inventory.InventoryNumber,
                    StoringCategory = inventory.SFDAStoringCategory
                };

                var registerResult = await _waslInventoryService.RegisterAsync(fleetDetails.ReferanceNumber, inventory.Warehouse.ReferenceKey, waslModel);
                await _unitOfWork.EventLogRepository.LogEventAsync(Event.registerWaslInventory, inventory.Id, registerResult, updatedBy);

                if (registerResult.IsSuccess || registerResult.Data?.IsDuplicate == true)
                {
                    inventory.IsLinkedWithWasl = true;
                    inventory.UpdatedBy = updatedBy;
                    inventory.UpdatedDate = DateTime.Now;
                    inventory.ReferenceKey = registerResult.Data.ReferenceKey;

                    await _unitOfWork.InventoryRepository.UpdateLinkedWithWaslInfoAsync(inventory);
                    await _unitOfWork.EventLogRepository.LogEventAsync(Event.update, inventory.Id, inventory, updatedBy);

                    result.Success(true);
                }
                else
                {
                    result.BadRequest(registerResult.ErrorList);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }

        public async Task<ReturnResult<bool>> UnlinkWithWasl(long id, string updatedBy)
        {
            var result = new ReturnResult<bool>();
            try
            {
                var inventory = await _unitOfWork.InventoryRepository.FindByIdAsync(id);

                if (inventory == null)
                {
                    result.NotFound(_sharedLocalizer["InventoryNotExists"]);
                    result.Data = false;
                    return result;
                }

                // التحقق من توفر بيانات الارتباط
                List<string> missingData = new List<string>();
                if (string.IsNullOrEmpty(inventory.ReferenceKey))
                {
                    missingData.Add("الرقم المرجعي للمخزن");
                }

                if (missingData.Count > 0)
                {
                    result.BadRequest($"بيانات فك الربط غير كاملة. يرجى التحقق من [{string.Join(", ", missingData)}]");
                    result.Data = false;
                    return result;
                }

                var registerResult = await _waslInventoryService.DeleteAsync(inventory.ReferenceKey);
                await _unitOfWork.EventLogRepository.LogEventAsync(Event.deleteWaslInventory, inventory.Id, registerResult, updatedBy);

                if (registerResult.IsSuccess)
                {
                    inventory.IsLinkedWithWasl = false;
                    inventory.UpdatedBy = updatedBy;
                    inventory.UpdatedDate = DateTime.Now;

                    await _unitOfWork.InventoryRepository.UpdateLinkedWithWaslInfoAsync(inventory);
                    await _unitOfWork.EventLogRepository.LogEventAsync(Event.update, inventory.Id, inventory, updatedBy);

                    result.Success(true);
                }
                else
                {
                    result.BadRequest(registerResult.ErrorList);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }

        public async Task<bool> IsInventorySensorExistsAsync(long sensorId)
        {
            try
            {
                var exists = await _unitOfWork.InventorySensorRepository.IsInventorySensorExistsAsync(sensorId);
                return exists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return false;
        }


    }
}
