using System;
using System.Collections.Generic;
using System.Text;

namespace GPS.Integration.WaslModels
{
    public class MaintenanceOperationModel
    {
        public long Id { get; set; }
        public long GroupId { get; set; }
        public long VehicleId { get; set; }
        public bool IsAgencyMaintenance { get; set; }
        public DateTime? MaintenanceDate { get; set; }
        public decimal? Odometer { get; set; }
        public int? NextMaintenanceAfterDistance { get; set; }
        public decimal? Cost { get; set; }
        public string Description { get; set; }
        public bool SendAlert { get; set; }
    }
}
