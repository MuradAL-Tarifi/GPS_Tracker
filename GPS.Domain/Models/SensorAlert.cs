namespace GPS.Domain.Models
{
    public class SensorAlert
    {
        public long Id { get; set; }
        public long InventorySensorId { get; set; }
        public int SensorAlertTypeLookupId { get; set; }
        public bool IsActive { get; set; }
        public bool IsSMS { get; set; }
        public bool IsEmail { get; set; }
        public decimal? FromValue { get; set; }
        public decimal? ToValue { get; set; }

        public InventorySensor InventorySensor { get; set; }
        public SensorAlertTypeLookup SensorAlertTypeLookup { get; set; }
    }
}
