using System;
using System.Collections.Generic;
using System.Text;

namespace GPS.Domain.ViewModels
{
    public class TemperatureAndHumidityHistoryReportResult
    {
        public int TotalRecords { get; set; }
        public TemperatureAndHumidityReportHeaderInfo HeaderInfo { get; set; }
        public List<TemperatureAndHumidityReportMonthHistory> MonthList { get; set; } = new List<TemperatureAndHumidityReportMonthHistory>();
    }

    public class TemperatureAndHumidityReportMonthHistory
    {
        public TemperatureAndHumidityReportHeaderInfo HeaderInfo { get; set; }
        public List<TemperatureAndHumidityReportDayHistory> DayList { get; set; } = new List<TemperatureAndHumidityReportDayHistory>();
    }

    public class TemperatureAndHumidityReportDayHistory
    {
        public TemperatureAndHumidityReportHeaderInfo HeaderInfo { get; set; }
        public List<TemperatureAndHumidityReportHistory> HistoryList { get; set; } = new List<TemperatureAndHumidityReportHistory>();
    }   

    public class TemperatureAndHumidityReportHeaderInfo
    {
        public string DateText { get; set; }

        public decimal? MaxTemperature1 { get; set; }
        public decimal? MinTemperature1 { get; set; }
        public decimal? AverageTemperature1 { get; set; }

        public decimal? MaxTemperature2 { get; set; }
        public decimal? MinTemperature2 { get; set; }
        public decimal? AverageTemperature2 { get; set; }

        public decimal? MaxTemperature3 { get; set; }
        public decimal? MinTemperature3 { get; set; }
        public decimal? AverageTemperature3 { get; set; }

        public decimal? MaxTemperature4 { get; set; }
        public decimal? MinTemperature4 { get; set; }
        public decimal? AverageTemperature4 { get; set; }

        public decimal? MaxHumidity1 { get; set; }
        public decimal? MinHumidity1 { get; set; }
        public decimal? AverageHumidity1 { get; set; }

        public decimal? MaxHumidity2 { get; set; }
        public decimal? MinHumidity2 { get; set; }
        public decimal? AverageHumidity2 { get; set; }

        public decimal? MaxHumidity3 { get; set; }
        public decimal? MinHumidity3 { get; set; }
        public decimal? AverageHumidity3 { get; set; }

        public decimal? MaxHumidity4 { get; set; }
        public decimal? MinHumidity4 { get; set; }
        public decimal? AverageHumidity4 { get; set; }
    }

    public class TemperatureAndHumidityReportHistory
    {
        public string GPSDate { get; set; }
        public string Temperature1 { get; set; }
        public string Temperature2 { get; set; }
        public string Temperature3 { get; set; }
        public string Temperature4 { get; set; }
        public string Humidity1 { get; set; }
        public string Humidity2 { get; set; }
        public string Humidity3 { get; set; }
        public string Humidity4 { get; set; }
    }
}