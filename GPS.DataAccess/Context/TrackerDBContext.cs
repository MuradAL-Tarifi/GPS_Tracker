using GPS.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.DataAccess.Context
{
    public partial class TrackerDBContext : DbContext
    {
        public TrackerDBContext(){}

        public TrackerDBContext(DbContextOptions<TrackerDBContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Agent>().HasQueryFilter(p => !p.IsDeleted);
            builder.Entity<DeviceType>().HasQueryFilter(p => !p.IsDeleted);
            builder.Entity<Fleet>().HasQueryFilter(p => !p.IsDeleted);
            builder.Entity<FleetDetails>().HasQueryFilter(p => !p.IsDeleted);
            builder.Entity<Brand>().HasQueryFilter(p => !p.IsDeleted);
            builder.Entity<Sensor>().HasQueryFilter(p => !p.IsDeleted);
            builder.Entity<PrivilegeType>().HasQueryFilter(p => !p.IsDeleted);
            builder.Entity<RegisterType>().HasQueryFilter(p => !p.IsDeleted);
            builder.Entity<User>().HasQueryFilter(p => !p.IsDeleted);
            builder.Entity<CustomAlert>().HasQueryFilter(p => !p.IsDeleted);
            builder.Entity<Gateway>().HasQueryFilter(p => !p.IsDeleted);
            builder.Entity<Warehouse>().HasQueryFilter(p => !p.IsDeleted);
            builder.Entity<Inventory>().HasQueryFilter(p => !p.IsDeleted);
            builder.Entity<InventorySensor>().HasQueryFilter(p => !p.IsDeleted);
            builder.Entity<ReportTypeLookup>().HasQueryFilter(p => !p.IsDeleted);
            builder.Entity<AlertTypeLookup>().HasQueryFilter(p => !p.IsDeleted);
            base.OnModelCreating(builder);
        }
        public virtual DbSet<AlertBySensor> AlertBySensor { get; set; }
        public virtual DbSet<AlertTracker> AlertTracker { get; set; }
        public virtual DbSet<Smtpchecker> Smtpchecker { get; set; }
        public virtual DbSet<Smtpsetting> Smtpsetting { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<UserRole> UserRole { get; set; }
        public virtual DbSet<Agent> Agent { get; set; }
        public virtual DbSet<DeviceType> DeviceType { get; set; }
        public virtual DbSet<Fleet> Fleet { get; set; }
        public virtual DbSet<FleetDetails> FleetDetails { get; set; }
        public virtual DbSet<Brand> Brand { get; set; }
        public virtual DbSet<Sensor> Sensor { get; set; }
        public virtual DbSet<RegisterType> RegisterType { get; set; }
        public virtual DbSet<AlertTypeLookup> AlertTypeLookup { get; set; }
        public virtual DbSet<PrivilegeType> PrivilegeType { get; set; }
        public virtual DbSet<UserPrivilege> UserPrivilege { get; set; }
        public virtual DbSet<EventLog> EventLog { get; set; }
        public virtual DbSet<WaslCode> WaslCode { get; set; }
        public virtual DbSet<ReportSchedule> ReportSchedule { get; set; }
        public virtual DbSet<CustomAlert> CustomAlert { get; set; }
        public virtual DbSet<InventoryCustomAlert> InventoryCustomAlert { get; set; }
        public virtual DbSet<EmailHistory> EmailHistory { get; set; }
        public virtual DbSet<Gateway> Gateway { get; set; }
        public virtual DbSet<Warehouse> Warehouse { get; set; }
        public virtual DbSet<Inventory> Inventory { get; set; }
        public virtual DbSet<InventorySensor> InventorySensor { get; set; }
        public virtual DbSet<Alert> Alert { get; set; }
        public virtual DbSet<ReportTypeLookup> ReportTypeLookup { get; set; }
        public virtual DbSet<DayOfWeekLookup> DayOfWeekLookup { get; set; }
        public virtual DbSet<ScheduleTypeLookup> ScheduleTypeLookup { get; set; }
        public virtual DbSet<ReportScheduleHistory> ReportScheduleHistory { get; set; }
        public virtual DbSet<SensorAlertTypeLookup> SensorAlertTypeLookup { get; set; }
        //public virtual DbSet<SensorAlert> SensorAlert { get; set; }
        public virtual DbSet<OnlineInventoryHistory> OnlineInventoryHistory { get; set; }
        public virtual DbSet<WaslIntegrationLogTypeLookup> WaslIntegrationLogTypeLookup { get; set; }
        public virtual DbSet<WaslIntegrationLog> WaslIntegrationLog { get; set; }
        public virtual DbSet<UserWarehouse> UserWarehouse { get; set; }
        public virtual DbSet<UserInventory> UserInventory { get; set; }
        public virtual DbSet<SensorAlertHisotry> SensorAlertHisotry { get; set; }
        public virtual DbSet<CustomAlertWatcher> CustomAlertWatcher { get; set; }
        public virtual DbSet<SystemSetting> SystemSetting { get; set; }
        public virtual DbSet<AlertByUserWatcher> AlertByUserWatcher { get; set; }
    }
}
