using System;

namespace GPS.Domain.Models
{
    public class CustomAlert
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public double? MinValueTemperature { get; set; }
        public double? MaxValueTemperature { get; set; }
        public double? MinValueHumidity { get; set; }
        public double? MaxValueHumidity { get; set; }
        public int Interval { get; set; }
        public string ToEmails { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? LastAlertDate { get; set; }
        public int AlertTypeLookupId { get; set; }
        public long FleetId { get; set; }
        public string UserIds { get; set; }
        public Fleet Fleet { get; set; }
        public AlertTypeLookup AlertTypeLookup { get; set; }
    }
}
