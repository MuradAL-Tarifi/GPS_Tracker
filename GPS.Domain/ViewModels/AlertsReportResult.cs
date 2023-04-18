using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Domain.ViewModels
{
    public class AlertsReportResult
    {
        public int TotalRecords { get; set; }
        public List<AlertsMonthHistory> MonthList { get; set; } = new List<AlertsMonthHistory>();
    }

    public class AlertsMonthHistory
    {
        public AlertsHeaderInfo HeaderInfo { get; set; }
        public List<AlertsDayHistory> DayList { get; set; } = new List<AlertsDayHistory>();
    }

    public class AlertsDayHistory
    {
        public AlertsHeaderInfo HeaderInfo { get; set; }
        public List<AlertsHistory> HistoryList { get; set; } = new List<AlertsHistory>();
    }

    public class AlertsHeaderInfo
    {
        public string DateText { get; set; }
    }

    public class AlertsHistory
    {
        public string GeofenceBorders { get; set; }
        public string AlertDateTime { get; set; }
        public string VehicleInfo { get; set; }
        public string AlertText { get; set; }
        public string AlertForValue { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }
}
