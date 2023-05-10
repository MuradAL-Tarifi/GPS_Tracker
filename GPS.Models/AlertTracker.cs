using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace GPS.Models
{
    public partial class AlertTracker
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
