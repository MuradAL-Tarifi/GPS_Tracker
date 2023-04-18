using AutoMapper;
using GPS.DataAccess.Repository.UnitOfWork;
using GPS.Domain.DTO;
using GPS.Domain.ViewModels;
using GPS.Domain.Views;
using GPS.Helper;
using GPS.Resources;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GPS.Services.Alerts
{
    public class AlertService : IAlertService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<AlertService> _logger;

        public AlertService(IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<AlertService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ReturnResult<PagedResult<AlertViewModel>>> GetPagedAlertsHistoryAsync(string userId, long? warehouseId, long? inventoryId, long? sensorId, long? alertType, long? alertId, string fromDate, string toDate, int pageNumber, int pageSize)
        {
            var result = new ReturnResult<PagedResult<AlertViewModel>>();
            try
            {
                DateTime? FromDate = null;
                DateTime? ToDate = null;
                if (!string.IsNullOrEmpty(fromDate))
                {
                    FromDate = GPSHelper.StringToDateTime(fromDate);
                }
                if (!string.IsNullOrEmpty(toDate))
                {
                    ToDate = GPSHelper.StringToDateTime(toDate);
                }
                var pagedAlerts = await _unitOfWork.AlertsRepository.SearchAsync(userId, warehouseId, inventoryId, sensorId, alertType, alertId, FromDate, ToDate, pageNumber, pageSize);
                var lsAlertViewModel = await BindToAlertViewModelAsync(_mapper.Map<List<AlertView>>(pagedAlerts.List));

                var pagedListView = new PagedResult<AlertViewModel>
                {
                    TotalRecords = pagedAlerts.TotalRecords,
                    List = _mapper.Map<List<AlertViewModel>>(lsAlertViewModel)
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

        public async Task<ReturnResult<List<AlertViewModel>>> GetTop100AlertsAsync(string userId)
        {
            var result = new ReturnResult<List<AlertViewModel>>();
            try
            {
                var lsAlertViewModel = new List<AlertViewModel>();
                var alerts = _mapper.Map<List<AlertView>>(await _unitOfWork.AlertsRepository.GetTop100AlertsAsync(userId));
                lsAlertViewModel = await BindToAlertViewModelAsync(alerts);
                if(lsAlertViewModel.Count > 0)
                {
                    lsAlertViewModel.First().NumberOfNewNotifications = await _unitOfWork.AlertsRepository.NumberOfUnViewedAlerts(userId);
                }
                result.Success(lsAlertViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }

        public async Task<ReturnResult<bool>> UpdateAlertsAsReadAsync(List<long> alertIds, string userId)
        {
            var result = new ReturnResult<bool>();
            try
            {
                result.Success(await _unitOfWork.AlertsRepository.UpdateAlertsAsReadAsync(alertIds, userId)); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }

        private async Task<List<AlertViewModel>> BindToAlertViewModelAsync(List<AlertView> alerts)
        {
            var lsAlertViewModel = new List<AlertViewModel>();

            var inventoryIds = alerts.Select(x => (long)x.InventoryId).Distinct().ToList();
            var warehouseIds = alerts.Select(x => (long)x.WarehouseId).Distinct().ToList();
            var sensorIds = alerts.Select(x => (long)x.SensorId).ToList();

            var inventories = await _unitOfWork.InventoryRepository.FindInventoriesAsync(inventoryIds);
            var warehouses = await _unitOfWork.WarehouseRepository.FindWarehousesAsync(warehouseIds);
            var sensors = await _unitOfWork.SensorRepository.FindSensorsAsync(sensorIds);
            foreach (var alert in alerts)
            {

                alert.AlertForValueEn = Regex.Replace(Regex.Replace(alert.AlertForValueEn, "[" + Regex.Escape(@"&;#") + "]", string.Empty), @"\b8451\b", string.Empty);
                alert.AlertForValueAr = Regex.Replace(Regex.Replace(alert.AlertForValueAr, "[" + Regex.Escape(@"&;#") + "]", string.Empty), @"\b8451\b", string.Empty);
                alert.AlertTextAr = Regex.Replace(Regex.Replace(alert.AlertTextAr, "[" + Regex.Escape(@"&;#") + "]", string.Empty), @"\b8451\b", string.Empty);
                alert.AlertTextEn = Regex.Replace(Regex.Replace(alert.AlertTextEn, "[" + Regex.Escape(@"&;#") + "]", string.Empty), @"\b8451\b", string.Empty);
                lsAlertViewModel.Add(new AlertViewModel
                {
                    Alert = _mapper.Map<AlertView>(alert),
                    Inventory = _mapper.Map<InventoryView>(inventories.FirstOrDefault(x => x.Id == alert.InventoryId)),
                    Warehouse = _mapper.Map<WarehouseView>(warehouses.FirstOrDefault(x => x.Id == alert.WarehouseId)),
                    Sensor = _mapper.Map<SensorView>(sensors.FirstOrDefault(x => x.Id == alert.SensorId))
                });
            }
            return lsAlertViewModel;
        }
    }
}
