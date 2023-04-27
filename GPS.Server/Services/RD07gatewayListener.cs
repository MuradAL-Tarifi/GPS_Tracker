using GPS.Models;
using GPS.Proxy;
using GPS.Redis;
using GPS.Redis.Enums;
using GPS.Server.Models.RD07DataParser;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;


namespace GPS.Server.Services
{
    public interface IRD07gatewayListener
    {
        Task Receive(byte[] buffer, int length);
        public int AckRD07_4G(byte[] data);
    }
    public class RD07gatewayListener : IRD07gatewayListener
    {
        private readonly ILogger<RD07gatewayListener> _logger;
        private readonly IInventoryProxyAccessor _inventoryProxyAccessor;
        private readonly ICacheService _cacheService;

        public RD07gatewayListener(ILogger<RD07gatewayListener> logger,
            IInventoryProxyAccessor inventoryProxyAccessor,
            ICacheService cacheService)
        {
            _logger = logger;
            _inventoryProxyAccessor = inventoryProxyAccessor;
            _cacheService = cacheService;
        }

        public async Task Receive(byte[] buffer, int length)
        {

            try
            {
                string protocols = Encoding.ASCII.GetString(buffer, 4, 2);

                if (protocols == "$$")
                { 
                    RD07Data _RD07Data = new RD07Data();

                    var generic = new TZONE.LoRa.Protocols.Generic().Analysis(buffer);
                    if (generic != null)
                    {
                        //log = "IMEI：" + generic.IMEI + " => Analysis Success";
                        _logger.LogDebug(generic.IMEI + "\n\r" + protocols);

                        List<TZONE.LoRa.Protocols.TAG> tagList = new TZONE.LoRa.Protocols.TAG().ConvertTag06(generic.TagList);

                        if (tagList != null && tagList.Count() > 0)
                        {
                            foreach (var tag in tagList)
                            {
                                _RD07Data.RD07TagList.Add(new RD07Tag()
                                {
                                    SN = long.Parse(tag.SN),
                                    IsLowVoltage = tag.IsLowVoltage,
                                    Humidity = (tag.Humidity * 100) > 120 ? 0: tag.Humidity,
                                    Temperature = tag.Temperature,
                                    RTC = Convert.ToDateTime(tag.RTC, new CultureInfo("en-US").DateTimeFormat)
                                });
                                //Console.WriteLine("tag: " + JsonSerializer.Serialize(tag));
                            }
                        }

                        var _imei = long.Parse(generic.IMEI);

                        _RD07Data.IMEI = long.Parse(generic.IMEI);
                        _RD07Data.GpsDate = generic.RTC; //generic.RTC.AddHours(3);
                        _RD07Data.Alram = generic.Alram;
                        _RD07Data.GSMStatus = generic.GSMStatus;
                        _RD07Data.GSMCSQ = generic.GSMCSQ;
                        _RD07Data.HardwareType = generic.HardwareType;

                        if (_RD07Data.GpsDate > DateTime.Now)
                            _RD07Data.GpsDate = DateTime.Now;




                        foreach (var tag in _RD07Data.RD07TagList)
                        {
                            var isSensorExists = await _inventoryProxyAccessor.IsSensorExists(tag.SN.ToString());
                            if (!isSensorExists)
                            {
                                _logger.LogWarning($"[RD07] 4G sensor with serial {tag.SN} was not found");
                                continue;
                            }

                            var inventorySensor = await _inventoryProxyAccessor.GetSensorBySerial(tag.SN.ToString());

                            if (inventorySensor == null)
                            {
                                _logger.LogWarning($"[RD07] No Invetory for sensor with serial {tag.SN}");
                                continue;
                            }

                            var history = new InventoryHistoryView()
                            {
                                InventoryReferanceKey = inventorySensor.InventoryReferenceKey,
                                InventoryId = inventorySensor.InventoryId,
                                Alram = _RD07Data.Alram,
                                GatewayIMEI = _RD07Data.IMEI.ToString(),
                                GSMStatus = _RD07Data.GSMStatus,
                                Serial = tag.SN.ToString(),
                                GpsDate = tag.RTC.AddMinutes(-30) < DateTime.Now ? tag.RTC.AddHours(3) : DateTime.Now,
                                Humidity = tag.Humidity > 0 ? (tag.Humidity * 100) : 0,
                                Temperature = tag.Temperature,
                                IsLowVoltage = tag.IsLowVoltage

                            };
                            //Console.WriteLine("history: " + JsonSerializer.Serialize(history));

                            await _inventoryProxyAccessor.SaveWarehouseHistory(history);

                            //publish wasl
                            if (!string.IsNullOrWhiteSpace(history.InventoryReferanceKey))
                            {
                                await _inventoryProxyAccessor.HandelWaslPublish(history);
                            }

                        }
                    }
                    else
                    {
                        _logger.LogWarning("RD07 faild to analyze data");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }

            await Task.FromResult(0);

        }

        public int AckRD07_4G(byte[] data)
        {
            int serial = -1;
            #region RD07 - 4G
            string protocols = Encoding.ASCII.GetString(data, 4, 2);
            if (protocols == "$$")
            {
                var generic = new TZONE.LoRa.Protocols.Generic().Analysis(data);
                if (generic != null)
                {
                    serial = Convert.ToInt32(generic.Serial);
                }
            }
            #endregion
            return serial;
        }



    }
}
