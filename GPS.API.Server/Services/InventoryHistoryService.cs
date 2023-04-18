using AutoMapper;
using GPS.Models;
using GPS.Redis;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GPS.API.Server.Services
{
    public interface IInventoryHistoryService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="gatewayHistoryView"></param>
        /// <returns></returns>
        Task<ReturnResult<long>> SaveAsync(InventoryHistoryView gatewayHistoryView);
        Task<ReturnResult<InventorySensorView>> GetInventorySensor(string Serial);
        Task<ReturnResult<List<string>>> GetListSensorsSNByInventoryId(long inventoryId);
        Task<ReturnResult<string>> GetSensorSN(string Serial);
    }
    public class InventoryHistoryService : IInventoryHistoryService
    {
        private readonly ICacheService _cacheService;
        private readonly IMapper _mapper;
        private readonly IDapperRepository _dapperRepository;
        private readonly ILogger<InventoryHistoryService> _logger;
        public InventoryHistoryService(
            IMapper mapper,
            IDapperRepository dapperRepository,
            ICacheService cacheService,
            ILogger<InventoryHistoryService> logger)
        {
            _cacheService = cacheService;
            _mapper = mapper;
            _dapperRepository = dapperRepository;
            _logger = logger;
        }

        public async Task<ReturnResult<long>> SaveAsync(InventoryHistoryView inventoryHistoryView)
        {
            var result = new ReturnResult<long>();
            try
            {
                await _dapperRepository.InsertInventoryHistoryAsync(inventoryHistoryView);

                // check alert
                //await checkAlert(inventoryHistoryView);

                // Update Online History
                var onlineHistory = await _dapperRepository.GetInventoryHistoryBySensorSerialAsync(inventoryHistoryView.Serial);

                if (onlineHistory == null)
                {
                    onlineHistory = _mapper.Map<OnlineInventoryHistory>(inventoryHistoryView);
                    await _dapperRepository.InsertOnlineInventoryHistoryAsync(onlineHistory);
                }
                else
                {
                    if (onlineHistory.GpsDate > inventoryHistoryView.GpsDate)
                    {
                        result.Success(1);
                        return result;
                    }

                    onlineHistory = _mapper.Map<OnlineInventoryHistory>(inventoryHistoryView);
                    await _dapperRepository.UpdateOnlineInventoryHistoryAsync(onlineHistory);
                }

                result.Success(1);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                result.ServerError(ex.Message);
            }

            return result;
        }
        public async Task<ReturnResult<InventorySensorView>> GetInventorySensor(string Serial)
        {
            var result = new ReturnResult<InventorySensorView>();
            try
            {

                var sensor = await _dapperRepository.GetSensorBySerial(Serial);

                if (sensor != null)
                {
                    var inventorySensor = _mapper.Map<InventorySensorView>(sensor);

                    var inventory = await _dapperRepository.GetInventoryById(inventorySensor.InventoryId);
                    inventorySensor.InventoryReferenceKey = inventory?.ReferenceKey;

                    result.Success(inventorySensor);
                }
                else
                {
                    result.NotFound("not found");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                result.ServerError(ex.Message);
            }

            return result;
        }

        public async Task<ReturnResult<List<string>>> GetListSensorsSNByInventoryId(long inventoryId)
        {
            var result = new ReturnResult<List<string>>();
            try
            {
                var sensorsSN = await _dapperRepository.GetSensorsSerialByInventoryId(inventoryId);

                if (sensorsSN.Count > 0)
                {
                    result.Success(sensorsSN);
                }
                else
                {
                    result.NotFound("not found");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                result.ServerError(ex.Message);
            }

            return result;
        }

        public async Task<ReturnResult<string>> GetSensorSN(string Serial)
        {
            var result = new ReturnResult<string>();
            try
            {
                var sensorSN = await _dapperRepository.GetSensorSN(Serial);
                if (sensorSN != null)
                {
                    result.Success(sensorSN);
                }
                else
                {
                    result.NotFound("not found");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                result.ServerError(ex.Message);
            }

            return result;
        }
    }
}
