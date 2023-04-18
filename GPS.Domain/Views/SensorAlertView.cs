using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Domain.Views
{
    public class SensorAlertView
    {
        public long Id { get; set; }
        public long InventorySensorId { get; set; }
        public int SensorAlertTypeLookupId { get; set; }
        public bool IsActive { get; set; }
        public bool IsSMS { get; set; }
        public bool IsEmail { get; set; }
        public decimal? FromValue { get; set; }
        public decimal? ToValue { get; set; }

        public InventorySensorView InventorySensor { get; set; }
        public SensorAlertTypeLookupView SensorAlertTypeLookup { get; set; }
    }
}
