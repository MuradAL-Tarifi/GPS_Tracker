using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Domain.ViewModels
{
    public class AlertTrackerViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public DateTime? AlertDateTime { get; set; }
        public string AlertType { get; set; }
        public string MonitoredUnit { get; set; }
        public string MessageForValue { get; set; }
        public string Serial { get; set; }
        public string Zone { get; set; }
        public string WarehouseName { get; set; }
        public string SendTo { get; set; }
        public bool? IsSend { get; set; }
        public int? AlertId { get; set; }
        public int? Interval { get; set; }
    }
}
