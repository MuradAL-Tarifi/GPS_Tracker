using AutoMapper;
using GPS.API.Proxy;
using GPS.DataAccess.Repository.UnitOfWork;
using GPS.Domain.DTO;
using GPS.Domain.Models;
using GPS.Domain.ViewModels;
using GPS.Domain.Views;
using GPS.Helper;
using GPS.Integration.EmailIntegrations;
using GPS.Redis;
using GPS.Redis.Enums;
using GPS.Resources;
using GPS.Services.EmailHistorys;
using GPS.Services.Inventorys;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GPS.Services.Job
{
    public class JobService : IJobService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<JobService> _logger;
        private readonly IEmailIntegration _emailIntegration;
        private readonly IEmailHistoryService _emailHistoryService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IStringLocalizer<SharedResources> _sharedLocalizer;
        private readonly IInventoryHistoryReportService _inventoryHistoryReportService;
        private readonly IInventoryHistoryApiProxy _inventoryHistoryApiProxy;
        private readonly ICacheService _cacheService;

        public JobService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<JobService> logger,
            IEmailIntegration emailIntegration,
            IEmailHistoryService emailHistoryService,
            IHostingEnvironment hostingEnvironment,
            IStringLocalizer<SharedResources> sharedLocalizer,
            IInventoryHistoryReportService inventoryHistoryReportService,
            IInventoryHistoryApiProxy inventoryHistoryApiProxy,
            ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _emailIntegration = emailIntegration;
            _emailHistoryService = emailHistoryService;
            _hostingEnvironment = hostingEnvironment;
            _sharedLocalizer = sharedLocalizer;
            _inventoryHistoryReportService = inventoryHistoryReportService;
            _inventoryHistoryApiProxy = inventoryHistoryApiProxy;
            _cacheService = cacheService;
        }

        public async Task InventoryCustomAlertsWatcherAsync()
        {
           
            try
            {
                DateTime now = DateTime.Now;
                DateTime fromDateTime;
                DateTime toDateTime = DateTime.Now;

                var lastInventoryCustomAlertsWatcher = await _unitOfWork.InventoryCustomAlertsWatcherRepository.GetLastAsync();
                if (lastInventoryCustomAlertsWatcher != null)
                {
                    fromDateTime = lastInventoryCustomAlertsWatcher.ToDate;

                    if (DateTime.Now.Subtract(fromDateTime).TotalMinutes > 5)
                    {
                        bool conditionTriggered = false;
                        int toMinutes = 0;
                        if (now.Minute >= 5 && now.Minute < 10)
                        {
                            toMinutes = 5;
                            conditionTriggered = true;
                        }
                        if (now.Minute >= 10 && now.Minute < 15)
                        {
                            toMinutes = 10;
                            conditionTriggered = true;
                        }
                        if (now.Minute >= 15 && now.Minute < 20)
                        {
                            toMinutes = 15;
                            conditionTriggered = true;
                        }
                        if (now.Minute >= 20 && now.Minute < 25)
                        {
                            toMinutes = 20;
                            conditionTriggered = true;
                        }
                        if (now.Minute >= 25 && now.Minute < 30)
                        {
                            toMinutes = 25;
                            conditionTriggered = true;
                        }
                        if (now.Minute >= 30 && now.Minute < 35)
                        {
                            toMinutes = 30;
                            conditionTriggered = true;
                        }
                        else if (now.Minute >= 35 && now.Minute < 40)
                        {
                            toMinutes = 35;
                            conditionTriggered = true;
                        }
                        else if (now.Minute >= 40 && now.Minute < 45)
                        {
                            toMinutes = 40;
                            conditionTriggered = true;
                        }
                        else if (now.Minute >= 45 && now.Minute < 50)
                        {
                            toMinutes = 45;
                            conditionTriggered = true;
                        }
                        else if (now.Minute >= 50 && now.Minute < 55)
                        {
                            toMinutes = 50;
                            conditionTriggered = true;
                        }
                        else if (now.Minute >= 55)
                        {
                            toMinutes = 55;
                            conditionTriggered = true;
                        }
                        if (!conditionTriggered)
                        {
                            toMinutes = 0;
                        }

                        toDateTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, toMinutes, 0);
                    }
                    else
                    {
                        toDateTime = fromDateTime.AddMinutes(5);
                    }
                }
                else
                {
                    bool conditionTriggered = false;
                    int fromMinutes = 0;
                    if (now.Minute >= 5 && now.Minute < 10)
                    {
                        fromMinutes = 5;
                        conditionTriggered = true;
                    }
                    if (now.Minute >= 10 && now.Minute < 15)
                    {
                        fromMinutes = 10;
                        conditionTriggered = true;
                    }
                    if (now.Minute >= 15 && now.Minute < 20)
                    {
                        fromMinutes = 15;
                        conditionTriggered = true;
                    }
                    if (now.Minute >= 20 && now.Minute < 25)
                    {
                        fromMinutes = 20;
                        conditionTriggered = true;
                    }
                    if (now.Minute >= 25 && now.Minute < 30)
                    {
                        fromMinutes = 25;
                        conditionTriggered = true;
                    }
                    if (now.Minute >= 30 && now.Minute < 35)
                    {
                        fromMinutes = 30;
                        conditionTriggered = true;
                    }
                    else if (now.Minute >= 35 && now.Minute < 40)
                    {
                        fromMinutes = 35;
                        conditionTriggered = true;
                    }
                    else if (now.Minute >= 40 && now.Minute < 45)
                    {
                        fromMinutes = 40;
                        conditionTriggered = true;
                    }
                    else if (now.Minute >= 45 && now.Minute < 50)
                    {
                        fromMinutes = 45;
                        conditionTriggered = true;
                    }
                    else if (now.Minute >= 50 && now.Minute < 55)
                    {
                        fromMinutes = 50;
                        conditionTriggered = true;
                    }
                    if (now.Minute >= 55)
                    {
                        fromMinutes = 55;
                        conditionTriggered = true;
                    }
                    if (!conditionTriggered)
                    {
                        fromMinutes = 0;
                    }

                    fromDateTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, fromMinutes, 0).AddMinutes(-5);
                    toDateTime = fromDateTime.AddMinutes(5);
                }

                if ((lastInventoryCustomAlertsWatcher != null && fromDateTime == toDateTime) || toDateTime > DateTime.Now)
                {
                    GPSHelper.LogHistory("Inventory Custom Alerts Watcher job Ended Because No Schdualed Jobs At this Moment");
                    return;
                }
                await _cacheService.Publish(PubSubChannel.CustomAlert, System.Text.Json.JsonSerializer.Serialize(new CustomAlertWatcher { 
                    FromDate = fromDateTime,
                    ToDate = toDateTime
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }

            
        }

        public async Task BackgroundCustomAlertsJobAsync(CustomAlertWatcher customAlertWatcher)
        {
            try
            {
                GPSHelper.LogHistory("Inventory Custom Alerts Watcher job started");
                var fromDateString = customAlertWatcher.FromDate.ToString("yyyy/MM/dd hh:mm:ss tt", new CultureInfo("en-US").DateTimeFormat);
                var toDateString = customAlertWatcher.ToDate.ToString("yyyy/MM/dd hh:mm:ss tt", new CultureInfo("en-US").DateTimeFormat);
                var customAlerts = await _unitOfWork.CustomAlertRepository.GetAllActiveAsync();

                var customAlertInventories = await BindCustomAlertInventoriesAndSensors(customAlerts);

                foreach (var customAlertInventory in customAlertInventories)
                {
                    foreach (var inventoryView in customAlertInventory.LsInventoryView)
                    {
                        foreach (var inventorySensor in inventoryView.InventorySensors)
                        {
                            var historyResult = await InventorySensorHistory(inventoryView.Id, inventorySensor.SensorView.Serial, fromDateString, toDateString, true);
                            if (historyResult.IsSuccess)
                            {
                                foreach (var historyItem in historyResult.Data)
                                {
                                    await CheckForInventoryCustomAlerts(customAlertInventory.CustomAlert, historyItem, inventoryView, inventorySensor.SensorView);
                                }
                            }
                        }
                    }
                }

                await _unitOfWork.InventoryCustomAlertsWatcherRepository.AddAsync(customAlertWatcher.FromDate, customAlertWatcher.ToDate);
                GPSHelper.LogHistory("Inventory Custom Alerts Watcher job Ended");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        private async Task CheckForInventoryCustomAlerts(CustomAlert customAlert, InventoryHistoryView historyView, InventoryView inventoryView, SensorView sensorView)
        {
            string[] alertMessageAr = new string[2];
            string[] alertMessageEn = new string[2];
            string[] propertyAr = new string[2];
            string[] propertyEn = new string[2];

            var lastSensorAlertDate = await _unitOfWork.SensorAlertRepository.LastSensorAlertDateAsync(sensorView.Id);
            var lastAlertTotalMinutes = lastSensorAlertDate.HasValue ? historyView.GpsDate.Subtract(lastSensorAlertDate.Value).TotalMinutes : 0;

            if (!lastSensorAlertDate.HasValue || lastAlertTotalMinutes >= customAlert.Interval)
            {
                bool isAlert = false;
                switch (customAlert.AlertTypeLookupId)
                {
                    case (int)CustomAlertTypeEnum.HumidityOutOfRang:
                        if (historyView.Humidity.HasValue &&
                             (customAlert.MinValueHumidity.HasValue && customAlert.MinValueHumidity > (double?)historyView.Humidity) ||
                             (customAlert.MaxValueHumidity.HasValue && (double?)historyView.Humidity > customAlert.MaxValueHumidity))
                        {
                            isAlert = true;
                            propertyAr[0] = $"درجة الرطوبة {historyView.Humidity}";
                            propertyEn[0] = $"Humidity {historyView.Humidity}";
                            if (customAlert.MaxValueHumidity.HasValue && (double?)historyView.Humidity > customAlert.MaxValueHumidity)
                            {
                                alertMessageAr[0] = $"تم ارتفاع درجة الرطوبة فوق {customAlert.MaxValueHumidity}";
                                alertMessageEn[0] = $"Humidity is over {customAlert.MaxValueHumidity}";
                            }
                            else
                            {
                                alertMessageAr[0] = $"تم انخفاض درجة الرطوبة تحت {customAlert.MinValueHumidity}";
                                alertMessageEn[0] = $"Humidity is below {customAlert.MinValueHumidity}";
                            }
                        }
                        break;

                    case (int)CustomAlertTypeEnum.TemperatureOutOfRang:
                        if (historyView.Temperature.HasValue &&
                            (customAlert.MinValueTemperature.HasValue && customAlert.MinValueTemperature > (double?)historyView.Temperature) ||
                            (customAlert.MaxValueTemperature.HasValue && (double?)historyView.Temperature > customAlert.MaxValueTemperature))
                        {
                            isAlert = true;
                            propertyAr[0] = $"درجة الحرارة {historyView.Temperature} &#8451;";
                            propertyEn[0] = $"Temperature {historyView.Temperature} &#8451;";
                            if (customAlert.MaxValueTemperature.HasValue && (double?)historyView.Temperature > customAlert.MaxValueTemperature)
                            {
                                alertMessageAr[0] = $"تم ارتفاع درجة الحرارة فوق {customAlert.MaxValueTemperature} &#8451;";
                                alertMessageEn[0] = $"Temperature is over {customAlert.MaxValueTemperature} &#8451;";
                            }
                            else
                            {
                                alertMessageAr[0] = $"تم انخفاض  درجة الحرارة تحت {customAlert.MinValueTemperature} &#8451;";
                                alertMessageEn[0] = $"Temperature is below {customAlert.MinValueTemperature} &#8451;";
                            }
                        }
                        break;
                    case (int)CustomAlertTypeEnum.TemperatureAndHumidityOutOfRang:

                        if (historyView.Temperature.HasValue &&
                            (customAlert.MinValueTemperature.HasValue && customAlert.MinValueTemperature > (double?)historyView.Temperature) ||
                            (customAlert.MaxValueTemperature.HasValue && (double?)historyView.Temperature > customAlert.MaxValueTemperature))
                        {
                            isAlert = true;
                            propertyAr[0] = $"درجة الحرارة {historyView.Temperature} &#8451;";
                            propertyEn[0] = $"Temperature {historyView.Temperature} &#8451;";
                            if (customAlert.MaxValueTemperature.HasValue && (double?)historyView.Temperature > customAlert.MaxValueTemperature)
                            {
                                alertMessageAr[0] = $"تم ارتفاع درجة الحرارة فوق {customAlert.MaxValueTemperature} &#8451;";
                                alertMessageEn[0] = $"Temperature is over {customAlert.MaxValueTemperature} &#8451;";
                            }
                            else
                            {
                                alertMessageAr[0] = $"تم انخفاض  درجة الحرارة تحت {customAlert.MinValueTemperature} &#8451;";
                                alertMessageEn[0] = $"Temperature is below {customAlert.MinValueTemperature} &#8451;";
                            }
                        }
                        if (historyView.Humidity.HasValue &&
                            (customAlert.MinValueHumidity.HasValue && customAlert.MinValueHumidity > (double?)historyView.Humidity) ||
                            (customAlert.MaxValueHumidity.HasValue && (double?)historyView.Humidity > customAlert.MaxValueHumidity))
                        {
                            isAlert = true;
                            propertyAr[1] = $"درجة الرطوبة {historyView.Humidity}";
                            propertyEn[1] = $"Humidity {historyView.Humidity}";
                            if (customAlert.MaxValueHumidity.HasValue && (double?)historyView.Humidity > customAlert.MaxValueHumidity)
                            {
                                alertMessageAr[1] = $"تم ارتفاع درجة الرطوبة فوق {customAlert.MaxValueHumidity}";
                                alertMessageEn[1] = $"Humidity is over {customAlert.MaxValueHumidity}";
                            }
                            else
                            {
                                alertMessageAr[1] = $"تم انخفاض درجة الرطوبة تحت {customAlert.MinValueHumidity}";
                                alertMessageEn[1] = $"Humidity is below {customAlert.MinValueHumidity}";
                            }
                        }
                        break;
                }
                if (isAlert)
                {
                    // update last alert date
                    await _unitOfWork.SensorAlertRepository.UpdateLastAlertDateBySensorIdAsync(customAlert.Id, sensorView.Id, historyView.GpsDate);

                    var addedAlert = new Alert()
                    {
                        AlertTypeLookupId = customAlert.AlertTypeLookupId,
                        FleetId = inventoryView.Warehouse.FleetId,
                        InventoryId = inventoryView.Id,
                        WarehouseId = inventoryView.WarehouseId,
                        SensorId = sensorView.Id,
                        CustomAlertId = customAlert.Id,
                        AlertDateTime = historyView.GpsDate,
                        Temperature = historyView.Temperature,
                        Humidity = historyView.Humidity,
                        CreatedDate = DateTime.Now
                    };

                    var htmlBody = new StringBuilder();
                    if (customAlert.AlertTypeLookupId == (int)CustomAlertTypeEnum.TemperatureAndHumidityOutOfRang)
                    {
                        StringBuilder alertTextAr = new StringBuilder();
                        StringBuilder alertTextEn = new StringBuilder();
                        StringBuilder propertyTextAr = new StringBuilder();
                        StringBuilder propertyTextEn = new StringBuilder();
                        if (!string.IsNullOrEmpty(alertMessageAr[0]) &&
                            !string.IsNullOrEmpty(alertMessageEn[0]) &&
                            !string.IsNullOrEmpty(propertyAr[0]) &&
                            !string.IsNullOrEmpty(propertyEn[0])
                            )
                        {
                            alertTextAr.Append(alertMessageAr[0]);
                            alertTextEn.Append(alertMessageEn[0]);
                            propertyTextAr.Append(propertyAr[0]);
                            propertyTextEn.Append(propertyEn[0]);
                        }
                        if (!string.IsNullOrEmpty(alertMessageAr[1]) &&
                            !string.IsNullOrEmpty(alertMessageEn[1]) &&
                            !string.IsNullOrEmpty(propertyAr[1]) &&
                            !string.IsNullOrEmpty(propertyEn[1]))
                        {
                            alertTextEn.Append("," + alertMessageAr[1]);
                            alertTextEn.Append("," + alertMessageEn[1]);
                            propertyTextAr.Append("," + propertyAr[1]);
                            propertyTextEn.Append("," + propertyEn[1]);
                        }
                        addedAlert.AlertTextAr = alertTextAr.ToString();
                        addedAlert.AlertTextEn = alertTextEn.ToString();
                        addedAlert.AlertForValueAr = propertyTextAr.ToString();
                        addedAlert.AlertForValueEn = propertyTextEn.ToString();

                        htmlBody.Append($@"
                               <tr>
                                 <td style ='width:280px'><h3>{alertMessageEn[1]}</h3></td>
                                 <td style = 'width:60px'></td>
                                 <td style = 'width:280px' dir='rtl'><h3>{ alertMessageAr[1]}</h3></td>
                               </tr>
                               <tr>
                                 <td><h3>{propertyEn[1]}</h3></td>
                                 <td></td>
                                 <td dir='rtl'><h3>{propertyAr[1]}</h3></td>
                               </tr>");
                    }
                    else
                    {
                        addedAlert.AlertTextAr = alertMessageAr[0];
                        addedAlert.AlertTextEn = alertMessageEn[0];
                        addedAlert.AlertForValueAr = propertyAr[0];
                        addedAlert.AlertForValueEn = propertyEn[0];
                        htmlBody.Append(string.Empty);
                    }
                    var systemSetting = await _unitOfWork.SystemSettingRepository.LoadSystemSettingAsync();
                    // send alert
                    await _unitOfWork.AlertsRepository.AddAsync(addedAlert);
                    string title = $"{systemSetting.CompanyName} | {customAlert.AlertTypeLookup.NameEn} Alert | {inventoryView.Warehouse.Name} - {inventoryView.Name}";

                    var templatePath = _hostingEnvironment.ContentRootPath + @"\assets\emailTemplates\CustomAlert.xml";
                    string body = System.IO.File.ReadAllText(templatePath);

                    body = body.Replace("{WarehouseInfo}", inventoryView.Warehouse.Name);
                    body = body.Replace("{InventoryInfo}", inventoryView.Name);
                    body = body.Replace("{Date}", historyView.GpsDate.ToString("dd/MM/yyyy hh:mm:ss tt"));
                    body = body.Replace("{AlertMessageAr}", alertMessageAr[0]);
                    body = body.Replace("{AlertMessageEn}", alertMessageEn[0]);
                    body = body.Replace("{PropertyAr}", propertyAr[0]);
                    body = body.Replace("{PropertyEn}", propertyEn[0]);
                    body = body.Replace("{AppendHTML}", htmlBody.ToString());

                    List<string> emailIds = new List<string>();
                    if (!string.IsNullOrEmpty(customAlert.UserIds))
                    {
                        var users = await _unitOfWork.UserRepository.GetUsersByIdsAsync(customAlert.UserIds.Split(",").ToList());
                        foreach (var user in users)
                        {
                            if (!string.IsNullOrEmpty(user.Email))
                            {
                                emailIds.Add(user.Email);
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(customAlert.ToEmails))
                    {
                        emailIds = emailIds.Count > 0 ? emailIds : customAlert.ToEmails.Split(",").ToList();
                    }

                    var isSent = await _emailIntegration.SendEmailAsync(title, body, emailIds.ToArray(), systemSetting);

                    // add email history
                    EmailHistoryView emailHistoryView = new EmailHistoryView()
                    {
                        AlertId = addedAlert.Id,
                        Title = title,
                        Body = body,
                        ToEmails = string.Join(",", emailIds),
                        IsSent = isSent,
                        CreatedDate = DateTime.Now,
                    };

                    if (isSent)
                    {
                        emailHistoryView.SentDate = DateTime.Now;
                    }
                    await _emailHistoryService.AddAsync(emailHistoryView);
                }
            }

        }

        private async Task<List<CustomAlertViewModel>> BindCustomAlertInventoriesAndSensors(List<CustomAlert> lsCustomAlerts)
        {
            List<CustomAlertViewModel> lsCustomAlert = new List<CustomAlertViewModel>();
            foreach(var customAlert in lsCustomAlerts)
            {
                var inventories = _mapper.Map<List<InventoryView>>
                    (await _unitOfWork.CustomAlertRepository.GetCustomAlertInventoriesAsync(customAlert.Id));
                foreach(var inventory in inventories)
                {
                   var inventorySensors = await _unitOfWork.InventorySensorRepository.GetByInventoryId(inventory.Id);
                    inventory.InventorySensors.AddRange(_mapper.Map<List<InventorySensorView>>(inventorySensors));
                }
                lsCustomAlert.Add(new CustomAlertViewModel()
                {
                    LsInventoryView = inventories,
                    CustomAlert = customAlert
                });
            }
            return lsCustomAlert;
        }

        private async Task<ReturnResult<List<InventoryHistoryView>>> InventorySensorHistory(long InventoryId,string SensorSN, string fromDate, string toDate,
           bool reversed = false)
        {
            var result = new ReturnResult<List<InventoryHistoryView>>();
            try
            {
                var historyResult = await _inventoryHistoryApiProxy.SensorHistory(InventoryId, SensorSN, fromDate, toDate);

                if (historyResult.IsSuccess)
                {
                    if (reversed)
                    {
                        historyResult.Data.Reverse();
                    }

                    result.Success(_mapper.Map<List<InventoryHistoryView>>(historyResult.Data));
                }
                else
                {
                    result.DefaultNotFound();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }

        #region Scheduled Reports Watcher
        public async Task ScheduledReportsWatcherAsync()
        {
            GPSHelper.LogHistory("Process Scheduled Reports Job Started....");

            var result = new ReturnResult<int>();
            try
            {

                var currentDate = DateTime.Now;
                var currentHour = currentDate.ToString("HH");
                var currentDayOfWeek = (int)currentDate.DayOfWeek;
                var currentDayOfMonth = currentDate.Day;

                var reportSchedules = await _unitOfWork.ReportScheduleRepository.GetScheduledReportsAsync();


                foreach (var item in reportSchedules)
                {

                    var isDue = true;
                    ReportScheduleHistory reportScheduleHistory;

                    if (item.Daily == true)
                    {
                     
                        reportScheduleHistory = await _unitOfWork.ReportScheduleRepository.GetReportScheduleHistoryByIdAsync(item.Id, ScheduleTypeEnum.Daily);

                        if (reportScheduleHistory != null)
                        {
                            isDue = reportScheduleHistory.DueDateTime.AddDays(item.DailyRepeat.Value).Date <= currentDate.Date;
                        }
                        if (isDue)
                        {
                            //  Daily report 
                            await GenerateAndSendReport(item, ScheduleTypeEnum.Daily);
                        }
                    }

                    if (item.Weekly == true)
                    {
                        isDue = true;

                        reportScheduleHistory = await _unitOfWork.ReportScheduleRepository.GetReportScheduleHistoryByIdAsync(item.Id, ScheduleTypeEnum.Weekly);

                        if (reportScheduleHistory != null)
                        {
                            isDue = reportScheduleHistory.DueDateTime.AddDays(item.WeeklyRepeat.Value * 7).Date <= currentDate.Date;
                        }
                        if (isDue)
                        {
                            //  Weekly report 
                            await GenerateAndSendReport(item, ScheduleTypeEnum.Weekly);
                        }
                    }

                    if (item.Monthly == true)
                    {
                        isDue = true;

                        reportScheduleHistory = await _unitOfWork.ReportScheduleRepository.GetReportScheduleHistoryByIdAsync(item.Id, ScheduleTypeEnum.Monthly);

                        if (reportScheduleHistory != null)
                        {
                            isDue = reportScheduleHistory.DueDateTime.AddMonths(item.MonthlyRepeat.Value).Date <= currentDate.Date;
                        }
                        if (isDue)
                        {
                            //  Monthly report 
                            await GenerateAndSendReport(item, ScheduleTypeEnum.Monthly);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }

            GPSHelper.LogHistory("Process Scheduled Reports Job Ended");
        }
    

        private async Task GenerateAndSendReport(ReportSchedule item, ScheduleTypeEnum scheduleType)
        {

            SetCulture(item.IsEnglish);

            var currentDate = DateTime.Now;

            var hour = currentDate.ToString("HH");

            DateTime fromDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, Convert.ToInt32(hour), 0, 0);
            DateTime toDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, Convert.ToInt32(hour), 0, 0);

            // add history
            var reportScheduleHistory = new ReportScheduleHistory()
            {
                ReportScheduleId = item.Id,
                ScheduleTypeId = (int)scheduleType,
                DueDateTime = toDate
            };

            await _unitOfWork.ReportScheduleRepository.AddReportScheduleHistoryAsync(reportScheduleHistory);

            string scheduleTypeName = "";
            string reportTypeName = "";
            string scheduleDesciption = "";

            var scheduleTypeLookup = await _unitOfWork.ReportScheduleRepository.GetScheduleTypeLookupByIdAsync((int)scheduleType);

            scheduleTypeName = item.IsEnglish ? scheduleTypeLookup.NameEn : scheduleTypeLookup.Name;
            reportTypeName = item.IsEnglish ? item.ReportTypeLookup.NameEn : item.ReportTypeLookup.Name;

            // report filter
            switch (scheduleType)
            {
                case ScheduleTypeEnum.Daily:

                    fromDate = fromDate.AddDays(-item.DailyRepeat.Value);


                    if (item.IsEnglish)
                    {
                        scheduleDesciption = $@"<span>
                                Every <span class='green text-bold'>{item.DailyRepeat}</span> Day - At <span class='green text-bold '>{toDate.ToString("hh:mm:ss tt")}</span>
                            </span>";
                    }
                    else
                    {
                        scheduleDesciption = $@"<span>
                                كل <span class='green text-bold'>{item.DailyRepeat}</span> يوم - الساعة <span class='green text-bold '>{toDate.ToString("hh:mm:ss tt")}</span>
                            </span>";
                    }
                    break;
                case ScheduleTypeEnum.Weekly:
                    fromDate = fromDate.AddDays(-(item.WeeklyRepeat.Value * 7));
                    if (item.IsEnglish)
                    {
                        scheduleDesciption = $@"<span>
                                Every <span class='green text-bold'>{item.WeeklyRepeat}</span> Week -  <span class='green text-bold'>{item.DaysOfWeekLookup.NameEn}</span> - At <span class='green text-bold '>{toDate.ToString("hh:mm:ss tt")}</span>
                            </span>";
                    }
                    else
                    {
                        scheduleDesciption = $@"<span>
                                كل <span class='green text-bold'>{item.WeeklyRepeat}</span> اسبوع -  <span class='green text-bold'>{item.DaysOfWeekLookup.Name}</span> - الساعة <span class='green text-bold '>{toDate.ToString("hh:mm:ss tt")}</span>
                            </span>";
                    }
                    break;
                case ScheduleTypeEnum.Monthly:
                    fromDate = fromDate.AddMonths(-item.MonthlyRepeat.Value);
                    if (item.IsEnglish)
                    {
                        scheduleDesciption = $@"<span>
                                Every <span class='green text-bold'>{item.MonthlyRepeat}</span> Month - Day <span class='green text-bold'>{item.DayOfMonthId}</span> - At <span class='green text-bold '>{toDate.ToString("hh:mm:ss tt")}</span>
                            </span>";
                    }
                    else
                    {
                        scheduleDesciption = $@"<span>
                                كل <span class='green text-bold'>{item.MonthlyRepeat}</span> شهر - يوم <span class='green text-bold'>{item.DayOfMonthId}</span> - الساعة <span class='green text-bold '>{toDate.ToString("hh:mm:ss tt")}</span>
                            </span>";
                    }
                    break;
            }

            string reportFileName = "";
       
            Fleet fleet = null;
            InventorySensor inventorySensor = null;
            Inventory inventory = null;


            fleet = await _unitOfWork.FleetRepository.FindByIdAsync(item.FleetId);

            if (item.ReportTypeLookupId == (int)ReportTypeEnum.InventorySensorsHistoryReport ||
                item.ReportTypeLookupId == (int)ReportTypeEnum.InventoryHistoryReport)
            {

                if (!String.IsNullOrEmpty(item.SensorSerial))
                {
                    inventorySensor = await _unitOfWork.InventorySensorRepository.GetInventorySensor(item.InventoryId.Value, item.SensorSerial);
                    reportFileName = $"{inventorySensor.Sensor.Serial}-{scheduleTypeName}-{reportTypeName}";
                }
                else
                {
                    inventory = await _unitOfWork.InventoryRepository.FindByIdAsync(item.InventoryId);
                    reportFileName = $"{inventory.Name}-{scheduleTypeName}-{reportTypeName}";
                }
            }
            else if (item.ReportTypeLookupId == (int)ReportTypeEnum.AlertsReport)
            {
                reportFileName = $"{scheduleTypeName}-{reportTypeName}";
            }
          

            string fromDateString = fromDate.ToString("yyyy/MM/dd hh:mm:ss tt", new CultureInfo("en-US").DateTimeFormat);
            string toDateString = toDate.ToString("yyyy/MM/dd hh:mm:ss tt", new CultureInfo("en-US").DateTimeFormat);

            // generate report
            var pdfReportResult = new ReturnResult<byte[]>();
            var excelReportResult = new ReturnResult<byte[]>();
            var emailAttachments = new List<EmailAttachmentModel>();


            switch (item.ReportTypeLookupId)
            {
          
                case (int)ReportTypeEnum.InventoryHistoryReport:
                    if (item.PDF == true)
                    {
                        pdfReportResult = await _inventoryHistoryReportService.InventoryHistoryReportPDF(item.InventoryId.Value,null, fromDateString, toDateString, item.GroupUpdatesByType, item.GroupUpdatesValue, item.IsEnglish);
                        if (pdfReportResult.IsSuccess && pdfReportResult.Data != null)
                        {
                            emailAttachments.Add(new EmailAttachmentModel()
                            {
                                Name = $"{reportFileName}.pdf",
                                Content = pdfReportResult.Data
                            });
                        }
                    }
                    if (item.Excel == true)
                    {
                        excelReportResult = await _inventoryHistoryReportService.InventoryHistoryReportExcel(item.InventoryId.Value, fromDateString, toDateString, item.GroupUpdatesByType, item.GroupUpdatesValue, item.IsEnglish);
                        if (excelReportResult.IsSuccess && excelReportResult.Data != null)
                        {
                            emailAttachments.Add(new EmailAttachmentModel()
                            {
                                Name = $"{reportFileName}.xlsx",
                                Content = excelReportResult.Data
                            });
                        }
                    }
                    break;
                
            }

            if (pdfReportResult.IsSuccess || excelReportResult.IsSuccess)
            {
                if (item.ReportTypeLookupId == (int)ReportTypeEnum.InventorySensorsHistoryReport ||
                    item.ReportTypeLookupId == (int)ReportTypeEnum.InventoryHistoryReport)
                {
                    //send report
                    //TaskFactory _alertTask = new TaskFactory(TaskCreationOptions.None, TaskContinuationOptions.None);
                    //var t0 = _alertTask.StartNew(async () =>
                    //{
                    await SendInventorySensorReportEmail(inventory, inventorySensor, item, emailAttachments, reportScheduleHistory.Id,
                           scheduleTypeName, reportTypeName, fromDateString, toDateString, scheduleDesciption, item.ReportTypeLookupId);
                    //});
                }
               
            }
        }
        #endregion
        private async Task SendInventorySensorReportEmail(Inventory inventory, InventorySensor inventorySensor, ReportSchedule item, List<EmailAttachmentModel> emailAttachments, long reportScheduleHistoryId,
           string scheduleTypeName, string reportTypeName, string fromDateString, string toDateString, string scheduleDesciption, int reportTypeEnum)
        {
            try
            {
                string fleetName = "";
                string warehouseName = "";
                string inventoryName = "";
                string sensorInfo = "";
                string title = "";
                string templatePath = "";

                var systemSetting = await _unitOfWork.SystemSettingRepository.LoadSystemSettingAsync();
                if (reportTypeEnum == (int)ReportTypeEnum.InventoryHistoryReport)
                {
                    title = $"{systemSetting.CompanyName} | {reportTypeName} | {scheduleTypeName} | {inventoryName}";
                    templatePath = _hostingEnvironment.ContentRootPath + @"\assets\emailTemplates\InventoryReportSchedule.xml";

                    fleetName = item.IsEnglish ? inventory.Warehouse.Fleet.NameEn : inventory.Warehouse.Fleet.Name;
                    warehouseName = inventory.Warehouse.Name;
                    inventoryName = inventory.Name;
                }
                else
                {
                    title = $"{systemSetting.CompanyName} | {reportTypeName} | {scheduleTypeName} | {sensorInfo}";
                    sensorInfo = $@"{inventorySensor.Sensor.Name} / {inventorySensor.Sensor.Serial}";
                    templatePath = _hostingEnvironment.ContentRootPath + @"\assets\emailTemplates\InventorySensorReportSchedule.xml";

                    fleetName = item.IsEnglish ? inventorySensor.Inventory.Warehouse.Fleet.NameEn : inventorySensor.Inventory.Warehouse.Fleet.Name;
                    warehouseName = inventorySensor.Inventory.Warehouse.Name;
                    inventoryName = inventorySensor.Inventory.Name;
                }
                var fleet = inventorySensor.Inventory.Warehouse.Fleet;
                //var base64FleetLogo = string.Empty;
                var imgExtention = string.Empty;
                if (fleet.LogoPhotoByte != null)
                {
                    if (fleet.LogoPhotoByte.Length > 0)
                    {
                        //base64FleetLogo = Convert.ToBase64String(fleet.LogoPhotoByte);
                        imgExtention = fleet.LogoPhotoExtention;
                        SaveImage(_hostingEnvironment.ContentRootPath + @"\assets\imgs", fleet.LogoPhotoByte, fleet.Id + fleet.LogoPhotoExtention);
                    }
                }

                if (string.IsNullOrEmpty(imgExtention))
                {
                    if (systemSetting.LogoPhotoByte != null)
                    {
                        //base64FleetLogo = Convert.ToBase64String(fleet.LogoPhotoByte);
                        imgExtention = ".png";
                        SaveImage(_hostingEnvironment.ContentRootPath + @"\assets\imgs", systemSetting.LogoPhotoByte, fleet.Id + imgExtention);
                    }
                }

                var logoPath = _hostingEnvironment.ContentRootPath + @"\assets\imgs\logo.png";

                //var src = !string.IsNullOrEmpty(base64FleetLogo) ? $"data:image/{fleet.LogoPhotoExtention}; base64,{base64FleetLogo}" : logoPath;
                var src = !string.IsNullOrEmpty(imgExtention) ? _hostingEnvironment.ContentRootPath + @"\assets\imgs\" + fleet.Id + imgExtention : logoPath;

                string body = System.IO.File.ReadAllText(templatePath);
                body = body.Replace("{ReportTitle}", item.Name);
                body = body.Replace("{ScheduleType}", scheduleTypeName);
                body = body.Replace("{Date}", toDateString);
                body = body.Replace("{ScheduleDesciption}", scheduleDesciption);
                body = body.Replace("{ReportName}", reportTypeName);
                body = body.Replace("{FleetLabel}", _sharedLocalizer["Fleet"]);
                body = body.Replace("{WarehouseLabel}", _sharedLocalizer["Warehouse"]);
                body = body.Replace("{InventoryLabel}", _sharedLocalizer["Inventory"]);
                body = body.Replace("{SensorLabel}", _sharedLocalizer["Sensor"]);
                body = body.Replace("{DateLabel}", _sharedLocalizer["Date"]);
                body = body.Replace("{Fleet}", fleetName);
                body = body.Replace("{Warehouse}", warehouseName);
                body = body.Replace("{Inventory}", inventoryName);
                body = body.Replace("{SensorInfo}", sensorInfo);
                body = body.Replace("{Dates}", $"{fromDateString} -> {toDateString}");
                body = body.Replace("{Src}", $"{src}");

 

                var isSent = await _emailIntegration.SendEmailWithAttachmentsAsync(title, body, item.Emails.Split(","),systemSetting, emailAttachments);

                // add email history
                EmailHistoryView emailHistoryView = new EmailHistoryView()
                {
                    Title = title,
                    Body = body,
                    ToEmails = item.Emails,
                    IsSent = isSent,
                    CreatedDate = DateTime.Now
                };

                if (isSent)
                {
                    emailHistoryView.SentDate = DateTime.Now;
                }
                await _emailHistoryService.AddAsync(emailHistoryView);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, null);
            }
        }

        private void SetCulture(bool isEnglish)
        {
            if (isEnglish)
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            }
            else
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("ar-SA");
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("ar-SA");
            }
        }
        private void SaveImage(string folderPath, byte[] img, string fileName)
        {
            if (System.IO.Directory.Exists(folderPath))
            {
                using (MemoryStream ms = new MemoryStream(img))
                {
                    using (Bitmap bm2 = new Bitmap(ms))
                    {
                        bm2.Save(folderPath + "\\" + fileName);
                        bm2.Dispose();
                        //System.IO.File.WriteAllBytes(Path.Combine(folderPath, fileName), img);
                    }
                    ms.Dispose();
                }
            }
        }

    }
}