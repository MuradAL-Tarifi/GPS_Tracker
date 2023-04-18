using GPS.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Models
{
    public class SensorAlert
    {
        public long Id { get; set; }
        public int InventorySensorId { get; set; }
        public SensorAlertTypeLookupEnum SensorAlertTypeLookupId { get; set; }
        public bool IsActive { get; set; }
        public bool IsSMS { get; set; }
        public bool IsEmail { get; set; }
        public double? FromValue { get; set; }
        public double? ToValue { get; set; }

    }
}
