using System;
using System.Collections.Generic;
using System.Text;

namespace GPS.Domain.Views
{
    public class InventoryHistoryView
    {
        public long Id { get; set; }
        public long InventoryId { get; set; }
        public string GatewayIMEI { get; set; }
        public string Serial { get; set; }
        public decimal? Temperature { get; set; }
        public decimal? Humidity { get; set; }
        public bool? IsLowVoltage { get; set; }
        public DateTime GpsDate { get; set; }
        public string Alram { get; set; }
        public string GSMStatus { get; set; }

        public InventorySensorView InventorySensor { get; set; }
    }
}
