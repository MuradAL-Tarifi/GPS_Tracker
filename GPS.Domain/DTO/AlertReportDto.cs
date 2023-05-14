using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Domain.DTO
{
    public class AlertReportDto
    {
        public string AlertType { get; set; }
        public DateTime? AlertDateTime { get; set; }
        public string MessageForValue { get; set; }
        public string ToEmails { get; set; }
        public string MonitoredUnit { get; set; }
        public string SensorNumber { get; set; }
        public string Warehouse { get; set; }
        public string Fleet { get; set; }

    }
}
