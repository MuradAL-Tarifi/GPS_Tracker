using System;
using System.Collections.Generic;
using System.Text;

namespace GPS.Domain.ViewModels
{
    public class TemperatureAndHumiditySensorHistoryReportResult
    {
        public int TotalRecords { get; set; }
        public TemperatureAndHumiditySensorReportHeaderInfo HeaderInfo { get; set; }
        public List<TemperatureAndHumiditySensorReportMonthHistory> MonthList { get; set; } = new List<TemperatureAndHumiditySensorReportMonthHistory>();
        public long? InventoryId { get; set; }
        public string SensorSerial { get; set; }
    }

    public class TemperatureAndHumiditySensorReportMonthHistory
    {
        public TemperatureAndHumiditySensorReportHeaderInfo HeaderInfo { get; set; }
        public List<TemperatureAndHumiditySensorReportDayHistory> DayList { get; set; } = new List<TemperatureAndHumiditySensorReportDayHistory>();
    }

    public class TemperatureAndHumiditySensorReportDayHistory
    {
        public TemperatureAndHumiditySensorReportHeaderInfo HeaderInfo { get; set; }
        public List<TemperatureAndHumiditySensorReportHistory> HistoryList { get; set; } = new List<TemperatureAndHumiditySensorReportHistory>();
    }

    public class TemperatureAndHumiditySensorReportHeaderInfo
    {
        public string DateText { get; set; }

        public decimal? MaxTemperature { get; set; }
        public decimal? MinTemperature { get; set; }
        public decimal? AverageTemperature { get; set; }

        public decimal? MaxHumidity { get; set; }
        public decimal? MinHumidity { get; set; }
        public decimal? AverageHumidity { get; set; }
    }

    public class TemperatureAndHumiditySensorReportHistory
    {
        public string GPSDate { get; set; }
        public string Temperature { get; set; }
        public string Humidity { get; set; }
    }
}