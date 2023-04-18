using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Domain.ViewModels
{
    public class ReportOptionsDetailsModel
    {
        public DTO.ReportTypeEnum ReportTypeId { get; set; }
        public long? ReportScheduleId { get; set; }
        public string ReportTypeName { get; set; }
        public string GroupName { get; set; }
        public string VehicleName { get; set; }
        public string GeofenceName { get; set; }
        public string WarehouseName { get; set; }
        public string InventoryName { get; set; }
        public string SensorName { get; set; }
        public string AlertTypeName { get; set; }

        public string Name { get; set; }
        public bool NewerToOlder { get; set; }
        public bool IsActive { get; set; }

        public string GroupUpdatesByType { get; set; }
        public int? GroupUpdatesValue { get; set; }

        public ReportScheduling Scheduling { get; set; }
    }
}
