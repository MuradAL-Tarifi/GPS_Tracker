using AutoMapper;
using GPS.DataAccess.Repository.UnitOfWork;
using GPS.Domain.DTO;
using GPS.Domain.Models;
using GPS.Domain.ViewModels;
using GPS.Domain.Views;
using GPS.Resources;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Services.ReportsSchedule
{
    public class ReportScheduleService : IReportScheduleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ReportScheduleService> _logger;
        private readonly IStringLocalizer<SharedResources> _sharedLocalizer;

        public ReportScheduleService(
             IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<ReportScheduleService> logger,
            IStringLocalizer<SharedResources> sharedLocalizer)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _sharedLocalizer = sharedLocalizer;
            _logger = logger;
        }

        public async Task<ReturnResult<PagedResult<ReportScheduleView>>> SearchAsync(GenericSearchModel searchModel)
        {
            var result = new ReturnResult<PagedResult<ReportScheduleView>>();
            try
            {
                var pagedResult = await _unitOfWork.ReportScheduleRepository.SearchAsync(searchModel);
                var pagedListView = new PagedResult<ReportScheduleView>
                {
                    TotalRecords = pagedResult.TotalRecords,
                    List = _mapper.Map<List<ReportScheduleView>>(pagedResult.List)
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

        public async Task<ReturnResult<ReportOptionsModel>> GetByIdAsync(long Id)
        {
            var result = new ReturnResult<ReportOptionsModel>();
            try
            {
                var reportSchedule = await _unitOfWork.ReportScheduleRepository.GetByIdAsync(Id);
                if (reportSchedule == null)
                {
                    result.NotFound(_sharedLocalizer["ReportNotExists"]);
                    return result;
                }

                var reportOptions = _mapper.Map<ReportOptionsModel>(reportSchedule);
                result.Success(reportOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }

        public async Task<ReturnResult<ReportOptionsDetailsModel>> GetReportDetailsAsync(long id)
        {
            var result = new ReturnResult<ReportOptionsDetailsModel>();
            try
            {
                var reportSchedule = await _unitOfWork.ReportScheduleRepository.GetDetailsByIdAsync(id);
                if (reportSchedule == null)
                {
                    result.NotFound(_sharedLocalizer["ReportNotExists"]);
                    return result;
                }

                var reportOptions = _mapper.Map<ReportOptionsDetailsModel>(reportSchedule);
                if (reportSchedule.ReportTypeLookupId == (int)ReportTypeEnum.InventorySensorsHistoryReport ||
                    reportSchedule.ReportTypeLookupId == (int)ReportTypeEnum.InventoryHistoryReport)
                {
                    if(!String.IsNullOrEmpty(reportSchedule.SensorSerial))
                    {
                        var inventorySensor = await _unitOfWork.InventorySensorRepository.GetBasicBySerial(reportSchedule.SensorSerial);
                        reportOptions.SensorName = $@"{inventorySensor.Sensor} / {inventorySensor.Sensor.Serial}";
                    }
                }

                result.Success(reportOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }

        public async Task<ReturnResult<int>> CountAsync(long? FleetId = null)
        {
            var result = new ReturnResult<int>();
            try
            {
                int Count = await _unitOfWork.ReportScheduleRepository.CountAsync();
                result.Success(Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }

        public async Task<ReturnResult<bool>> DeleteAsync(long Id, string UpdatedBy)
        {
            var result = new ReturnResult<bool>();
            try
            {
                var reportSchedule = await _unitOfWork.ReportScheduleRepository.DeleteAsync(Id, UpdatedBy);
                if (reportSchedule == null)
                {
                    result.NotFound(_sharedLocalizer["ReportNotExists"]);
                    result.Data = false;
                    return result;
                }

                await _unitOfWork.EventLogRepository.LogEventAsync(Event.delete, reportSchedule.Id, reportSchedule, UpdatedBy);

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

        public async Task<ReturnResult<bool>> ActiveStatusAsync(long Id, bool activate, string UpdatedBy)
        {
            var result = new ReturnResult<bool>();
            try
            {
                var reportSchedule = await _unitOfWork.ReportScheduleRepository.ActiveStatusAsync(Id, activate, UpdatedBy);
                if (reportSchedule == null)
                {
                    result.NotFound(_sharedLocalizer["ReportNotExists"]);
                    result.Data = false;
                    return result;
                }

                await _unitOfWork.EventLogRepository.LogEventAsync(Event.delete, reportSchedule.Id, reportSchedule, UpdatedBy);

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

        public async Task<ReturnResult<bool>> SaveAsync(ReportOptionsModel reportOptions)
        {
            if (reportOptions.ReportScheduleId > 0)
            {
                return await UpdateAsync(reportOptions);
            }
            else
            {
                return await AddAsync(reportOptions);
            }
        }

        private async Task<ReturnResult<bool>> AddAsync(ReportOptionsModel reportOptions)
        {
            var result = new ReturnResult<bool>();
            try
            {
                var reportScheduleView = _mapper.Map<ReportScheduleView>(reportOptions);
                reportScheduleView.CreatedBy = reportOptions.UserId;
                var newReportSchedule = _mapper.Map<ReportSchedule>(reportScheduleView);

                newReportSchedule = await _unitOfWork.ReportScheduleRepository.AddAsync(newReportSchedule);

                await _unitOfWork.EventLogRepository.LogEventAsync(Event.create, newReportSchedule.Id, newReportSchedule, reportOptions.UserId);

                result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }

            return result;
        }

        private async Task<ReturnResult<bool>> UpdateAsync(ReportOptionsModel reportOptions)
        {
            var result = new ReturnResult<bool>();
            try
            {

                var oldReportSchedule = await _unitOfWork.ReportScheduleRepository.GetByIdAsync(reportOptions.ReportScheduleId.Value);
                if (oldReportSchedule == null)
                {
                    result.NotFound(_sharedLocalizer["ReportNotExists"]);
                    return result;
                }

                var reportScheduleView = _mapper.Map<ReportScheduleView>(reportOptions);
                reportScheduleView.CreatedBy = oldReportSchedule.CreatedBy;
                reportScheduleView.CreatedDate = oldReportSchedule.CreatedDate;
                reportScheduleView.UpdatedBy = reportScheduleView.UserId;
                reportScheduleView.UpdatedDate = DateTime.Now;

                oldReportSchedule = _mapper.Map<ReportSchedule>(reportScheduleView);

                oldReportSchedule = await _unitOfWork.ReportScheduleRepository.UpdateAsync(oldReportSchedule);

                await _unitOfWork.EventLogRepository.LogEventAsync(Event.create, reportScheduleView.Id, reportScheduleView, reportOptions.UserId);

                result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }

            return result;
        }





        //public async Task GenerateAndSendGeoFenceReport()
        //{
        //    LogHistory("Generate and Send GeoFence Report");

        //    SetCulture(false);

        //    DateTime now = DateTime.Now;
        //    DateTime fromDateTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0).AddHours(-3);
        //    DateTime toDateTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0);
        //    var fromDateString = fromDateTime.ToString("yyyy/MM/dd hh:mm:ss tt", new CultureInfo("en-US").DateTimeFormat);
        //    var toDateString = toDateTime.ToString("yyyy/MM/dd hh:mm:ss tt", new CultureInfo("en-US").DateTimeFormat);

        //    var fleetIds = _configuration.GetSection("GeofenceReport:FleetIds").Get<long[]>();
        //    var emails = _configuration.GetSection("GeofenceReport:Emails").Get<string[]>();

        //    foreach (var fleetId in fleetIds)
        //    {
        //        var geofences = await _unitOfWork.GeoFenceRepository.GetByFleetIdAsync(fleetId);

        //        foreach (var geofence in geofences)
        //        {
        //            var emailAttachments = new List<EmailAttachmentModel>();
        //            var excelReportResult = await _reportApiProxy.GenerateGeoFenceReport(geofence.Id, fromDateString, toDateString);
        //            if (excelReportResult.IsSuccess && excelReportResult.Data != null)
        //            {
        //                emailAttachments.Add(new EmailAttachmentModel()
        //                {
        //                    Name = $"تقرير السياج الجغرافي.xlsx",
        //                    Content = excelReportResult.Data
        //                });

        //                try
        //                {
        //                    var fleet = await _unitOfWork.FleetRepository.FindByIdAsync(geofence.FleetId);
        //                    string fleetName = fleet.Name;

        //                    string title = $"EWE Tracker Report | تقرير السياج الجغرافي | {geofence.Name}";
        //                    var templatePath = _hostingEnvironment.WebRootPath + @"\assets\emailTemplates\GeoFenceReport.xml";
        //                    string body = System.IO.File.ReadAllText(templatePath);
        //                    body = body.Replace("{ReportTitle}", "تقرير السياج الجغرافي");
        //                    body = body.Replace("{ReportName}", "تقرير السياج الجغرافي");
        //                    body = body.Replace("{ScheduleType}", "");
        //                    body = body.Replace("{Date}", toDateString);
        //                    body = body.Replace("{FleetLabel}", _sharedLocalizer["Fleet"]);
        //                    body = body.Replace("{GeofenceLabel}", _sharedLocalizer["Geofence"]);
        //                    body = body.Replace("{DateLabel}", _sharedLocalizer["Date"]);
        //                    body = body.Replace("{Fleet}", fleetName);
        //                    body = body.Replace("{Geofence}", geofence.Name);
        //                    body = body.Replace("{Dates}", $"{fromDateString} -> {toDateString}");

        //                    var sent = await _emailIntegration.SendEmailWithAttachmentsAsync(title, body, emails, emailAttachments);

        //                    // add email history
        //                    EmailHistoryView emailHistoryView = new EmailHistoryView()
        //                    {
        //                        Title = title,
        //                        Body = body,
        //                        ToEmails = string.Join(",", emails),
        //                        IsSent = sent,
        //                        CreatedDate = DateTime.Now
        //                    };

        //                    if (sent)
        //                    {
        //                        emailHistoryView.SentDate = DateTime.Now;
        //                        await _emailHistoryService.AddAsync(emailHistoryView);
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    _logger.LogError(ex, ex.Message, null);
        //                }
        //            }
        //        }
        //    }
        //}

        //private void LogHistory(string job)
        //{
        //    using (StreamWriter writetext = File.AppendText(@"C:\temp\JobHistory.txt"))
        //    {
        //        writetext.WriteLine($"{DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss tt", new CultureInfo("en-US").DateTimeFormat)}: {job} job started");
        //    }
        //}
    }
}
