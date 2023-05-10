using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace GPS.Models
{
    public partial class AlertBySensor
    {
        public int Id { get; set; }
        public long? InventoryId { get; set; }
        public long? WarehouseId { get; set; }
        public double? MinValueTemperature { get; set; }
        public double? MaxValueTemperature { get; set; }
        public double? MinValueHumidity { get; set; }
        public double? MaxValueHumidity { get; set; }
        public string ToEmails { get; set; }
        public int? AlertTypeLookupId { get; set; }
        public int? Interval { get; set; }
        public string Serial { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UserName { get; set; }
    }
}
