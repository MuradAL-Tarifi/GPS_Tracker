using AutoMapper;
using GPS.Domain.Models;
using GPS.Domain.ViewModels;
using GPS.Domain.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.DataAccess.AutoMapper
{
    public interface IAutoMapperConfig
    {
        public interface IAutoMapperConfig
        {
            IMapper CreateMapper();
        }

        public class AutoMapperConfig : Profile
        {
            public AutoMapperConfig()
            {
                CreateMap<Agent, AgentView>();
                CreateMap<Fleet, FleetView>();
                CreateMap<FleetView, Fleet>();
                CreateMap<Fleet, FleetDetailsView>();
                CreateMap<FleetDetailsView, Fleet>();
                CreateMap<FleetDetails, FleetDetailsView>();
                CreateMap<FleetDetailsView, FleetDetails>();
                CreateMap<Brand, BrandView>();
                CreateMap<BrandView, Brand>();
                CreateMap<Sensor, SensorView>();
                CreateMap<SensorView, Sensor>();

                CreateMap<Gateway, GatewayView>();
                CreateMap<GatewayView, Gateway>();

                CreateMap<InventorySensor, InventorySensorView>();
                CreateMap<InventorySensorView, InventorySensor>();

                CreateMap<OnlineInventoryHistory, OnlineInventoryHistoryView>();
                CreateMap<OnlineInventoryHistoryView, OnlineInventoryHistory>();

                CreateMap<Warehouse, WarehouseView>();
                CreateMap<WarehouseView, Warehouse>();

                CreateMap<Inventory, InventoryView>();
                CreateMap<InventoryView, Inventory>();
                CreateMap<OnlineInventoryHistory, OnlineInventoryHistoryView>();
                CreateMap<OnlineInventoryHistoryView, OnlineInventoryHistory>();
                CreateMap<InventoryHistory, InventoryHistoryView>();
                CreateMap<InventoryHistoryView, InventoryHistory>();
                CreateMap<InventorySensor, InventorySensorView>().ForMember(s => s.SensorView, map => map.MapFrom(s => s.Sensor));
                CreateMap<InventorySensorView, InventorySensor>();

                CreateMap<ReportSchedule, ReportScheduleView>();
                CreateMap<ReportScheduleView, ReportSchedule>();
                CreateMap<CustomAlert, CustomAlertView>();
                CreateMap<CustomAlertView, CustomAlert>();
                CreateMap<EventLogView, EventLog>();
                CreateMap<EventLog, EventLogView>();
                CreateMap<Alert, AlertView>();
                CreateMap<AlertView, Alert>();

                // user, role privilegeType
                CreateMap<User, UserView>();
                CreateMap<UserView, User>();
                CreateMap<Role, RoleView>();
                CreateMap<PrivilegeType, PrivilegeTypeView>();
                CreateMap<UserPrivilege, UserPrivilegeView>();
                CreateMap<SystemSetting, SystemSettingView>();
                CreateMap<SystemSettingView, SystemSetting>();
                CreateMap<EmailHistoryView, EmailHistory>();
                CreateMap<EmailHistory, EmailHistoryView>();
                

                CreateMap<ReportTypeLookup, ReportTypeLookupView>();

                CreateMap<ReportOptionsModel, ReportScheduleView>()
                .ForMember(d => d.Id, map => map.MapFrom(s => s.ReportScheduleId))
                .ForMember(d => d.ReportTypeLookupId, map => map.MapFrom(s => s.ReportTypeId))
                .ForMember(d => d.SensorSerial, map => map.MapFrom(s => s.SensorSerial))
                .ForMember(d => d.AlertTypeLookupId, map => map.MapFrom(s => s.AlertTypeLookupId))
                .ForMember(d => d.Daily, map => map.MapFrom(s => s.Scheduling.Daily))
                .ForMember(d => d.Weekly, map => map.MapFrom(s => s.Scheduling.Weekly))
                .ForMember(d => d.Monthly, map => map.MapFrom(s => s.Scheduling.Monthly))
                .ForMember(d => d.Yearly, map => map.MapFrom(s => s.Scheduling.Yearly))
                .ForMember(d => d.DayOfWeekId, map => map.MapFrom(s => s.Scheduling.DayOfWeekId))
                .ForMember(d => d.DayOfMonthId, map => map.MapFrom(s => s.Scheduling.DayOfMonthId))
                .ForMember(d => d.DailyRepeat, map => map.MapFrom(s => s.Scheduling.DailyRepeat))
                .ForMember(d => d.WeeklyRepeat, map => map.MapFrom(s => s.Scheduling.WeeklyRepeat))
                .ForMember(d => d.MonthlyRepeat, map => map.MapFrom(s => s.Scheduling.MonthlyRepeat))
                .ForMember(d => d.DailyTime, map => map.MapFrom(s => s.Scheduling.DailyTime))
                .ForMember(d => d.WeeklyTime, map => map.MapFrom(s => s.Scheduling.WeeklyTime))
                .ForMember(d => d.MonthlyTime, map => map.MapFrom(s => s.Scheduling.MonthlyTime))
                .ForMember(d => d.Emails, map => map.MapFrom(s => s.Scheduling.Emails))
                .ForMember(d => d.PDF, map => map.MapFrom(s => s.Scheduling.PDF))
                .ForMember(d => d.Excel, map => map.MapFrom(s => s.Scheduling.Excel));


                CreateMap<ReportSchedule, ReportOptionsModel>()
                .ForMember(d => d.ReportScheduleId, map => map.MapFrom(s => s.Id))
                .ForMember(d => d.ReportTypeId, map => map.MapFrom(s => s.ReportTypeLookupId))
                .ForMember(d => d.SensorSerial, map => map.MapFrom(s => s.SensorSerial))
                .ForMember(d => d.AlertTypeLookupId, map => map.MapFrom(s => s.AlertTypeLookupId))
                .ForPath(d => d.Scheduling.Daily, map => map.MapFrom(s => s.Daily))
                .ForPath(d => d.Scheduling.Weekly, map => map.MapFrom(s => s.Weekly))
                .ForPath(d => d.Scheduling.Monthly, map => map.MapFrom(s => s.Monthly))
                .ForPath(d => d.Scheduling.Yearly, map => map.MapFrom(s => s.Yearly))
                .ForPath(d => d.Scheduling.DayOfWeekId, map => map.MapFrom(s => s.DayOfWeekId))
                .ForPath(d => d.Scheduling.DayOfMonthId, map => map.MapFrom(s => s.DayOfMonthId))
                .ForPath(d => d.Scheduling.DailyRepeat, map => map.MapFrom(s => s.DailyRepeat))
                .ForPath(d => d.Scheduling.WeeklyRepeat, map => map.MapFrom(s => s.WeeklyRepeat))
                .ForPath(d => d.Scheduling.MonthlyRepeat, map => map.MapFrom(s => s.MonthlyRepeat))
                .ForPath(d => d.Scheduling.DailyTime, map => map.MapFrom(s => s.DailyTime))
                .ForPath(d => d.Scheduling.WeeklyTime, map => map.MapFrom(s => s.WeeklyTime))
                .ForPath(d => d.Scheduling.MonthlyTime, map => map.MapFrom(s => s.MonthlyTime))
                .ForPath(d => d.Scheduling.Emails, map => map.MapFrom(s => s.Emails))
                .ForPath(d => d.Scheduling.PDF, map => map.MapFrom(s => s.PDF))
                .ForPath(d => d.Scheduling.Excel, map => map.MapFrom(s => s.Excel));


                CreateMap<ReportSchedule, ReportOptionsDetailsModel>()
                .ForMember(d => d.ReportScheduleId, map => map.MapFrom(s => s.Id))
                .ForMember(d => d.ReportTypeId, map => map.MapFrom(s => s.ReportTypeLookupId))
                .ForMember(d => d.ReportTypeName, map => map.MapFrom(s => s.ReportTypeLookup.Name))
                .ForMember(d => d.WarehouseName, map => map.MapFrom(s => s.Warehouse.Name))
                .ForMember(d => d.InventoryName, map => map.MapFrom(s => s.Inventory.Name))
                .ForMember(d => d.GroupUpdatesByType, map => map.MapFrom(s => s.GroupUpdatesByType))
                .ForMember(d => d.GroupUpdatesValue, map => map.MapFrom(s => s.GroupUpdatesValue))
                .ForPath(d => d.Scheduling.Daily, map => map.MapFrom(s => s.Daily))
                .ForPath(d => d.Scheduling.Weekly, map => map.MapFrom(s => s.Weekly))
                .ForPath(d => d.Scheduling.Monthly, map => map.MapFrom(s => s.Monthly))
                .ForPath(d => d.Scheduling.Yearly, map => map.MapFrom(s => s.Yearly))
                .ForPath(d => d.Scheduling.DayOfWeekId, map => map.MapFrom(s => s.DayOfWeekId))
                .ForPath(d => d.Scheduling.DayOfMonthId, map => map.MapFrom(s => s.DayOfMonthId))
                .ForPath(d => d.Scheduling.DailyRepeat, map => map.MapFrom(s => s.DailyRepeat))
                .ForPath(d => d.Scheduling.WeeklyRepeat, map => map.MapFrom(s => s.WeeklyRepeat))
                .ForPath(d => d.Scheduling.MonthlyRepeat, map => map.MapFrom(s => s.MonthlyRepeat))
                .ForPath(d => d.Scheduling.DailyTime, map => map.MapFrom(s => s.DailyTime))
                .ForPath(d => d.Scheduling.WeeklyTime, map => map.MapFrom(s => s.WeeklyTime))
                .ForPath(d => d.Scheduling.MonthlyTime, map => map.MapFrom(s => s.MonthlyTime))
                .ForPath(d => d.Scheduling.Emails, map => map.MapFrom(s => s.Emails))
                .ForPath(d => d.Scheduling.PDF, map => map.MapFrom(s => s.PDF))
                .ForPath(d => d.Scheduling.Excel, map => map.MapFrom(s => s.Excel));

                CreateMap<DayOfWeekLookup, DayOfWeekLookupView>();

                CreateMap<ScheduleTypeLookup, ScheduleTypeLookupView>();

                CreateMap<ReportScheduleHistory, ReportScheduleHistoryView>();
                CreateMap<ReportScheduleHistoryView, ReportScheduleHistory>();


                CreateMap<FleetDetails, FleetWaslModel>()
                .ForMember(d => d.FleetDetailsId, map => map.MapFrom(s => s.Id))
                .ForMember(d => d.IdentityNumber, map => map.MapFrom(s => s.IdentityNumber))
                .ForMember(d => d.DateOfBirthHijri, map => map.MapFrom(s => s.DateOfBirthHijri))
                .ForMember(d => d.CommercialRecordNumber, map => map.MapFrom(s => s.CommercialRecordNumber))
                .ForMember(d => d.CommercialRecordIssueDateHijri, map => map.MapFrom(s => s.CommercialRecordIssueDateHijri))
                .ForMember(d => d.PhoneNumber, map => map.MapFrom(s => s.PhoneNumber))
                .ForMember(d => d.ExtensionNumber, map => map.MapFrom(s => s.ExtensionNumber))
                .ForMember(d => d.EmailAddress, map => map.MapFrom(s => s.EmailAddress))
                .ForMember(d => d.ManagerName, map => map.MapFrom(s => s.ManagerName))
                .ForMember(d => d.ManagerPhoneNumber, map => map.MapFrom(s => s.ManagerPhoneNumber))
                .ForMember(d => d.ManagerMobileNumber, map => map.MapFrom(s => s.ManagerMobileNumber))
                .ForMember(d => d.CreatedBy, map => map.MapFrom(s => s.CreatedBy))
                .ForMember(d => d.UpdatedBy, map => map.MapFrom(s => s.UpdatedBy))
                .ForMember(d => d.ReferanceNumber, map => map.MapFrom(s => s.ReferanceNumber))
                .ForMember(d => d.ActivityType, map => map.MapFrom(s => s.ActivityType))
                .ForMember(d => d.SFDACompanyActivities, map => map.MapFrom(s => s.SFDACompanyActivities))
                .ForMember(d => d.IsLinkedWithWasl, map => map.MapFrom(s => s.IsLinkedWithWasl))
                .ForMember(d => d.FleetType, map => map.MapFrom(s => s.FleetType));

                CreateMap<FleetWaslModel, FleetDetails>()
                .ForMember(x => x.Id, map => map.Ignore());

            }
        }
    }
}
