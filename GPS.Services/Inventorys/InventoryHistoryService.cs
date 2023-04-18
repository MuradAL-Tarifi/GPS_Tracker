using AutoMapper;
using GPS.DataAccess.Repository.UnitOfWork;
using GPS.Domain.DTO;
using GPS.Domain.Models;
using GPS.Domain.ViewModels;
using GPS.Domain.Views;
using GPS.Helper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Services.Inventorys
{
    public class InventoryHistoryService : IInventoryHistoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<InventoryHistoryService> _logger;


        public InventoryHistoryService(
            IMapper mapper,
            ILogger<InventoryHistoryService> logger,
            IUnitOfWork unitOfWork)
        {

            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ReturnResult<long>> SaveAsync(InventoryHistoryView inventoryHistoryView)
        {
            var result = new ReturnResult<long>();
            try
            {
                await _unitOfWork.InventoryHistoryRepository.InsertAsync(inventoryHistoryView);

                // Update Online History
                var onlineHistory = await _unitOfWork.OnlineInventoryHistoryRepository.GetBySensorSerialAsync(inventoryHistoryView.Serial);

                if (onlineHistory == null)
                {
                    onlineHistory = _mapper.Map<OnlineInventoryHistory>(inventoryHistoryView);
                    await _unitOfWork.OnlineInventoryHistoryRepository.InsertAsync(onlineHistory);
                }
                else
                {
                    if (onlineHistory.GpsDate > inventoryHistoryView.GpsDate)
                    {
                        result.Success(1);
                        return result;
                    }

                    onlineHistory = _mapper.Map<OnlineInventoryHistory>(inventoryHistoryView);
                    await _unitOfWork.OnlineInventoryHistoryRepository.UpdateAsync(onlineHistory);
                }

                result.Success(1);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }

            return result;
        }

        public async Task<ReturnResult<List<InventoryHistoryView>>> SensorHistory(long inventoryId, string sensorSerial,
        string fromDate, string toDate)
        {
            var result = new ReturnResult<List<InventoryHistoryView>>();

            DateTime fromDateTime = GPSHelper.StringToDateTime(fromDate);
            DateTime toDateTime = GPSHelper.StringToDateTime(toDate);

            try
            {
                var history = await _unitOfWork.InventoryHistoryRepository.GetByInventoryIdAsync(inventoryId, fromDateTime, toDateTime);
                history = history.Where(x => x.Serial == sensorSerial).ToList();

                var historyView = _mapper.Map<List<InventoryHistoryView>>(history);

                result.Success(historyView);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }

        public async Task<ReturnResult<GatewayHistoryReport>> PagedHistory(long inventoryId, string sensorSerial,
            string fromDate, string toDate, int pageNumber, int pageSize)
        {
            var result = new ReturnResult<GatewayHistoryReport>();

            DateTime fromDateTime = GPSHelper.StringToDateTime(fromDate);
            DateTime toDateTime = GPSHelper.StringToDateTime(toDate);

            try
            {
                if (pageNumber > 1)
                {
                    pageSize--;
                }

                var skip = (pageNumber - 1) * pageSize;

                var inventoryHistory = await _unitOfWork.InventoryHistoryRepository.GetByInventoryIdAsync(inventoryId, fromDateTime, toDateTime);

                var totalRecords = inventoryHistory
                           .Count(x => x.InventoryId == inventoryId &&
                           x.GpsDate >= fromDateTime && x.GpsDate <= toDateTime &&
                           (string.IsNullOrEmpty(sensorSerial) || x.Serial == sensorSerial));

                var historyRecords = inventoryHistory
                     .Where(x => x.InventoryId == inventoryId &&
                        (string.IsNullOrEmpty(sensorSerial) || x.Serial == sensorSerial))
                        .Skip(skip).Take(pageSize)
                    .ToList();

                if (historyRecords.Count > 0)
                {

                    var historyRecordsView = _mapper.Map<List<InventoryHistoryView>>(historyRecords);

                    var maxTemperature = historyRecordsView.Max(x => x.Temperature);
                    var maxHumidity = historyRecordsView.Max(x => x.Humidity);

                    result.Success(new GatewayHistoryReport()
                    {
                        TotalRecords = totalRecords,
                        MaxTemperature = maxTemperature,
                        MaxHumidity = maxHumidity,
                        Records = historyRecordsView
                    });
                }
                else
                {
                    result.NotFound(result.ErrorList);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }
       

        public async Task<ReturnResult<TemperatureAndHumiditySensorHistoryReportResult>> PagedSensorTemperatureAndHumidityHistoryAsync(long inventoryId, string sensorSerial,
            string fromDate, string toDate, int pageNumber, int pageSize, string groupUpdatesByType, int? groupUpdatesValue, bool isEnglish)
        {
            DateTime fromDateTime = GPSHelper.StringToDateTime(fromDate);
            DateTime toDateTime = GPSHelper.StringToDateTime(toDate);

            var result = new ReturnResult<TemperatureAndHumiditySensorHistoryReportResult>();
            try
            {
                var reportList = new TemperatureAndHumiditySensorHistoryReportResult();

                if (inventoryId > 0 && !string.IsNullOrEmpty(sensorSerial))
                {
                    var skip = (pageNumber - 1) * pageSize;

                    var history = await _unitOfWork.InventoryHistoryRepository.GetByInventoryIdAndSensorSerialAsync(inventoryId, sensorSerial, fromDateTime, toDateTime);

                    if (!string.IsNullOrEmpty(groupUpdatesByType))
                    {
                        reportList = GroupSensorTemperatureAndHumidityHistory(history, groupUpdatesByType, groupUpdatesValue, isEnglish);
                    }
                    else
                    {
                        var pagedHistory = history.Skip(skip).Take(pageSize).ToList();
                        reportList = GroupSensorTemperatureAndHumidityHistory(history, groupUpdatesByType, groupUpdatesValue, isEnglish, pagedHistory);
                    }
                }

                result.Success(reportList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }

        public async Task<ReturnResult<List<TemperatureAndHumiditySensorHistoryReportResult>>> GetListGroupedSensorsTemperatureAndHumidityHistoryAsync(FilterInventoryHistory filterInventoryHistory)
        {
            DateTime fromDateTime = GPSHelper.StringToDateTime(filterInventoryHistory.fromDate);
            DateTime toDateTime = GPSHelper.StringToDateTime(filterInventoryHistory.toDate);
            var result = new ReturnResult<List<TemperatureAndHumiditySensorHistoryReportResult>>();
            var lsTemperatureAndHumiditySensorHistoryReportResult = new List<TemperatureAndHumiditySensorHistoryReportResult>();
            try
            {
                if (filterInventoryHistory.lsSensorSerials.Count > 0)
                {
                    var history = await _unitOfWork.InventoryHistoryRepository.GetBySensorsSerialsAsync(filterInventoryHistory.lsSensorSerials, fromDateTime, toDateTime);
                    var groupedHistoryBySensorSerials = history.GroupBy(x => x.Serial);
                    foreach(var groupedHistoryBySN in groupedHistoryBySensorSerials)
                    {
                        var reportList = GroupSensorTemperatureAndHumidityHistory(groupedHistoryBySN.ToList(), filterInventoryHistory.groupUpdatesByType, filterInventoryHistory.groupUpdatesValue, filterInventoryHistory.isEnglish);
                        if (reportList != null)
                        {
                            lsTemperatureAndHumiditySensorHistoryReportResult.Add(reportList);
                        }
                    }
                    result.Success(lsTemperatureAndHumiditySensorHistoryReportResult);
                }
                else
                {
                    result.NotFound($"Search List Sensor Serial Is Empty");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }
        public async Task<ReturnResult<TemperatureAndHumiditySensorHistoryReportResult>> GetGroupedSensorTemperatureAndHumidityHistoryAsync(long inventoryId, string sensorSerial,
            string fromDate, string toDate, string groupUpdatesByType, int? groupUpdatesValue, bool isEnglish)
        {
            DateTime fromDateTime = GPSHelper.StringToDateTime(fromDate);
            DateTime toDateTime = GPSHelper.StringToDateTime(toDate);

            var result = new ReturnResult<TemperatureAndHumiditySensorHistoryReportResult>();
            try
            {
                if (inventoryId > 0 &&  !string.IsNullOrEmpty(sensorSerial))
                {
                    var history = await _unitOfWork.InventoryHistoryRepository.GetByInventoryIdAndSensorSerialAsync(inventoryId, sensorSerial, fromDateTime, toDateTime);

                    var reportList = GroupSensorTemperatureAndHumidityHistory(history, groupUpdatesByType, groupUpdatesValue, isEnglish);
                    result.Success(reportList);
                }
                else
                {
                    result.NotFound($"inventory and sensor with id {inventoryId} and serial {sensorSerial} not found");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }

        public async Task<ReturnResult<List<InventoryHistoryView>>> GetByInventoryIdAsync(long inventoryId, string fromDate, string toDate)
        {
            var result = new ReturnResult<List<InventoryHistoryView>>();

            DateTime fromDateTime = GPSHelper.StringToDateTime(fromDate);
            DateTime toDateTime = GPSHelper.StringToDateTime(toDate);

            try
            {
                var history = await _unitOfWork.InventoryHistoryRepository.GetByInventoryIdAsync(inventoryId, fromDateTime, toDateTime);
                var historyView = _mapper.Map<List<InventoryHistoryView>>(history);

                result.Success(historyView);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }

        public async Task<ReturnResult<List<GroupedInventoryAverageTemperatureAndHumidity>>> InventorySensorsAverageTemperatureAndHumidityByHourAsync(long inventoryId,string fromDate, string toDate)
        {
            var result = new ReturnResult<List<GroupedInventoryAverageTemperatureAndHumidity>>();
            var lsGroupedInventoryAverageTemperatureAndHumidity = new List<GroupedInventoryAverageTemperatureAndHumidity>();
            DateTime fromDateTime = GPSHelper.StringToDateTime(fromDate);
            DateTime toDateTime = GPSHelper.StringToDateTime(toDate);

            try
            {
                var history = await _unitOfWork.InventoryHistoryRepository.GetByInventoryIdAsync(inventoryId, fromDateTime, toDateTime);
                if (history.Count > 0)
                {
                    var groupedHistoryBySensorSerial = history.GroupBy(x => x.Serial);
                    foreach(var groupedHistroyBySensorSN  in groupedHistoryBySensorSerial)
                    {
                        var groupedList = groupedHistroyBySensorSN.OrderBy(x => x.GpsDate.Hour).GroupBy(x => x.GpsDate.Hour);
                        var groupedInventoryAverageTemperatureAndHumidity = new GroupedInventoryAverageTemperatureAndHumidity();
                        foreach (var groupItem in groupedList)
                        {
                            var headerInfo = new InventorySensorTemperatureAndHumidityChartHeaderInfo
                            {
                                HourText = new DateTime(fromDateTime.Year, fromDateTime.Month, fromDateTime.Day, groupItem.Key, 0, 0),
                                AverageTemperature = groupItem.Average(x => x.Temperature).HasValue ? Math.Round(groupItem.Average(x => x.Temperature).Value, 2) : null,
                                AverageHumidity = groupItem.Average(x => x.Humidity).HasValue && groupItem.Average(x => x.Humidity) > 0 ? Math.Round(groupItem.Average(x => x.Humidity).Value, 2) : null,
                            };

                            groupedInventoryAverageTemperatureAndHumidity.LsInventorySensorTemperatureAndHumidityChartHeaderInfo.Add(headerInfo);
                        }
                        groupedInventoryAverageTemperatureAndHumidity.SensorName = groupedHistroyBySensorSN.Key;
                        lsGroupedInventoryAverageTemperatureAndHumidity.Add(groupedInventoryAverageTemperatureAndHumidity);
                    }
                }
                result.Success(lsGroupedInventoryAverageTemperatureAndHumidity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }

        private TemperatureAndHumiditySensorHistoryReportResult GroupSensorTemperatureAndHumidityHistory(List<InventoryHistory> history, string groupUpdatesByType, int? groupUpdatesValue, bool isEnglish, List<InventoryHistory> pagedHistory = null)
        {
            if (pagedHistory == null)
            {
                pagedHistory = history;
            }

            var reportList = new TemperatureAndHumiditySensorHistoryReportResult
            {
                TotalRecords = history.Count,
                HeaderInfo = new TemperatureAndHumiditySensorReportHeaderInfo()
                {
                    MaxTemperature = history.Max(x => x.Temperature).HasValue ? Math.Round(history.Max(x => x.Temperature).Value, 2) : null,
                    MinTemperature = history.Min(x => x.Temperature).HasValue ? Math.Round(history.Min(x => x.Temperature).Value, 2) : null,
                    AverageTemperature = history.Average(x => x.Temperature).HasValue ? Math.Round(history.Average(x => x.Temperature).Value, 2) : null,
                    MaxHumidity = history.Max(x => x.Humidity).HasValue && history.Min(x => x.Humidity) > 0 ? Math.Round(history.Max(x => x.Humidity).Value, 2) : null,
                    MinHumidity = history.Min(x => x.Humidity).HasValue && history.Min(x => x.Humidity) > 0 ? Math.Round(history.Min(x => x.Humidity).Value, 2) : null,
                    AverageHumidity = history.Average(x => x.Humidity).HasValue && history.Min(x => x.Humidity) > 0 ? Math.Round(history.Average(x => x.Humidity).Value, 2) : null,
                },
                InventoryId = history.Count > 0 ? history.First().InventoryId : null,
                SensorSerial = history.Count > 0 ? history.First().Serial : null,
            };

            reportList.MonthList = new List<TemperatureAndHumiditySensorReportMonthHistory>();

            int skip = 0;

            var pagedHistoryGroupedByYear = pagedHistory.GroupBy(x => x.GpsDate.Year);
            var historyGroupedByYear = history.GroupBy(x => x.GpsDate.Year);

            foreach (var pagedHistoryYearItem in pagedHistoryGroupedByYear)
            {
                var yearItem = historyGroupedByYear.FirstOrDefault(x => x.Key == pagedHistoryYearItem.Key);

                var pagedHistoryGroupedByMonth = pagedHistoryYearItem.GroupBy(x => x.GpsDate.Month);
                var historyGroupedByMonth = yearItem.GroupBy(x => x.GpsDate.Month);

                int monthIndex = 0;
                int dayIndex = 0;
                foreach (var pagedHistoryMonthItem in pagedHistoryGroupedByMonth)
                {
                    var historyMonthItem = historyGroupedByMonth.FirstOrDefault(x => x.Key == pagedHistoryMonthItem.Key);

                    var monthDateText = "";
                    if (isEnglish)
                    {
                        monthDateText = historyMonthItem.FirstOrDefault().GpsDate.ToString("MMMM", new CultureInfo("en-US").DateTimeFormat);
                    }
                    else
                    {
                        monthDateText = @$"{historyMonthItem.FirstOrDefault().GpsDate.ToString("MMMM", new CultureInfo("ar-EG").DateTimeFormat)}
                            <br/>
                            {historyMonthItem.FirstOrDefault().GpsDate.ToString("MMMM", new CultureInfo("en-US").DateTimeFormat)}";
                    }

                    var monthHeaderInfo = new TemperatureAndHumiditySensorReportHeaderInfo
                    {
                        DateText = monthDateText,
                        MaxTemperature = historyMonthItem.Max(x => x.Temperature).HasValue ? Math.Round(historyMonthItem.Max(x => x.Temperature).Value, 2) : null,
                        MinTemperature = historyMonthItem.Min(x => x.Temperature).HasValue ? Math.Round(historyMonthItem.Min(x => x.Temperature).Value, 2) : null,
                        AverageTemperature = historyMonthItem.Average(x => x.Temperature).HasValue ? Math.Round(historyMonthItem.Average(x => x.Temperature).Value, 2) : null,
                        MaxHumidity = historyMonthItem.Max(x => x.Humidity).HasValue && history.Min(x => x.Humidity) > 0 ? Math.Round(historyMonthItem.Max(x => x.Humidity).Value, 2) : null,
                        MinHumidity = historyMonthItem.Min(x => x.Humidity).HasValue && history.Min(x => x.Humidity) > 0 ? Math.Round(historyMonthItem.Min(x => x.Humidity).Value, 2) : null,
                        AverageHumidity = historyMonthItem.Average(x => x.Humidity).HasValue && history.Min(x => x.Humidity) > 0 ? Math.Round(historyMonthItem.Average(x => x.Humidity).Value, 2) : null,
                    };

                    var pagedHistoryGroupedByDay = pagedHistoryMonthItem.GroupBy(x => x.GpsDate.Day);

                    var dayList = new List<TemperatureAndHumiditySensorReportDayHistory>();

                    var historyGroupedByDay = historyMonthItem.GroupBy(x => x.GpsDate.Day);

                    dayIndex = 0;
                    foreach (var pagedHistoryDayItem in pagedHistoryGroupedByDay)
                    {
                        var dayItem = historyGroupedByDay.FirstOrDefault(x => x.Key == pagedHistoryDayItem.Key);


                        var dayDateText = "";
                        if (isEnglish)
                        {
                            dayDateText = dayItem.FirstOrDefault().GpsDate.ToString("dddd yyyy/MM/dd", new CultureInfo("en-US").DateTimeFormat);
                        }
                        else
                        {
                            dayDateText = dayItem.FirstOrDefault().GpsDate.ToString("dddd yyyy/MM/dd", new CultureInfo("ar-EG").DateTimeFormat);
                        }

                        var dayHeaderInfo = new TemperatureAndHumiditySensorReportHeaderInfo
                        {
                            DateText = dayDateText,
                            MaxTemperature = dayItem.Max(x => x.Temperature).HasValue ? Math.Round(dayItem.Max(x => x.Temperature).Value, 2) : null,
                            MinTemperature = dayItem.Min(x => x.Temperature).HasValue ? Math.Round(dayItem.Min(x => x.Temperature).Value, 2) : null,
                            AverageTemperature = dayItem.Average(x => x.Temperature).HasValue ? Math.Round(dayItem.Average(x => x.Temperature).Value, 2) : null,
                            MaxHumidity = dayItem.Max(x => x.Humidity).HasValue && history.Min(x => x.Humidity) > 0 ? Math.Round(dayItem.Max(x => x.Humidity).Value, 2) : null,
                            MinHumidity = dayItem.Min(x => x.Humidity).HasValue && history.Min(x => x.Humidity) > 0 ? Math.Round(dayItem.Min(x => x.Humidity).Value, 2) : null,
                            AverageHumidity = dayItem.Average(x => x.Humidity).HasValue && history.Min(x => x.Humidity) > 0 ? Math.Round(dayItem.Average(x => x.Humidity).Value, 2) : null,
                        };

                        var temperatureAndHumidityDayHistory = new TemperatureAndHumiditySensorReportDayHistory()
                        {
                            HeaderInfo = dayHeaderInfo
                        };

                        if (groupUpdatesByType == "hour")
                        {
                            var groupByDayPagedHistoryDayList = pagedHistoryDayItem.OrderBy(x => x.GpsDate.Hour).GroupBy(x => x.GpsDate.Hour);

                            skip = 0;
                            while (skip < groupByDayPagedHistoryDayList.Count())
                            {
                                var groupedList = groupByDayPagedHistoryDayList.Skip(skip).Take(groupUpdatesValue.Value).ToList();
                                skip += groupUpdatesValue.Value;

                                List<InventoryHistory> groupHistory = new();
                                foreach (var groupItem in groupedList)
                                {
                                    groupHistory.AddRange(groupItem);
                                }

                                groupHistory = groupHistory.OrderBy(x => x.GpsDate).ToList();

                                decimal? MaxTemperature = groupHistory.Max(x => x.Temperature).HasValue ? Math.Round(groupHistory.Max(x => x.Temperature).Value, 2) : null;
                                decimal? MinTemperature = groupHistory.Min(x => x.Temperature).HasValue ? Math.Round(groupHistory.Min(x => x.Temperature).Value, 2) : null;
                                decimal? AverageTemperature = groupHistory.Average(x => x.Temperature).HasValue ? Math.Round(groupHistory.Average(x => x.Temperature).Value, 2) : null;
                                decimal? MaxHumidity = groupHistory.Max(x => x.Humidity).HasValue && history.Min(x => x.Humidity) > 0 ? Math.Round(groupHistory.Max(x => x.Humidity).Value, 2) : null;
                                decimal? MinHumidity = groupHistory.Min(x => x.Humidity).HasValue && history.Min(x => x.Humidity) > 0 ? Math.Round(groupHistory.Min(x => x.Humidity).Value, 2) : null;
                                decimal? AverageHumidity = groupHistory.Average(x => x.Humidity).HasValue && history.Min(x => x.Humidity) > 0 ? Math.Round(groupHistory.Average(x => x.Humidity).Value, 2) : null;

                                var gpsDate = groupHistory.FirstOrDefault().GpsDate;

                                var fromTime = new DateTime(gpsDate.Year, gpsDate.Month, gpsDate.Day, gpsDate.Hour, 0, 0);
                                var toTime = fromTime.AddHours(groupUpdatesValue.Value);
                                if (skip >= groupByDayPagedHistoryDayList.Count())
                                {
                                    toTime = groupHistory.LastOrDefault().GpsDate;
                                }

                                var fromTimeText = fromTime.ToString("hh:mm:ss tt", new CultureInfo("en-US").DateTimeFormat);
                                var toTimeText = toTime.ToString("hh:mm:ss tt", new CultureInfo("en-US").DateTimeFormat);


                                var historyRecord = new TemperatureAndHumiditySensorReportHistory
                                {
                                    GPSDate = toTimeText + "<br><i class='fa fa-arrow-up text-success'></i><br>" + fromTimeText
                                };

                                if (MaxTemperature.HasValue && MinTemperature.HasValue && AverageTemperature.HasValue)
                                {
                                    historyRecord.Temperature = @$"<table class='table m-0'>
                                                            <tr>
                                                                <td><i class='fa fa-arrow-up text-danger'></i> {MaxTemperature}</td>
                                                                <td><i class='fa fa-random'></i> {AverageTemperature}</td>
                                                                <td><i class='fa fa-arrow-down text-info'></i> {MinTemperature}</td>
                                                            </tr>
                                                        </table>";
                                }

                                if (MaxHumidity.HasValue && MinHumidity.HasValue && AverageHumidity.HasValue)
                                {
                                    historyRecord.Humidity = @$"<table class='table m-0'>
                                                            <tr>
                                                                <td><i class='fa fa-arrow-up text-danger'></i> {MaxHumidity}</td>
                                                                <td><i class='fa fa-random'></i> {AverageHumidity}</td>
                                                                <td><i class='fa fa-arrow-down text-info'></i> {MinHumidity}</td>
                                                            </tr>
                                                        </table>";
                                }

                                temperatureAndHumidityDayHistory.HistoryList.Add(historyRecord);
                            }

                            temperatureAndHumidityDayHistory.HistoryList.Reverse();
                            dayList.Add(temperatureAndHumidityDayHistory);
                        }
                        else
                        {
                            if (groupUpdatesByType != "day")
                            {
                                temperatureAndHumidityDayHistory.HistoryList = pagedHistoryDayItem.Select(x =>
                                new TemperatureAndHumiditySensorReportHistory
                                {
                                    GPSDate = x.GpsDate.ToString("yyyy/MM/dd hh:mm:ss tt", new CultureInfo("en-US").DateTimeFormat),
                                    Temperature = x.Temperature?.ToString(),
                                    Humidity = x.Humidity.HasValue && x.Humidity.Value > 0 ?  x.Humidity?.ToString() : null,
                                }).ToList();
                            }

                            dayList.Add(temperatureAndHumidityDayHistory);
                        }

                        dayIndex++;
                    }

                    reportList.MonthList.Add(new TemperatureAndHumiditySensorReportMonthHistory()
                    {
                        HeaderInfo = monthHeaderInfo,
                        DayList = dayList
                    });

                    monthIndex++;
                }
            }

            if (groupUpdatesByType == "day" && groupUpdatesValue > 1)
            {
                // group days
                foreach (var monthItem in reportList.MonthList)
                {
                    skip = 0;
                    var newList = new List<TemperatureAndHumiditySensorReportDayHistory>();

                    while (skip < monthItem.DayList.Count)
                    {
                        var groupedList = monthItem.DayList.Skip(skip).Take(groupUpdatesValue.Value).ToList();
                        skip += groupUpdatesValue.Value;

                        var DateText = groupedList.FirstOrDefault().HeaderInfo.DateText + "<br><i class='fa fa-arrow-up text-success'></i><br>" + groupedList.LastOrDefault().HeaderInfo.DateText;
                        if (groupedList.LastOrDefault().HeaderInfo.DateText == groupedList.FirstOrDefault().HeaderInfo.DateText)
                        {
                            DateText = groupedList.FirstOrDefault().HeaderInfo.DateText;
                        }

                        newList.Add(new TemperatureAndHumiditySensorReportDayHistory()
                        {
                            HeaderInfo = new TemperatureAndHumiditySensorReportHeaderInfo()
                            {
                                DateText = DateText,
                                MaxTemperature = groupedList.Any(x => x.HeaderInfo.MaxTemperature.HasValue) ? Math.Round(groupedList.Where(x => x.HeaderInfo.MaxTemperature.HasValue).Max(x => x.HeaderInfo.MaxTemperature.Value), 2) : null,
                                MinTemperature = groupedList.Any(x => x.HeaderInfo.MinTemperature.HasValue) ? Math.Round(groupedList.Where(x => x.HeaderInfo.MinTemperature.HasValue).Min(x => x.HeaderInfo.MinTemperature.Value), 2) : null,
                                AverageTemperature = groupedList.Any(x => x.HeaderInfo.AverageTemperature.HasValue) ? Math.Round(groupedList.Where(x => x.HeaderInfo.AverageTemperature.HasValue).Average(x => x.HeaderInfo.AverageTemperature.Value), 2) : null,
                                MaxHumidity = groupedList.Any(x => x.HeaderInfo.MaxHumidity.HasValue) && history.Min(x => x.Humidity) > 0 ? Math.Round(groupedList.Where(x => x.HeaderInfo.MaxHumidity.HasValue).Max(x => x.HeaderInfo.MaxHumidity.Value), 2) : null,
                                MinHumidity = groupedList.Any(x => x.HeaderInfo.MinHumidity.HasValue) && history.Min(x => x.Humidity) > 0 ? Math.Round(groupedList.Where(x => x.HeaderInfo.MinHumidity.HasValue).Min(x => x.HeaderInfo.MinHumidity.Value), 2) : null,
                                AverageHumidity = groupedList.Any(x => x.HeaderInfo.AverageHumidity.HasValue) && history.Min(x => x.Humidity) > 0 ? Math.Round(groupedList.Where(x => x.HeaderInfo.AverageHumidity.HasValue).Average(x => x.HeaderInfo.AverageHumidity.Value), 2) : null,
                            }
                        });
                    }

                    monthItem.DayList = newList;
                }
            }

            return reportList;
        }
    }
}
