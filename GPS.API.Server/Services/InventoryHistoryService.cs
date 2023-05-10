using AutoMapper;
using GPS.Models;
using GPS.Redis;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
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
                await checkAlert(inventoryHistoryView);

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
        public async Task checkAlert(InventoryHistoryView inventoryHistoryView)
        {
            var sensor = await _dapperRepository.GetAlertBySensorBySerial(inventoryHistoryView.Serial);
            var sensorData = await _dapperRepository.GetSMTPCheckerBySerial(inventoryHistoryView.Serial);
            if (sensorData != null)
            {
                var alertTrakerDataLight = await _dapperRepository.GetAlertTrakerDataLightAsync(sensor.Serial, sensor.InventoryId, sensor.WarehouseId);
                // Temperature Check
                if (Convert.ToDouble(inventoryHistoryView.Temperature) <= sensor.MaxValueTemperature && Convert.ToDouble(inventoryHistoryView.Temperature) >= sensor.MinValueTemperature)
                {
                    Smtpchecker smtpchecker = new Smtpchecker
                    {
                        Serial = sensorData.Serial,
                        IsSendHumidity = sensorData.IsSendHumidity,
                        IsSendHumiditySecond = sensorData.IsSendHumiditySecond,
                        IsSendTemperature = false,
                        IsSendTemperatureSecond = false,
                        UpdatedDateHumidity = sensorData.UpdatedDateHumidity,
                        UpdatedDateTemperature = inventoryHistoryView.GpsDate,
                    };
                    await _dapperRepository.UpdateSMTPCheckerAsync(smtpchecker);

                }
                else if (sensorData.IsSendTemperature == false && sensorData.IsSendTemperatureSecond == false && sensorData.UpdatedDateTemperature.Value.AddMinutes(30) <= inventoryHistoryView.GpsDate)
                {
                    // OutSide Temperature
                    if (Convert.ToDouble(inventoryHistoryView.Temperature) > sensor.MaxValueTemperature || Convert.ToDouble(inventoryHistoryView.Temperature) < sensor.MinValueTemperature)
                    {
                        AlertTracker alertTracker = new AlertTracker()
                        {
                            UserName = sensor.UserName,
                            AlertDateTime = inventoryHistoryView.GpsDate,
                            AlertType = "Out Of Range!",
                            MonitoredUnit = alertTrakerDataLight.MonitoredUnit,
                            MessageForValue = "Temperature " + inventoryHistoryView.Temperature + " &#8451;",
                            Serial = inventoryHistoryView.Serial,
                            Zone = alertTrakerDataLight.Zone,
                            WarehouseName = alertTrakerDataLight.WarehouseName,
                            SendTo = sensor.ToEmails,
                            Interval = 60,
                            IsSend = true,
                            AlertId = Convert.ToInt32(inventoryHistoryView.Id)
                        };

                        var result = await _dapperRepository.InsertAlertTrackerAsync(alertTracker);
                        Smtpchecker smtpchecker = new Smtpchecker
                        {
                            Serial = sensorData.Serial,
                            IsSendHumidity = sensorData.IsSendHumidity,
                            IsSendHumiditySecond = sensorData.IsSendHumiditySecond,
                            IsSendTemperature = true,
                            IsSendTemperatureSecond = sensorData.IsSendTemperatureSecond,
                            UpdatedDateHumidity = sensorData.UpdatedDateHumidity,
                            UpdatedDateTemperature = inventoryHistoryView.GpsDate,
                        };
                        await _dapperRepository.UpdateSMTPCheckerAsync(smtpchecker);
                        var smtpsettings = await _dapperRepository.GetSmtpsettingsAsync();
                        var settingsCount = smtpsettings.Count();
                        var count = 0;

                        var alertBySensorrResult = await _dapperRepository.GetAlertBySensorBySerial(inventoryHistoryView.Serial);


                        var emails = alertBySensorrResult.ToEmails.Split(",");
                        if (emails != null)
                        {
                            bool results = false;
                            foreach (var email in emails)
                            {
                                var smtpsetting = smtpsettings[count];
                                results = false;
                                results = SendEmailAlarm(email, alertTracker, smtpsetting.UserName, smtpsetting.Password, smtpsetting.MailAddress);
                                if (results)
                                {
                                    await _dapperRepository.UpdateSmtpsettingAsync(smtpsetting.Id);
                                    count++;
                                    if (count >= settingsCount)
                                    {
                                        count = 0;
                                    }
                                }
                            }
                        }
                    }
                }
                else if (sensorData.IsSendTemperature == true && sensorData.IsSendTemperatureSecond == false && sensorData.UpdatedDateTemperature.Value.AddMinutes(30) <= inventoryHistoryView.GpsDate)
                {
                    // OutSide Temperature
                    if (Convert.ToDouble(inventoryHistoryView.Temperature) > sensor.MaxValueTemperature || Convert.ToDouble(inventoryHistoryView.Temperature) < sensor.MinValueTemperature)
                    {
                        AlertTracker alertTracker = new AlertTracker()
                        {
                            UserName = sensor.UserName,
                            AlertDateTime = inventoryHistoryView.GpsDate,
                            AlertType = "Out Of Range!",
                            MonitoredUnit = alertTrakerDataLight.MonitoredUnit,
                            MessageForValue = "Temperature " + inventoryHistoryView.Temperature + " &#8451;",
                            Serial = inventoryHistoryView.Serial,
                            Zone = alertTrakerDataLight.Zone,
                            WarehouseName = alertTrakerDataLight.WarehouseName,
                            SendTo = sensor.ToEmails,
                            Interval = 60,
                            IsSend = true,
                            AlertId = Convert.ToInt32(inventoryHistoryView.Id)
                        };
                        var result = await _dapperRepository.InsertAlertTrackerAsync(alertTracker);

                        Smtpchecker smtpchecker = new Smtpchecker
                        {
                            Serial = sensorData.Serial,
                            IsSendHumidity = sensorData.IsSendHumidity,
                            IsSendHumiditySecond = sensorData.IsSendHumiditySecond,
                            IsSendTemperature = sensorData.IsSendTemperature,
                            IsSendTemperatureSecond = true,
                            UpdatedDateHumidity = sensorData.UpdatedDateHumidity,
                            UpdatedDateTemperature = inventoryHistoryView.GpsDate,
                        };
                        await _dapperRepository.UpdateSMTPCheckerAsync(smtpchecker);

                        var smtpsettings = await _dapperRepository.GetSmtpsettingsAsync();
                        var settingsCount = smtpsettings.Count();
                        var count = 0;

                        var alertBySensorrResult = await _dapperRepository.GetAlertBySensorBySerial(inventoryHistoryView.Serial);


                        var emails = alertBySensorrResult.ToEmails.Split(",");
                        if (emails != null)
                        {
                            bool results = false;
                            foreach (var email in emails)
                            {
                                var smtpsetting = smtpsettings[count];
                                results = false;
                                results = SendEmailAlarm(email, alertTracker, smtpsetting.UserName, smtpsetting.Password, smtpsetting.MailAddress);
                                if (results)
                                {
                                    await _dapperRepository.UpdateSmtpsettingAsync(smtpsetting.Id);
                                    count++;
                                    if (count >= settingsCount)
                                    {
                                        count = 0;
                                    }
                                }
                            }
                        }
                    }
                }
                 sensor = await _dapperRepository.GetAlertBySensorBySerial(inventoryHistoryView.Serial);
                 sensorData = await _dapperRepository.GetSMTPCheckerBySerial(inventoryHistoryView.Serial);
                if (inventoryHistoryView.Humidity != -100000)
                {
                    if (Convert.ToDouble(inventoryHistoryView.Humidity) <= sensor.MaxValueHumidity && Convert.ToDouble(inventoryHistoryView.Humidity) >= sensor.MinValueHumidity)
                    {
                        Smtpchecker smtpchecker = new Smtpchecker
                        {
                            Serial = sensorData.Serial,
                            IsSendHumidity = false,
                            IsSendHumiditySecond = false,
                            IsSendTemperature = sensorData.IsSendTemperature,
                            IsSendTemperatureSecond = sensorData.IsSendTemperatureSecond,
                            UpdatedDateHumidity = inventoryHistoryView.GpsDate,
                            UpdatedDateTemperature = sensorData.UpdatedDateTemperature,
                        };
                        await _dapperRepository.UpdateSMTPCheckerAsync(smtpchecker);

                    }
                    else if (sensorData.IsSendHumidity == false && sensorData.IsSendHumiditySecond == false && sensorData.UpdatedDateHumidity.Value.AddMinutes(30) <= inventoryHistoryView.GpsDate)
                    {
                        //OutSide Humidity
                        if (Convert.ToDouble(inventoryHistoryView.Humidity) > sensor.MaxValueHumidity || Convert.ToDouble(inventoryHistoryView.Humidity) < sensor.MinValueHumidity)
                        {
                            AlertTracker alertTracker = new AlertTracker()
                            {
                                UserName = sensor.UserName,
                                AlertDateTime = inventoryHistoryView.GpsDate,
                                AlertType = "Out Of Range!",
                                MonitoredUnit = alertTrakerDataLight.MonitoredUnit,
                                MessageForValue = "Humidity " + inventoryHistoryView.Humidity,
                                Serial = inventoryHistoryView.Serial,
                                Zone = alertTrakerDataLight.Zone,
                                WarehouseName = alertTrakerDataLight.WarehouseName,
                                SendTo = sensor.ToEmails,
                                Interval = 60,
                                IsSend = true,
                                AlertId = Convert.ToInt32(inventoryHistoryView.Id)
                            };
                            var result = await _dapperRepository.InsertAlertTrackerAsync(alertTracker);
                            Smtpchecker smtpchecker = new Smtpchecker
                            {
                                Serial = sensorData.Serial,
                                IsSendHumidity = true,
                                IsSendHumiditySecond = sensorData.IsSendHumiditySecond,
                                IsSendTemperature = sensorData.IsSendTemperature,
                                IsSendTemperatureSecond = sensorData.IsSendTemperatureSecond,
                                UpdatedDateHumidity = inventoryHistoryView.GpsDate,
                                UpdatedDateTemperature = sensorData.UpdatedDateTemperature,
                            };
                            await _dapperRepository.UpdateSMTPCheckerAsync(smtpchecker);

                            var smtpsettings = await _dapperRepository.GetSmtpsettingsAsync();
                            var settingsCount = smtpsettings.Count();
                            var count = 0;

                            var alertBySensorrResult = await _dapperRepository.GetAlertBySensorBySerial(inventoryHistoryView.Serial);


                            var emails = alertBySensorrResult.ToEmails.Split(",");
                            if (emails != null)
                            {
                                bool results = false;
                                foreach (var email in emails)
                                {
                                    var smtpsetting = smtpsettings[count];
                                    results = false;
                                    results = SendEmailAlarm(email, alertTracker, smtpsetting.UserName, smtpsetting.Password, smtpsetting.MailAddress);
                                    if (results)
                                    {
                                        await _dapperRepository.UpdateSmtpsettingAsync(smtpsetting.Id);
                                        count++;
                                        if (count >= settingsCount)
                                        {
                                            count = 0;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (sensorData.IsSendHumidity == true && sensorData.IsSendHumiditySecond == false && sensorData.UpdatedDateHumidity.Value.AddMinutes(30) <= inventoryHistoryView.GpsDate)
                    {
                        //OutSide Humidity
                        if (Convert.ToDouble(inventoryHistoryView.Humidity) > sensor.MaxValueHumidity || Convert.ToDouble(inventoryHistoryView.Humidity) < sensor.MinValueHumidity)
                        {
                            AlertTracker alertTracker = new AlertTracker()
                            {
                                UserName = sensor.UserName,
                                AlertDateTime = inventoryHistoryView.GpsDate,
                                AlertType = "Out Of Range!",
                                MonitoredUnit = alertTrakerDataLight.MonitoredUnit,
                                MessageForValue = "Humidity " + inventoryHistoryView.Humidity,
                                Serial = inventoryHistoryView.Serial,
                                Zone = alertTrakerDataLight.Zone,
                                WarehouseName = alertTrakerDataLight.WarehouseName,
                                SendTo = sensor.ToEmails,
                                Interval = 60,
                                IsSend = true,
                                AlertId = Convert.ToInt32(inventoryHistoryView.Id)
                            };
                            var result = await _dapperRepository.InsertAlertTrackerAsync(alertTracker);

                            Smtpchecker smtpchecker = new Smtpchecker
                            {
                                Serial = sensorData.Serial,
                                IsSendHumidity = sensorData.IsSendHumidity,
                                IsSendHumiditySecond = true,
                                IsSendTemperature = sensorData.IsSendTemperature,
                                IsSendTemperatureSecond = sensorData.IsSendTemperatureSecond,
                                UpdatedDateHumidity = inventoryHistoryView.GpsDate,
                                UpdatedDateTemperature = sensorData.UpdatedDateTemperature,
                            };
                            await _dapperRepository.UpdateSMTPCheckerAsync(smtpchecker);

                            var smtpsettings = await _dapperRepository.GetSmtpsettingsAsync();
                            var settingsCount = smtpsettings.Count();
                            var count = 0;

                            var alertBySensorrResult = await _dapperRepository.GetAlertBySensorBySerial(inventoryHistoryView.Serial);


                            var emails = alertBySensorrResult.ToEmails.Split(",");
                            if (emails != null)
                            {
                                bool results = false;
                                foreach (var email in emails)
                                {
                                    var smtpsetting = smtpsettings[count];
                                    results = false;
                                    results = SendEmailAlarm(email, alertTracker, smtpsetting.UserName, smtpsetting.Password, smtpsetting.MailAddress);
                                    if (results)
                                    {
                                        await _dapperRepository.UpdateSmtpsettingAsync(smtpsetting.Id);
                                        count++;
                                        if (count >= settingsCount)
                                        {
                                            count = 0;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

            }


        }
        public bool SendEmailAlarm(string to, AlertTracker alertTracker, string UserName, string Password, string MailAddress)
        {
            using (SmtpClient smtpClient = new SmtpClient())
            {
                var basicCredential = new NetworkCredential(UserName, Password);
                using (MailMessage message = new MailMessage())
                {
                    MailAddress fromAddress = new MailAddress(MailAddress);

                    smtpClient.Host = "smtp.office365.com";
                    smtpClient.Port = 587;
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Credentials = basicCredential;
                    smtpClient.EnableSsl = true;

                    message.From = fromAddress;
                    message.Subject = "Quality Compliance";
                    message.IsBodyHtml = true;
                    message.Body = $@"    
<div style=""font-family: 'Times New Roman', Times, serif;font-size: 15px;"">
        <pre>Hello {alertTracker.UserName},<br>An alarm has been triggered on Accu Tracking<br>Date               {alertTracker.AlertDateTime.Value.ToString("dddd, MMMM, dd, yyyy hh:mm tt")}<br>Type               {alertTracker.AlertType}<br>Monitored unit     {alertTracker.MonitoredUnit}<br>Alarm measurement  {alertTracker.MessageForValue}<br>Recorder           {alertTracker.Serial}<br>Zone               {alertTracker.Zone}<br>Batches            {alertTracker.WarehouseName}<br><br>Regards,<br>Your alert system of Accu Tracking<br><br><small>This message has been generated automatically. Please do not reply</small></pre>
    </div>";
                    message.To.Add(to);

                    try
                    {
                        smtpClient.Send(message);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        return false;
                    }
                }
            }
        }

    }
}
