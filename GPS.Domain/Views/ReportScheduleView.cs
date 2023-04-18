using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Domain.Views
{
    public class ReportScheduleView
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        public long FleetId { get; set; }
        public int ReportTypeLookupId { get; set; }
        public long? WarehouseId { get; set; }
        public long? InventoryId { get; set; }
        public long? SensorSerial { get; set; }
        public int? AlertTypeLookupId { get; set; }
        public string Name { get; set; }
        public bool NewerToOlder { get; set; }
        public bool? Daily { get; set; }
        public bool? Weekly { get; set; }
        public bool? Monthly { get; set; }
        public bool? Yearly { get; set; }
        public int? DayOfWeekId { get; set; }
        public int? DayOfMonthId { get; set; }
        public int? DailyRepeat { get; set; }
        public int? WeeklyRepeat { get; set; }
        public int? MonthlyRepeat { get; set; }
        public string DailyTime { get; set; }
        public string WeeklyTime { get; set; }
        public string MonthlyTime { get; set; }
        public string Emails { get; set; }
        public bool IsActive { get; set; }
        public bool IsEnglish { get; set; }
        public bool? PDF { get; set; }
        public bool? Excel { get; set; }
        public string GroupUpdatesByType { get; set; }
        public int? GroupUpdatesValue { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public FleetView Fleet { get; set; }
        public WarehouseView Warehouse { get; set; }
        public InventoryView Inventory { get; set; }
        public ReportTypeLookupView ReportTypeLookup { get; set; }
        public DayOfWeekLookupView DaysOfWeekLookup { get; set; }
    }
}
