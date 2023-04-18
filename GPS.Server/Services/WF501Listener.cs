using GPS.Models;
using GPS.Proxy;
using GPS.Redis;
using GPS.Redis.Enums;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Server.Services
{
    public interface IWF501Listener
    {
        Task Receive(byte[] buffer, int length);
        public int AckWF501(byte[] data);
    }
    public class WF501Listener : IWF501Listener
    {
        private readonly ILogger<WF501Listener> _logger;
        private readonly IInventoryProxyAccessor _inventoryProxyAccessor;
      

        public WF501Listener(ILogger<WF501Listener> logger,
            IInventoryProxyAccessor inventoryProxyAccessor)
        {
            _logger = logger;
            _inventoryProxyAccessor = inventoryProxyAccessor;
        }

        public int AckWF501(byte[] data)
        {
            int serial = -1;
            #region WF501
            string protocols = Encoding.ASCII.GetString(data, 4, 2);
            if (protocols == "$$")
            {
                TZONE.WF501.Protocols.Generic generic = new TZONE.WF501.Protocols.Generic().Analysis(data);
                if (generic != null)
                {
                    serial = Convert.ToInt32(generic.Serial);
                }
            }
            else if (protocols == "$D")
            {
                TZONE.WF501.Protocols.CMD cmd = new TZONE.WF501.Protocols.CMD().Analysis(data);
                if (cmd != null)
                {
                    serial = Convert.ToInt32(cmd.Serial);
                }
            }
            #endregion
            return serial;
        }

        public async Task Receive(byte[] buffer, int length)
        {

            try
            {
                string protocols = Encoding.ASCII.GetString(buffer, 4, 2);

                if (protocols == "$$")
                {
                    TZONE.WF501.Protocols.Generic generic = new TZONE.WF501.Protocols.Generic().Analysis(buffer);
                    if (generic != null)
                    {
                        //_logger.LogWarning(JsonConvert.SerializeObject(generic));
                        var isSensorExists = await _inventoryProxyAccessor.IsSensorExists(generic.IMEI);
                        if (!isSensorExists)
                        {
                            _logger.LogWarning($"[WF501] Wifi sensor with serial {generic.IMEI} was not found");
                            return;
                        }
                        var inventorySensor = await _inventoryProxyAccessor.GetSensorBySerial(generic.IMEI);

                        if (inventorySensor == null)
                        {
                            _logger.LogWarning($"[WF501] No Invetory for sensor with serial {generic.IMEI}");
                            return;
                        }

                        var history = new InventoryHistoryView()
                        {
                            InventoryReferanceKey = inventorySensor.InventoryReferenceKey,
                            InventoryId = inventorySensor.InventoryId,
                            Alram = generic.Alram,
                            GatewayIMEI = generic.IMEI,
                            Serial = generic.IMEI,
                            GSMStatus = generic.WiFiStatus,
                            GpsDate = generic.RTC.AddMinutes(-30) < DateTime.Now ?  generic.RTC.AddHours(3) : DateTime.Now, //WF501 RTC
                            Humidity = (generic.Humidity * 100) > 120 ? 0 : GPS.Common.Converter.GetDecimal((generic.Humidity * 100).ToString()),
                            Temperature = GPS.Common.Converter.GetDecimal((generic.Temperature).ToString()),
                            IsLowVoltage = false
                        };
                        await _inventoryProxyAccessor.SaveWarehouseHistory(history);
                        //_logger.LogWarning(JsonConvert.SerializeObject(history));
                        //publish wasl
                        if (!string.IsNullOrWhiteSpace(history.InventoryReferanceKey))
                        {
                            await _inventoryProxyAccessor.HandelWaslPublish(history);
                        }
                    }
                    else
                    {
                        _logger.LogWarning("WF501 faild to analyze data");
                    }
                }
                else if (protocols == "$D")
                {
                    TZONE.WF501.Protocols.CMD cmd = new TZONE.WF501.Protocols.CMD().Analysis(buffer);
                    if (cmd != null)
                    {
                        _logger.LogInformation($"[WF501] sensor with serial {cmd.IMEI} protocol is cmd");
                    }
                }
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }

            await Task.FromResult(0);

        }

      

    }

}
