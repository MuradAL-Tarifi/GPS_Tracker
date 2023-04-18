using GPS.Models;
using GPS.Proxy.Abstractions;
using GPS.Redis;
using GPS.Redis.Enums;
using Microsoft.Extensions.Logging;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Proxy
{
    public interface IInventoryProxyAccessor
    {
        Task<ApiResponse<string>> SaveWarehouseHistory(InventoryHistoryView gpsRawModel);
        Task<InventorySensorView> GetSensorBySerial(string serial);
        Task HandelWaslPublish(InventoryHistoryView historyView);
        Task<bool> IsSensorExists(string serial);
    }
    public class InventoryProxyAccessor : IInventoryProxyAccessor
    {
        private readonly IInventoryProxy _inventoryProxy;
        private readonly ICacheService _cacheManager;
        public InventoryProxyAccessor(IInventoryProxy inventoryProxy, ICacheService cacheManager)
        {
            _inventoryProxy = inventoryProxy;
            _cacheManager = cacheManager;
        }

        public async Task<ApiResponse<string>> SaveWarehouseHistory(InventoryHistoryView gpsRawModel)
        {
            return await _inventoryProxy.SaveWarehouseHistory(gpsRawModel);
        }

        public async Task<InventorySensorView> GetSensorBySerial(string serial)
        {
            var key = $"inventorySensor_{serial.ToLower()}";

            var sensorJson = await _cacheManager.GetCacheValueAsync(key);
            InventorySensorView inventorySensor = null;

            if (!string.IsNullOrWhiteSpace(sensorJson))
            {
                inventorySensor = System.Text.Json.JsonSerializer.Deserialize<InventorySensorView>(sensorJson);
            }

            if (inventorySensor != null)
                return inventorySensor;

            var result = await _inventoryProxy.GetSensorBySerial(serial);

            if (result.IsSuccessStatusCode && result.Content != null)
            {
                result.Content.Serial = result.Content.Serial.ToLower();
                sensorJson = System.Text.Json.JsonSerializer.Serialize(result.Content);
                await _cacheManager.SetCacheValueAsync(key, sensorJson, TimeSpan.FromMinutes(30), StackExchange.Redis.When.NotExists);
                return result.Content;
            }

            return null;
        }

        private async Task<List<string>> GetListSensorsSerialNumberByInventoryId(long inventoryId)
        {
            List<string> sensorsSN = new List<string>();
            var key =  $"inventoryId_{inventoryId}";
            var sensorsSNJson = await _cacheManager.GetCacheValueAsync(key);

            if (!string.IsNullOrWhiteSpace(sensorsSNJson))
            {
                sensorsSN = System.Text.Json.JsonSerializer.Deserialize<List<string>>(sensorsSNJson);
            }
            if (sensorsSN.Count > 0)
                return sensorsSN;

            var result = await _inventoryProxy.ListSensorsSerialNumberByInventoryId(inventoryId);

            if (result.IsSuccessStatusCode && result.Content != null)
            {
                sensorsSNJson = System.Text.Json.JsonSerializer.Serialize(result.Content);

                await _cacheManager.SetCacheValueAsync(key, sensorsSNJson, TimeSpan.FromMinutes(30), StackExchange.Redis.When.NotExists);
                return result.Content;
            }
            return sensorsSN;
        }

        public async Task HandelWaslPublish(InventoryHistoryView historyView)
        {
            // first cashe sensor data 
            var key = $"sensor{historyView.Serial}";
 
            await _cacheManager.SetCacheValueAsync(key, System.Text.Json.JsonSerializer.Serialize(historyView),
                TimeSpan.FromMinutes(30));


            var lsSensorsSN = await GetListSensorsSerialNumberByInventoryId(historyView.InventoryId);

            if (lsSensorsSN.Count > 0)
            {
                List<InventoryHistoryView> lsInventoryHistoryView = new List<InventoryHistoryView>();
                // loop through all sensors in cache
                foreach (var sensorSN in lsSensorsSN)
                {
                     key = $"sensor{sensorSN}";
                    var sensorHistory = await _cacheManager.GetCacheValueAsync(key);
                    if (!string.IsNullOrWhiteSpace(sensorHistory))
                    {
                        lsInventoryHistoryView.Add(System.Text.Json.JsonSerializer.Deserialize<InventoryHistoryView>(sensorHistory));
                    }
                }
                
                historyView.Temperature = lsInventoryHistoryView.Sum(x => (x.Temperature / lsInventoryHistoryView.Count));
                historyView.Humidity = lsInventoryHistoryView.Sum(x => (x.Humidity / lsInventoryHistoryView.Count));
            }
            await _cacheManager.Publish(PubSubChannel.WaslWarehouse, System.Text.Json.JsonSerializer.Serialize(historyView));

        }

        public async Task<bool> IsSensorExists(string serial)
        {
            var key = $"SensorSN_{serial.ToLower()}";

            var sensorSNJson = await _cacheManager.GetCacheValueAsync(key);

            if (!string.IsNullOrWhiteSpace(sensorSNJson))
            {
                return true;
            }
            var result = await _inventoryProxy.GetSensorSN(serial);

            if (result.IsSuccessStatusCode && result.Content != null)
            {
                sensorSNJson = System.Text.Json.JsonSerializer.Serialize(result.Content);
                await _cacheManager.SetCacheValueAsync(key, sensorSNJson.ToLower(), TimeSpan.FromHours(48), StackExchange.Redis.When.NotExists);
                return true;
            }
            return false;
        }
    }
}
