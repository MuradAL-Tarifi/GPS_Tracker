using AutoMapper;
using GPS.DataAccess.Repository.UnitOfWork;
using GPS.Domain.DTO;
using GPS.Domain.Models;
using GPS.Domain.Views;
using GPS.Resources;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Services.CustomAlerts
{
   public class CustomAlertService : ICustomAlertService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CustomAlertService> _logger;
        private readonly IStringLocalizer<SharedResources> _sharedLocalizer;
        //private readonly IEmailHistoryService _emailHistoryService;
        //private readonly IEmailIntegration _emailIntegration;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly AppSettings _appSettings;

        public CustomAlertService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<CustomAlertService> logger,
            IStringLocalizer<SharedResources> sharedLocalizer,
            //IEmailHistoryService emailHistoryService,
            //IEmailIntegration emailIntegration,
            IHostingEnvironment hostingEnvironment,
            AppSettings appSettings
            )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _sharedLocalizer = sharedLocalizer;
            //_emailHistoryService = emailHistoryService;
            //_emailIntegration = emailIntegration;
            _hostingEnvironment = hostingEnvironment;
            _appSettings = appSettings;
        }

        public async Task<ReturnResult<PagedResult<CustomAlertView>>> SearchAsync(long FleetId, long? WarehouseId = null, long? InventoryId = null, int? IsActive = null, string SearchString = "", int PageNumber = 1, int PageSize = 100)
        {
            var result = new ReturnResult<PagedResult<CustomAlertView>>();
            var lsCustomAlert = new List<CustomAlertView>();
            var skip = (PageNumber - 1) * PageSize;
            try
            {
                var resultCustomAlerts = await _unitOfWork.CustomAlertRepository.SearchAsync(FleetId, WarehouseId, InventoryId, IsActive, SearchString);
                foreach (var customAlert in resultCustomAlerts)
                {
                    var inventories = await _unitOfWork.CustomAlertRepository.GetCustomAlertInventoriesAsync(customAlert.Id);
                    if (inventories.Count > 0)
                    {
                        var customAlertView = _mapper.Map<CustomAlertView>(customAlert);
                        customAlertView.Inventories = _mapper.Map<List<InventoryView>>(inventories);
                        lsCustomAlert.Add(customAlertView);
                    } 
                }

                var pagedResult = lsCustomAlert.OrderByDescending(x => x.CreatedDate).Skip(skip).Take(PageSize).ToList();
                var pagedResultView = new PagedResult<CustomAlertView>
                {
                    List = pagedResult,
                    TotalRecords = lsCustomAlert.Count
                };
                result.Success(pagedResultView);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }

            return result;
        }

        public async Task<ReturnResult<bool>> AddAsync(CustomAlertView CustomAlertView)
        {
            var result = new ReturnResult<bool>();
            try
            {
                int minInterval = _appSettings.CustomAlerts.MinIntervalMinutes;
                if (CustomAlertView.Interval < minInterval)
                {
                    result.BadRequest(string.Format(_sharedLocalizer["InvalidCustomAlertMinInterval"], minInterval));
                    return result;
                }
                List<long> inventoryIds = new List<long>();
                // 0 indecates all inventories 
                if (CustomAlertView.Inventories.Any(x => x.Id == 0))
                {
                    var inventories = await _unitOfWork.InventoryRepository.GetByWarehouseIdAsync(CustomAlertView.WarehouseId);
                    inventoryIds = inventories.Select(x => x.Id).Distinct().ToList();
                }
                else
                {
                    inventoryIds = CustomAlertView.Inventories.Select(x => x.Id).Distinct().ToList();
                }
                var newCustomAlert = _mapper.Map<CustomAlert>(CustomAlertView);
                var added = await _unitOfWork.CustomAlertRepository.AddAsync(newCustomAlert, inventoryIds.ToArray());
                await _unitOfWork.EventLogRepository.LogEventAsync(Event.create, newCustomAlert.Id, newCustomAlert, CustomAlertView.CreatedBy);
                result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }

            return result;
        }

        public async Task<ReturnResult<bool>> UpdateAsync(CustomAlertView CustomAlertView)
        {
            var result = new ReturnResult<bool>();
            try
            {
                int minInterval = _appSettings.CustomAlerts.MinIntervalMinutes;
                if (CustomAlertView.Interval < minInterval)
                {
                    result.BadRequest(string.Format(_sharedLocalizer["InvalidCustomAlertMinInterval"], minInterval));
                    return result;
                }
                List<long> inventoryIds = new List<long>();
                // 0 indecates all inventories 
                if (CustomAlertView.Inventories.Any(x => x.Id == 0))
                {
                    var inventories = await _unitOfWork.InventoryRepository.GetByWarehouseIdAsync(CustomAlertView.WarehouseId);
                    inventoryIds = inventories.Select(x => x.Id).Distinct().ToList();
                }
                else
                {
                    inventoryIds = CustomAlertView.Inventories.Select(x => x.Id).Distinct().ToList();
                }
                
                var updated = await _unitOfWork.CustomAlertRepository.UpdateAsync(_mapper.Map<CustomAlert>(CustomAlertView), inventoryIds.ToArray());
                if (updated != null)
                {
                    await _unitOfWork.EventLogRepository.LogEventAsync(Event.update, updated.Id, updated, CustomAlertView.UpdatedBy);
                }

                result.Success(true);
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
                var deleted = await _unitOfWork.CustomAlertRepository.DeleteAsync(Id, UpdatedBy);
                await _unitOfWork.EventLogRepository.LogEventAsync(Event.delete, Id, deleted, UpdatedBy);
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

        public async Task<ReturnResult<CustomAlertView>> CustomAlertByIdAsync(long Id)
        {
            var result = new ReturnResult<CustomAlertView>();
            try
            {
                var customAlertView = _mapper.Map<CustomAlertView>(await _unitOfWork.CustomAlertRepository.FindAsync(Id));
                customAlertView.Inventories = _mapper.Map<List<InventoryView>>(await _unitOfWork.CustomAlertRepository.GetCustomAlertInventoriesAsync(Id)) ;
                result.Success(customAlertView);
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
