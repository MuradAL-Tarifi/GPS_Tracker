using AutoMapper;
using GPS.DataAccess.Context;
using GPS.DataAccess.Repository.UnitOfWork;
using GPS.Domain.DTO;
using GPS.Domain.Models;
using GPS.Domain.ViewModels;
using GPS.Domain.Views;
using GPS.Integration.WaslModels;
using GPS.Integration.WaslServices.Warehouse;
using GPS.Resources;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Services.WareHouses
{
    public class WarehouseService : IWarehouseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<WarehouseService> _logger;
        private readonly IStringLocalizer<SharedResources> _sharedLocalizer;
        private readonly IWaslWarehouseService _waslWarehouseService;

        public WarehouseService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<WarehouseService> logger,
            IStringLocalizer<SharedResources> sharedLocalizer,
            IWaslWarehouseService waslWarehouseService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _sharedLocalizer = sharedLocalizer;
            _waslWarehouseService = waslWarehouseService;
        }

        public async Task<ReturnResult<PagedResult<WarehouseView>>> SearchAsync(long? FleetId = null, int? waslLinkStatus = null, int? isActive = null, string SearchString = "", int PageNumber = 1, int pageSize = 100)
        {
            var result = new ReturnResult<PagedResult<WarehouseView>>();
            try
            {
                var pagedResult = await _unitOfWork.WarehouseRepository.SearchAsync(FleetId, waslLinkStatus, isActive, SearchString, PageNumber, pageSize);

                var pagedListView = new PagedResult<WarehouseView>
                {
                    TotalRecords = pagedResult.TotalRecords,
                    List = _mapper.Map<List<WarehouseView>>(pagedResult.List)
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

        public async Task<ReturnResult<WarehouseView>> FindByIdAsync(long? Id)
        {
            var result = new ReturnResult<WarehouseView>();

            try
            {
                var warehouse = await _unitOfWork.WarehouseRepository.FindByIdAsync(Id);

                if (warehouse == null || warehouse.IsDeleted)
                {
                    result.NotFound(_sharedLocalizer["WarehouseNotExists"]);
                    return result;
                }

                result.Success(_mapper.Map<WarehouseView>(warehouse));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }
        public async Task<ReturnResult<WarehouDetailsViewModel>> FindDetailedWarehouseByIdAsync(long WarehouseId)
        {
            var result = new ReturnResult<WarehouDetailsViewModel>();

            try
            {
                var warehouDetails = new WarehouDetailsViewModel();
                var warehouse = await _unitOfWork.WarehouseRepository.FindByIdAsync(WarehouseId);

                if (warehouse == null || warehouse.IsDeleted)
                {
                    result.NotFound(_sharedLocalizer["WarehouseNotExists"]);
                    return result;
                }
                
                warehouDetails.WarehouseView = _mapper.Map<WarehouseView>(warehouse);

                var inventories = await _unitOfWork.InventoryRepository.GetInventoriesByWarehouseId(WarehouseId);

                foreach(var inventory in inventories)
                {
                    InventorySensorsViewModel inventorySensorsViewModel = new InventorySensorsViewModel();
                    inventorySensorsViewModel.InventoryView = _mapper.Map<InventoryView>(inventory);
                    var inventorySensors = await _unitOfWork.InventorySensorRepository.GetBasicByInventoryId(inventory.Id);
                    if(inventorySensors.Count > 0)
                    {
                        inventorySensorsViewModel.LsSensor = _mapper.Map<List<SensorView>>(inventorySensors.Select(x => x.Sensor).ToList());
                    }
                    warehouDetails.LsInventorySensors.Add(inventorySensorsViewModel);
                }

                result.Success(_mapper.Map<WarehouDetailsViewModel>(warehouDetails));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }

        

        public async Task<ReturnResult<WarehouseView>> SaveAsync(WarehouseView warehouse)
        {
            if (warehouse.Id > 0)
            {
                return await UpdateAsync(warehouse);
            }
            else
            {
                return await AddAsync(warehouse);
            }
        }

        private async Task<ReturnResult<WarehouseView>> AddAsync(WarehouseView model)
        {
            var result = new ReturnResult<WarehouseView>();
            try
            {
                //model.RegisterTypeId = model.RegisterTypeId > 0 ? model.RegisterTypeId : null;
                var warehouse = _mapper.Map<Warehouse>(model);

                warehouse.CreatedDate = DateTime.Now;

                var entity = await _unitOfWork.WarehouseRepository.AddAsync(warehouse);

                await _unitOfWork.EventLogRepository.LogEventAsync(Event.create, entity.Id, entity, entity.CreatedBy);

                result.Success(_mapper.Map<WarehouseView>(warehouse));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }

        private async Task<ReturnResult<WarehouseView>> UpdateAsync(WarehouseView model)
        {
            var result = new ReturnResult<WarehouseView>();
            try
            {
                var entity = await _unitOfWork.WarehouseRepository.UpdateAsync(model);
                await _unitOfWork.EventLogRepository.LogEventAsync(Event.update, entity.Id, entity, entity.UpdatedBy);
                result.Success(_mapper.Map<WarehouseView>(entity));
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
                var entity = await _unitOfWork.WarehouseRepository.DeleteAsync(Id, UpdatedBy);
                await _unitOfWork.EventLogRepository.LogEventAsync(Event.delete, entity.Id, entity, UpdatedBy);

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
        public async Task<ReturnResult<List<WarehouseView>>> GetByUserIdAsync(string userId)
        {
            var result = new ReturnResult<List<WarehouseView>>();
            try
            {
                var groups = await _unitOfWork.WarehouseRepository.GetByUserIdAsync(userId);
                if (groups != null)
                {
                    result.Success(_mapper.Map<List<WarehouseView>>(groups));
                }
                else
                {
                    result.DefaultNotFound();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }

        public async Task<ReturnResult<List<WarehouseView>>> GetFleetLinkedWithWaslWarehousesAsync(long fleetId)
        {
            var result = new ReturnResult<List<WarehouseView>>();
            try
            {
                var entites = await _unitOfWork.WarehouseRepository.GetFleetLinkedWithWaslWarehousesAsync(fleetId);
                if (entites != null)
                {
                    var Warehouses = _mapper.Map<List<WarehouseView>>(entites);
                    foreach (var item in Warehouses)
                    {
                        var inventories = await _unitOfWork.InventoryRepository.GetWarehouseLinkedWithWaslInventoriesAsync(item.Id);
                        item.Inventories = _mapper.Map<List<InventoryView>>(inventories);
                    }
                    result.Success(Warehouses);
                }
                else
                {
                    result.DefaultNotFound();
                }
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
                var warehouse = await _unitOfWork.WarehouseRepository.FindByIdAsync(id);

                if (warehouse == null)
                {
                    result.NotFound(_sharedLocalizer["WarehouseNotExists"]);
                    result.Data = false;
                    return result;
                }

                var fleetDetails = await _unitOfWork.FleetRepository.FindDetailsByFleetIdAsync(warehouse.FleetId);

                // التحقق من توفر بيانات الارتباط
                List<string> missingData = new List<string>();

                //if (string.IsNullOrEmpty(warehouse.WaslActivityType))
                //{
                //    missingData.Add("نوع النشاط");
                //}

                if (string.IsNullOrEmpty(warehouse.Name))
                {
                    missingData.Add("اسم المستودع");
                }

                if (string.IsNullOrEmpty(warehouse.City))
                {
                    missingData.Add("المدينة");
                }

                if (string.IsNullOrEmpty(warehouse.Address))
                {
                    missingData.Add("العنوان");
                }

                if (!warehouse.Latitude.HasValue)
                {
                    missingData.Add("خط الطول");
                }

                if (!warehouse.Longitude.HasValue)
                {
                    missingData.Add("خط العرض");
                }

                if (string.IsNullOrEmpty(warehouse.LandCoordinates))
                {
                    missingData.Add("الإحداثيات");
                }

                if (string.IsNullOrEmpty(warehouse.LicenseNumber))
                {
                    missingData.Add("رقم الرخصة");
                }

                if (string.IsNullOrEmpty(warehouse.LicenseIssueDate))
                {
                    missingData.Add("تاريخ إصدار الرخصة");
                }

                if (string.IsNullOrEmpty(warehouse.LicenseExpiryDate))
                {
                    missingData.Add("تاريخ انتهاء الرخصة");
                }

                if (string.IsNullOrEmpty(warehouse.Phone))
                {
                    missingData.Add("رقم الهاتف");
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

                var companyId = fleetDetails.ReferanceNumber;

                var waslModel = new WaslWarehouseModel()
                {
                    //Activity = warehouse.WaslActivityType,
                    Name = warehouse.Name,
                    City = warehouse.City,
                    Address = warehouse.Address,
                    Latitude = warehouse.Latitude,
                    Longitude = warehouse.Longitude,
                    LandCoordinates = JsonConvert.DeserializeObject<List<LandCoordinates>>(warehouse.LandCoordinates),
                    LicenseNumber = warehouse.LicenseNumber,
                    LicenseIssueDate = warehouse.LicenseIssueDate,
                    LicenseExpiryDate = warehouse.LicenseExpiryDate,
                    Phone = warehouse.Phone,
                    ManagerMobile = warehouse.ManagerMobile,
                    Email = warehouse.Email,
                    LandAreaInSquareMeter = warehouse.LandAreaInSquareMeter > 0 ? warehouse.LandAreaInSquareMeter.ToString() : "",

                };

                var registerResult = await _waslWarehouseService.RegisterAsync(companyId, waslModel);
                await _unitOfWork.EventLogRepository.LogEventAsync(Event.registerWaslWarehouse, warehouse.Id, registerResult, updatedBy);

                if (registerResult.IsSuccess || registerResult.Data?.IsDuplicate == true)
                {
                    warehouse.IsLinkedWithWasl = true;
                    warehouse.UpdatedBy = updatedBy;
                    warehouse.UpdatedDate = DateTime.Now;
                    warehouse.ReferenceKey = registerResult.Data.ReferenceKey;

                    await _unitOfWork.WarehouseRepository.UpdateLinkedWithWaslInfoAsync(warehouse);
                    await _unitOfWork.EventLogRepository.LogEventAsync(Event.update, warehouse.Id, warehouse, updatedBy);

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
                var warehouse = await _unitOfWork.WarehouseRepository.FindByIdAsync(id);

                if (warehouse == null)
                {
                    result.NotFound(_sharedLocalizer["WarehouseNotExists"]);
                    result.Data = false;
                    return result;
                }

                var isAnyInventoryLinkedWithWasl = await _unitOfWork.InventoryRepository.IsAnyLinkedWithWaslAsync(id);
                if (isAnyInventoryLinkedWithWasl)
                {
                    result.BadRequest("لا يمكن فك الربط بسبب وجود مخازن تابعة للمستودع ومرتبطة في وصل");
                    result.Data = false;
                    return result;
                }

                // التحقق من توفر بيانات الارتباط
                List<string> missingData = new List<string>();
                if (string.IsNullOrEmpty(warehouse.ReferenceKey))
                {
                    missingData.Add("الرقم المرجعي للمستودع");
                }

                if (missingData.Count > 0)
                {
                    result.BadRequest($"بيانات فك الربط غير كاملة. يرجى التحقق من [{string.Join(", ", missingData)}]");
                    result.Data = false;
                    return result;
                }

                var registerResult = await _waslWarehouseService.DeleteAsync(warehouse.ReferenceKey);
                await _unitOfWork.EventLogRepository.LogEventAsync(Event.deleteWaslWarehouse, warehouse.Id, registerResult, updatedBy);

                if (registerResult.IsSuccess)
                {
                    warehouse.IsLinkedWithWasl = false;
                    warehouse.UpdatedBy = updatedBy;
                    warehouse.UpdatedDate = DateTime.Now;

                    await _unitOfWork.WarehouseRepository.UpdateLinkedWithWaslInfoAsync(warehouse);
                    await _unitOfWork.EventLogRepository.LogEventAsync(Event.update, warehouse.Id, warehouse, updatedBy);

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


    }
}
