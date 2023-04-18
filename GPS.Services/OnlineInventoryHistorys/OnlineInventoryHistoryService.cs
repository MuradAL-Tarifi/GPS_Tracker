using GPS.Domain.DTO;
using GPS.Domain.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using GPS.Domain;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using GPS.Domain.ViewModels;
using AutoMapper;
using GPS.DataAccess.Repository.UnitOfWork;
using GPS.Domain.Models;

namespace GPS.Services.OnlineInventoryHistorys
{
    public class OnlineInventoryHistoryService : IOnlineInventoryHistoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<OnlineInventoryHistoryService> _logger;

        public OnlineInventoryHistoryService(
              IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<OnlineInventoryHistoryService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ReturnResult<List<OnlineInventoryHistoryView>>> SearchAsync(long fleetId, long? warehouseId = null, long? inventoryId = null)
        {
            var result = new ReturnResult<List<OnlineInventoryHistoryView>>();
            try
            {
                //var sensorList = await db.InventorySensor
                //    .Include(x => x.Inventory).ThenInclude(x => x.Warehouse).ThenInclude(x => x.Fleet)
                //    .Include(x => x.Inventory).ThenInclude(x => x.Gateway)
                //    .Where(x => !x.IsDeleted && x.Inventory.Warehouse.FleetId == fleetId &&
                //    (!warehouseId.HasValue || x.Inventory.WarehouseId == warehouseId) &&
                //    (!inventoryId.HasValue || x.InventoryId == inventoryId))
                //    .AsNoTracking()
                //    .ToListAsync();

                //sensorList.ForEach(x => x.Inventory.InventorySensors = new List<InventorySensor>());

                //var sensorListView = _mapper.Map<List<InventorySensorView>>(sensorList);

                //var onlineHistory = await db.OnlineInventoryHistory.Where(x => sensorList.Select(c => c.Serial).Contains(x.Serial)).ToListAsync();
                //var onlineHistoryView = _mapper.Map<List<OnlineInventoryHistoryView>>(onlineHistory);

                //onlineHistoryView.ForEach(x => x.InventorySensor = sensorListView.FirstOrDefault(s => s.Serial == x.Serial));


                (List<InventorySensor> sensorList, List<OnlineInventoryHistory> onlineHistory) = await _unitOfWork.OnlineInventoryHistoryRepository.SearchAsync(fleetId, warehouseId, inventoryId);

                var sensorListView = _mapper.Map<List<InventorySensorView>>(sensorList);
                var onlineHistoryView = _mapper.Map<List<OnlineInventoryHistoryView>>(onlineHistory);
                onlineHistoryView.ForEach(x => x.InventorySensor = sensorListView.FirstOrDefault(s => s.SensorView.Serial == x.Serial));
                result.Success(onlineHistoryView);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }

        public async Task<ReturnResult<List<WarehouseView>>> GetByUserIdAsync(string userId)
        {
            var result = new ReturnResult<List<WarehouseView>>();
            try
            {
                var userInventoriesAndWarehouses = await _unitOfWork.UserRepository.GetUserInventoriesAndWarehouesAsync(userId);
                var warehouseList = UserWarehouses(_mapper.Map<List<InventoryView>>(userInventoriesAndWarehouses));
               // var warehouseList = _mapper.Map<List<WarehouseView>>(warehouses);

                foreach (var warehouse in warehouseList)
                {
                    //var inventories = await _unitOfWork.InventoryRepository.GetByWarehouseIdAsync(warehouse.Id);
                    var inventories = userInventoriesAndWarehouses.Where(x => x.WarehouseId == warehouse.Id).ToList();
                    warehouse.Inventories = _mapper.Map<List<InventoryView>>(inventories);

                    foreach (var Inventory in warehouse.Inventories)
                    {
                        var inventorySensors = await _unitOfWork.InventorySensorRepository.GetBasicByInventoryId(Inventory.Id);
                        Inventory.InventorySensors = _mapper.Map<List<InventorySensorView>>(inventorySensors);

                        foreach (var item in Inventory.InventorySensors)
                        {
                            var onlineInventoryHistory = await _unitOfWork.OnlineInventoryHistoryRepository.GetBySensorSerialAsync(item.SensorView.Serial);
                            if(onlineInventoryHistory != null)
                            {
                                item.OnlineHistory = _mapper.Map<OnlineInventoryHistoryView>(onlineInventoryHistory);
                            }
                            else
                            {
                                item.OnlineHistory = new OnlineInventoryHistoryView
                                {
                                    GatewayIMEI = null,
                                    GpsDate = new DateTime(1999,1,1),
                                    Serial = item.SensorView.Serial,
                                    Temperature = 0,
                                    Humidity = 0
                                };
                            }
                            

                            //if (sensor.OnlineHistory != null)
                            //{
                            //    var sensorAlerts = await _unitOfWork.SensorAlertRepository.GetByInventorySensorIdAsync(sensor.Id);
                            //    if (sensorAlerts.Count > 0)
                            //    {
                            //        var temperatureAlerts = sensorAlerts.Where(x => x.SensorAlertTypeLookupId == 1).FirstOrDefault();
                            //        if (sensor.OnlineHistory.Temperature < temperatureAlerts.FromValue || sensor.OnlineHistory.Temperature > temperatureAlerts.ToValue)
                            //        {
                            //            sensor.OnlineHistory.TemperatureOutOfRange = true;
                            //        }
                            //    }
                            //}
                        }
                    }
                }

                //var sensorList = warehouseList.SelectMany(p => p.Inventories).SelectMany(p => p.InventorySensors).ToArray();
                //var viewSensorList = mapper.Map<List<InventorySensorView>>(sensorList);

                //foreach (var sensor in viewSensorList)
                //{
                //    var onlineInventoryHistory = await db.OnlineInventoryHistory.Where(x => x.Serial == sensor.Serial).FirstOrDefaultAsync();
                //    sensor.OnlineHistory = mapper.Map<OnlineInventoryHistoryView>(onlineInventoryHistory);
                //}

                result.Success(warehouseList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }


        /// <summary>
        /// الاستعلام عن تحديثات حساسات المستودع
        /// </summary>
        /// <param name="inventoryId"></param>
        /// <returns></returns>
        public async Task<ReturnResult<List<InventorySensorTemperatureModel>>> GetOnlineSensorTempertureAsync(long inventoryId)
        {
            var result = new ReturnResult<List<InventorySensorTemperatureModel>>();
            try
            {
                var inventorySensors = await _unitOfWork.InventorySensorRepository.GetByInventoryId(inventoryId);

                //var inventoryView = mapper.Map<InventoryView>(inventory);

                var dataList = new List<InventorySensorTemperatureModel>();

                foreach (var item in inventorySensors)
                {
                    var onlineInventoryHistory = await _unitOfWork.OnlineInventoryHistoryRepository.GetBySensorSerialAsync(item.Sensor.Serial);
                    if (onlineInventoryHistory != null)
                    {
                        dataList.Add(new InventorySensorTemperatureModel()
                        {
                            GatewayIMEI = onlineInventoryHistory.GatewayIMEI,
                            Serial = onlineInventoryHistory.Serial,
                            Temperature = onlineInventoryHistory.Temperature,
                            Humidity = onlineInventoryHistory.Humidity,
                            IsLowVoltage = onlineInventoryHistory.IsLowVoltage,
                            GpsDate = onlineInventoryHistory.GpsDate,
                            Alram = onlineInventoryHistory.Alram,
                            GSMStatus = onlineInventoryHistory.GSMStatus,
                            SensorName = item.Sensor.Name,
                            IsCalibrated = item.Sensor.CalibrationDate >= DateTime.Now ? true : false,
                            WarehouseId = item.Inventory.WarehouseId,
                            InventoryId = item.InventoryId,
                            InventoryName = item.Inventory.Name,
                            HasAnyRecords = true
                        }); ;
                    }
                    else
                    {
                        dataList.Add(new InventorySensorTemperatureModel()
                        {
                            GatewayIMEI = null,
                            Serial = item.Sensor.Serial,
                            Temperature = null,
                            Humidity = null,
                            IsLowVoltage = null,
                            GpsDate = new DateTime(1999,1,1),
                            Alram = string.Empty,
                            GSMStatus = string.Empty,
                            SensorName = item.Sensor.Name,
                            IsCalibrated = item.Sensor.CalibrationDate >= DateTime.Now ? true : false,
                            WarehouseId = item.Inventory.WarehouseId,
                            InventoryId = item.InventoryId,
                            InventoryName = item.Inventory.Name,
                            HasAnyRecords = false
                        });
                    }
                    //sensor.OnlineHistory = mapper.Map<OnlineInventoryHistoryView>(onlineInventoryHistory);
                }

                result.Success(dataList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }

        private List<WarehouseView> UserWarehouses(List<InventoryView> inventories)
        {
            List<WarehouseView> warehoueses = new List<WarehouseView>();
            foreach (var inventory in inventories)
            {
                if (!warehoueses.Any(x => x.Id == inventory.WarehouseId))
                {
                    warehoueses.Add(inventory.Warehouse);
                }
            }
            return warehoueses.ToList();
        }
        public async Task<ReturnResult<List<OnlineInventorySensors>>> GetOnlineInventoriesSensorData(List<long> inventoryIds)
        {
            var result = new ReturnResult<List<OnlineInventorySensors>>();
            var lsDataOnlineInventorySensors = new List<OnlineInventorySensors>();

            try
            {
                //var inventoryIds = invenotyIds.Split(",").Select(Int64.Parse).ToList();

                foreach (var inventoryId in inventoryIds)
                {
                    var onlineInventorySensors = new OnlineInventorySensors();
                    var inventorySensors = await _unitOfWork.InventorySensorRepository.GetByInventoryId(inventoryId);

                    //var inventoryView = mapper.Map<InventoryView>(inventory);

                    var dataList = new List<InventorySensorTemperatureModel>();

                    foreach (var item in inventorySensors)
                    {
                        var onlineInventoryHistory = await _unitOfWork.OnlineInventoryHistoryRepository.GetBySensorSerialAsync(item.Sensor.Serial);
                        if (onlineInventoryHistory != null)
                        {
                            dataList.Add(new InventorySensorTemperatureModel()
                            {
                                GatewayIMEI = onlineInventoryHistory.GatewayIMEI,
                                Serial = onlineInventoryHistory.Serial,
                                Temperature = onlineInventoryHistory.Temperature,
                                Humidity = onlineInventoryHistory.Humidity,
                                IsLowVoltage = onlineInventoryHistory.IsLowVoltage,
                                GpsDate = onlineInventoryHistory.GpsDate,
                                Alram = onlineInventoryHistory.Alram,
                                GSMStatus = onlineInventoryHistory.GSMStatus,
                                SensorName = item.Sensor.Name,
                                IsCalibrated = item.Sensor.CalibrationDate >= DateTime.Now ? true: false,
                                WarehouseId = item.Inventory.WarehouseId,
                                InventoryId = item.InventoryId,
                                InventoryName = item.Inventory.Name,
                                HasAnyRecords = true
                            });
                        }
                        else
                        {
                            dataList.Add(new InventorySensorTemperatureModel()
                            {
                                GatewayIMEI =null,
                                Serial = item.Sensor.Serial,
                                Temperature = null,
                                Humidity = null,
                                IsLowVoltage = null,
                                GpsDate = new DateTime(1999, 1, 1),
                                Alram = string.Empty,
                                GSMStatus = string.Empty,
                                SensorName = item.Sensor.Name,
                                IsCalibrated = item.Sensor.CalibrationDate >= DateTime.Now ? true : false,
                                WarehouseId = item.Inventory.WarehouseId,
                                InventoryId = item.InventoryId,
                                InventoryName = item.Inventory.Name,
                                HasAnyRecords = false
                            });
                        }
                    }
                    onlineInventorySensors.InventoryId = inventoryId;
                    onlineInventorySensors.LsInventorySensorTemperatureModel.AddRange(dataList);
                    lsDataOnlineInventorySensors.Add(onlineInventorySensors);
                }


                result.Success(lsDataOnlineInventorySensors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// for wasl update
        /// </summary>
        /// <returns></returns>
        //public async Task<ReturnResult<OnlineInventoryHistoryDto[]>> AllInvetoryHistoy()
        //{
        //    var result = new ReturnResult<OnlineInventoryHistoryDto[]>();
        //    try
        //    {
        //        //var fromDate = DateTime.Now.AddMinutes(-5);

        //        var historyList = await (from p in db.Inventory
        //                                 join s in db.InventorySensor on p.Id equals s.InventoryId
        //                                 join h in db.OnlineInventoryHistory
        //                                 on s.Id equals h.Id
        //                                 where p.IsActive && p.IsRegistered && !p.IsDeleted
        //                                 //&& h.GpsDate > fromDate
        //                                 select new OnlineInventoryHistoryDto()
        //                                 {
        //                                     InveroryId = p.Id,
        //                                     Name = p.Name,
        //                                     ReferenceKey = p.ReferenceKey,
        //                                     Serial = h.Serial,
        //                                     Temperature = h.Temperature,
        //                                     Humidity = h.Humidity
        //                                 }
        //                                 ).ToArrayAsync();



        //        result.Success(historyList);
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.LogError(ex, ex.Message, result);
        //        result.ServerError(ex.Message);
        //    }
        //    return result;
        //}

    }
}
