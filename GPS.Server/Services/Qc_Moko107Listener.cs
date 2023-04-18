using GPS.Models;
using GPS.Proxy;
using GPS.Redis;
using GPS.Server.Models.Qc_Moko107Parser;
using Microsoft.Extensions.Logging;
using MQTTnet.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Server.Services
{
    public interface IQc_Moko107Listener
    {
        Task MqttMsgPublishReceived(MqttApplicationMessageReceivedEventArgs e);
    }
   public class Qc_Moko107Listener: IQc_Moko107Listener
    {
        private readonly ILogger<Qc_Moko107Listener> _logger;
        private readonly IInventoryProxyAccessor _inventoryProxyAccessor;
        private readonly ICacheService _cacheService;

        public Qc_Moko107Listener(ILogger<Qc_Moko107Listener> logger,
            IInventoryProxyAccessor inventoryProxyAccessor,
            ICacheService cacheService)
        {
            _logger = logger;
            _inventoryProxyAccessor = inventoryProxyAccessor;
            _cacheService = cacheService;
        }
        public async Task MqttMsgPublishReceived(MqttApplicationMessageReceivedEventArgs e)
        {
            try
            {
                var jsonString = System.Text.Encoding.Default.GetString(e.ApplicationMessage.Payload);
                
                //return;
                
                var msg = JsonConvert.DeserializeObject<Moko107Data>(jsonString);
                 if (msg.sensors != null)
                {
                    if (msg.sensors.Count > 0)
                    {
                        foreach (var sensor in msg.sensors)
                        {
                            if (sensor.value != null)
                            {
                                var isSensorExists = await _inventoryProxyAccessor.IsSensorExists(sensor.value.mac);
                                if (!isSensorExists)
                                {
                                    _logger.LogWarning($"[Moko107] sensor with serial {sensor.value.mac} was not found");
                                    continue;
                                }
                                var inventorySensor = await _inventoryProxyAccessor.GetSensorBySerial(sensor.value.mac);

                                if (inventorySensor == null)
                                {
                                    _logger.LogWarning($"[Moko107] sensor [inventory] with serial {sensor.value.mac} was not found");
                                    continue;
                                }

                                var history = new InventoryHistoryView()
                                {
                                    InventoryReferanceKey = inventorySensor.InventoryReferenceKey,
                                    InventoryId = inventorySensor.InventoryId,
                                    Alram = null,
                                    GatewayIMEI = msg.device_info.mac,
                                    GSMStatus = null,
                                    Serial = sensor.value.mac,
                                    GpsDate = sensor.value.timestamp, //RD07Data.GpsDate, .AddHours(3)
                                    Humidity = decimal.Parse(sensor.value.humidity),
                                    Temperature = decimal.Parse(sensor.value.temperature),
                                    IsLowVoltage = false
                                };
                                Console.WriteLine($"[Moko107] Data Received : {DateTime.Now.ToString("yyyy/MM/dd  hh:mm tt", new CultureInfo("en-US").DateTimeFormat)}" + " ===> " + jsonString);
                                Console.WriteLine("history: " + JsonConvert.SerializeObject(history));

                                await _inventoryProxyAccessor.SaveWarehouseHistory(history);

                                //publish wasl
                                if (!string.IsNullOrWhiteSpace(history.InventoryReferanceKey))
                                {
                                    await _inventoryProxyAccessor.HandelWaslPublish(history);
                                }
                            }

                        }
                    }
                }
               

            }
            catch (Exception ex)
            {
                _logger.LogError(ex,ex.ToString());
            }
        }
    }
}
