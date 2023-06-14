using AutoMapper;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using GPS.DataAccess.Repository.UnitOfWork;
using GPS.Domain.DTO;
using GPS.Domain.Models;
using GPS.Domain.Views;
using GPS.Resources;
using GPS.Services.Users;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Services.AlertBySensor
{
    public class AlertBySensorService : IAlertBySensorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<AlertBySensorService> _logger;
        private readonly IStringLocalizer<SharedResources> _sharedLocalizer;

        public AlertBySensorService(
             IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<AlertBySensorService> logger,
            IStringLocalizer<SharedResources> sharedLocalizer)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _sharedLocalizer = sharedLocalizer;
        }

        public async Task<ReturnResult<AlertSensorView>> FindbyId(long? id)
        {
            var result = new ReturnResult<AlertSensorView>();
            try
            {
                var Sensor = await _unitOfWork.AlertBySensorRepository.FindbyIdAsync(id);

                if (Sensor == null)
                {
                    result.NotFound(_sharedLocalizer["AlertTrakerNotExists"]);
                    return result;
                }
                result.Success(_mapper.Map<Domain.Models.AlertBySensor, AlertSensorView>(Sensor));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }
        public async Task<ReturnResult<bool>> SaveAsync(AlertSensorView model,string userId)
        {
            if (model.Id > 0)
            {
                return await UpdateAsync(model, userId);
            }
            else
            {
                return await AddAsync(model, userId);
            }
        }
        public async Task<ReturnResult<bool>> AddAsync(AlertSensorView alertSensorView, string userId)
        {
            var result = new ReturnResult<bool>();
            try
            {
                if (!await IsAlertBySensorExistsAsync(alertSensorView.Serial))
                {
                    alertSensorView.Serial = alertSensorView.Serial.ToLower();
                    var alertSensor = _mapper.Map<AlertSensorView, Domain.Models.AlertBySensor>(alertSensorView);
                    alertSensor.CreatedDate = DateTime.Now;
                    await _unitOfWork.AlertBySensorRepository.AddAsync(alertSensor);
                    await _unitOfWork.EventLogRepository.LogEventAsync(Event.create, alertSensor.Id, alertSensor, userId);
                    Smtpchecker smtpchecker=new Smtpchecker()
                    {
                        IsSendHumidity=false,
                        IsSendHumiditySecond=false,
                        IsSendTemperature=false,
                        IsSendTemperatureSecond = false,
                        UpdatedDateHumidity=DateTime.Now,
                        UpdatedDateTemperature=DateTime.Now,
                        Serial=alertSensor.Serial
                    };
                    await _unitOfWork.SmtpcheckerRepository.AddAsync(smtpchecker);
                    await _unitOfWork.EventLogRepository.LogEventAsync(Event.create, smtpchecker.Id, smtpchecker, userId);
                    result.Success(true);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
                result.Data = false;
            }
            return result;
        }

        public async Task<ReturnResult<bool>> UpdateAsync(AlertSensorView alertSensorView, string userId)
        {
            var result = new ReturnResult<bool>();
            try
            {
                if (!await IsAlertBySensorExistsAsync(alertSensorView.Serial))
                {
                    alertSensorView.Serial = alertSensorView.Serial.ToLower();
                    bool updatedSmptChecker = await _unitOfWork.SmtpcheckerRepository.UpdateAsync(alertSensorView);
                    bool updated = await _unitOfWork.AlertBySensorRepository.UpdateAsync(alertSensorView);

                    if (updated)
                    {
                        await _unitOfWork.EventLogRepository.LogEventAsync(Event.create, alertSensorView.Id, alertSensorView, userId);
                    }
                    result.Success(true);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
                result.Data = false;
            }
            return result;
        }
        public async Task<bool> IsAlertBySensorExistsAsync(string sensorSN)
        {
            try
            {
                var exists = await _unitOfWork.AlertBySensorRepository.IsAlertBySensorExistsAsync(sensorSN);
                return exists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return false;
        }
        public async Task<ReturnResult<PagedResult<AlertSensorView>>> SearchAsync(long? warehouseId, long? InventoryId, string Serial, string search, int pageNumber, int pageSize)
        {
            var result = new ReturnResult<PagedResult<AlertSensorView>>();
            try
            {
                var pagedResult = await _unitOfWork.AlertBySensorRepository.SearchAsync( warehouseId, InventoryId, Serial, search,pageNumber, pageSize);

                var pagedListView = new PagedResult<AlertSensorView>
                {
                    TotalRecords = pagedResult.TotalRecords,
                    List = _mapper.Map<List<AlertSensorView>>(pagedResult.List)
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

        public async Task<ReturnResult<bool>> Delete(long itemId, string userId)
        {
            var result = new ReturnResult<bool>();
            try
            {
                Domain.Models.Smtpchecker entitySmptChecker = await _unitOfWork.SmtpcheckerRepository.DeleteAsync(itemId);
                Domain.Models.AlertBySensor entity = await _unitOfWork.AlertBySensorRepository.DeleteAsync(itemId);

                if (entity == null)
                {
                    result.NotFound(_sharedLocalizer["AlertTrakerNotExists"]);
                    result.Data = false;
                    return result;
                }
                await _unitOfWork.EventLogRepository.LogEventAsync(Event.delete, entity.Id, entity, userId);
                await _unitOfWork.EventLogRepository.LogEventAsync(Event.delete, entitySmptChecker.Id, entitySmptChecker, userId);

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
    }
}
