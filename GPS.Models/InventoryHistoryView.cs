using System;

namespace GPS.Models
{
    public class InventoryHistoryView
    {
        public string InventoryReferanceKey { get; set; }
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
    }
}
