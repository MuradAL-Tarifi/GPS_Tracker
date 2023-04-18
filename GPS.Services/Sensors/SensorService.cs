using AutoMapper;
using GPS.DataAccess.Repository.UnitOfWork;
using GPS.Domain.DTO;
using GPS.Domain.Models;
using GPS.Domain.Views;
using GPS.Resources;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Services.Sensors
{
    public class SensorService : ISensorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<SensorService> _logger;
        private readonly IStringLocalizer<SharedResources> _sharedLocalizer;

        public SensorService(IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<SensorService> logger,
            IStringLocalizer<SharedResources> sharedLocalizer)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _sharedLocalizer = sharedLocalizer;
        }

        public async Task<ReturnResult<PagedResult<SensorView>>> SearchAsync(long? fleetId, int? SensorStatus, int? BrandId = null, string SearchString = "", int PageNumber = 1, int pageSize = 100)
        {
            var result = new ReturnResult<PagedResult<SensorView>>();
            var lsSensorView = new List<SensorView>();
            var pagedResult = new PagedResult<Sensor>();
            try
            {
                if (fleetId.HasValue && fleetId > 0)
                {
                    var inventories = await _unitOfWork.InventoryRepository.GetInventoryByFleetIdAsync((long)fleetId);
                    var inventorySensor = await _unitOfWork.InventorySensorRepository.GetListInventorySensorByInventoryIdsAsync(inventories.Select(x => x.Id).ToList());
                    pagedResult = await _unitOfWork.SensorRepository
                        .SearchAsync(inventorySensor.Select(x => x.Sensor.Serial).ToList(), SensorStatus, BrandId, SearchString, PageNumber, pageSize);
                }
                else
                {
                    var sensorSNs = await _unitOfWork.SensorRepository.AllSensorsSerialNumber();
                    pagedResult = await _unitOfWork.SensorRepository
                        .SearchAsync(sensorSNs, SensorStatus, BrandId, SearchString, PageNumber, pageSize);
                }
                

                if (pagedResult == null)
                {
                    result.DefaultNotFound();
                    return result;
                }
                var lsInventorySensor = await _unitOfWork.InventorySensorRepository.GetListInventorySensor(pagedResult.List.Select(x => x.Id).ToList());
                foreach(var sensorView in _mapper.Map<List<SensorView>>(pagedResult.List))
                {
                    if (lsInventorySensor.Where(x => x.SensorId == sensorView.Id).Count() > 0)
                    {
                        sensorView.InventoryName = lsInventorySensor.Where(x => x.SensorId == sensorView.Id).FirstOrDefault().Inventory.Name;
                    }
                    lsSensorView.Add(sensorView);
                }

                var pagedListView = new PagedResult<SensorView>
                {
                    TotalRecords = pagedResult.TotalRecords,
                    List = lsSensorView
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

        public async Task<ReturnResult<SensorView>> FindbyIdAsync(long? Id)
        {
            var result = new ReturnResult<SensorView>();
            try
            {
                var Sensor = await _unitOfWork.SensorRepository.FindbyIdAsync(Id);

                if (Sensor == null || Sensor.IsDeleted)
                {
                    result.NotFound(_sharedLocalizer["SensorNotExists"]);
                    return result;
                }
                result.Success(_mapper.Map<Sensor, SensorView>(Sensor));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }

        public async Task<ReturnResult<bool>> SaveAsync(SensorView Sensor)
        {
            if (Sensor.Id > 0)
            {
                return await UpdateAsync(Sensor);
            }
            else
            {
                return await AddAsync(Sensor);
            }
        }

        public async Task<ReturnResult<bool>> SaveRangeAsync(List<SensorView> sensorViewList, string UserId, long? inventoryId)
        {
            return await AddRangeAsync(sensorViewList, UserId, inventoryId);
        }

        private async Task<ReturnResult<bool>> AddRangeAsync(List<SensorView> sensorViewList, string UserId, long? inventoryId)
        {
            var result = new ReturnResult<bool>();
            try
            {
                if (sensorViewList.Count > 0)
                {
                    var inventorySensors = new List<InventorySensor>();
                    List<Sensor> sensorList = new();

                    var brands = await _unitOfWork.BrandRepository.GetAllAsync();
                    foreach (SensorView sensorView in sensorViewList)
                    {
                        sensorView.Serial = sensorView.Serial.ToLower();
                        var Sensor = _mapper.Map<SensorView, Sensor>(sensorView);
                        Sensor.CreatedBy = UserId;
                        Sensor.CreatedDate = DateTime.Now;
                        if (!await IsSensorExistsAsync(sensorView.Serial) && brands.Any(x => x.Id == Sensor.BrandId))
                        {
                            sensorList.Add(Sensor);
                        }
                    }

                    if (inventoryId > 0)
                    {
                       foreach(var sensor in sensorList)
                        {
                           var addedSensor = await _unitOfWork.SensorRepository.AddAsync(sensor);
                                inventorySensors.Add(new InventorySensor()
                                {
                                    InventoryId = (long)inventoryId,
                                    SensorId = (long)addedSensor.Id,
                                    CreatedBy = UserId,
                                    CreatedDate = DateTime.Now,
                                }
                            );
                        }
                    }
                    else
                    {
                        await _unitOfWork.SensorRepository.AddRangeAsync(sensorList);
                    }
                    
                    if (inventorySensors.Count > 0)
                    {
                        await _unitOfWork.InventoryRepository.AddInventorySensorsAsync(inventorySensors);
                    }
                    await _unitOfWork.EventLogRepository.LogEventAsync(Event.create, sensorList.FirstOrDefault().Id, sensorList, sensorList.FirstOrDefault().CreatedBy);

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


        public async Task<ReturnResult<bool>> AddAsync(SensorView SensorView)
        {
            var result = new ReturnResult<bool>();
            try
            {
                if (!await IsSensorExistsAsync(SensorView.Serial))
                {
                    SensorView.Serial = SensorView.Serial.ToLower();
                    var Sensor = _mapper.Map<SensorView, Sensor>(SensorView);
                    Sensor.CreatedBy = SensorView.CreatedBy;
                    Sensor.CreatedDate = DateTime.Now;
                    await _unitOfWork.SensorRepository.AddAsync(Sensor);
                    await _unitOfWork.EventLogRepository.LogEventAsync(Event.create, Sensor.Id, Sensor, SensorView.CreatedBy);
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

        public async Task<ReturnResult<bool>> UpdateAsync(SensorView SensorView)
        {
            var result = new ReturnResult<bool>();
            try
            {
                if (!await IsUpdatedSensorExistsAsync(SensorView.Serial, SensorView.Id))
                {
                    SensorView.Serial = SensorView.Serial.ToLower();
                    bool updated = await _unitOfWork.SensorRepository.UpdateAsync(SensorView);
                    if (updated)
                    {
                        await _unitOfWork.EventLogRepository.LogEventAsync(Event.update, SensorView.Id, SensorView, SensorView.UpdatedBy);
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

        public async Task<ReturnResult<bool>> DeleteAsync(long Id, string UpdatedBy)
        {
            var result = new ReturnResult<bool>();
            try
            {
                var entity = await _unitOfWork.SensorRepository.DeleteAsync(Id, UpdatedBy);

                if (entity == null)
                {
                    result.NotFound(_sharedLocalizer["SensorNotExists"]);
                    result.Data = false;
                    return result;
                }

                await _unitOfWork.EventLogRepository.LogEventAsync(Event.delete, entity.Id, entity, UpdatedBy);


                var inventorySensor = _unitOfWork.InventorySensorRepository.DeleteSensorFromInventoryAsync(Id, UpdatedBy);
                await _unitOfWork.EventLogRepository.LogEventAsync(Event.delete, inventorySensor.Id, inventorySensor, UpdatedBy);

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

        public async Task<ReturnResult<int>> CountAsync(int? BrandId = null)
        {
            var result = new ReturnResult<int>();
            try
            {
                int Count = await _unitOfWork.SensorRepository.CountAsync(BrandId);
                result.Success(Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }

        public async Task<ReturnResult<List<SensorView>>> GetAllAsync(int? BrandId = null)
        {
            var result = new ReturnResult<List<SensorView>>();
            try
            {
                var Sensors = await _unitOfWork.SensorRepository.GetAllAsync(BrandId);
                result.Success(_mapper.Map<List<SensorView>>(Sensors));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }

        public async Task<bool> IsSensorExistsAsync(string SensorSN)
        {
            try
            {
                var exists = await _unitOfWork.SensorRepository.IsSensorExistsAsync(SensorSN);
                return exists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return false;
        }

        public async Task<bool> IsUpdatedSensorExistsAsync(string SensorSN, long sensorId)
        {
            try
            {
                var sensor = await _unitOfWork.SensorRepository.FindSensorBySerialNumberAsync(SensorSN);
                if (sensor == null)
                {
                    return false;
                }
                else
                {
                    var result = (sensor.Id == sensorId && sensor.Serial == SensorSN) ? false: true;
                    return result;
                }
                 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return false;
        }

    }
}
