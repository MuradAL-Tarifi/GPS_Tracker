using GPS.DataAccess.Repository.Accounts;
using GPS.DataAccess.Repository.Agents;
using GPS.DataAccess.Repository.Alerts;
using GPS.DataAccess.Repository.Brands;
using GPS.DataAccess.Repository.CustomAlerts;
using GPS.DataAccess.Repository.EmailHistorys;
using GPS.DataAccess.Repository.EventLogs;
using GPS.DataAccess.Repository.Fleets;
using GPS.DataAccess.Repository.Gateways;
using GPS.DataAccess.Repository.Inventorys;
using GPS.DataAccess.Repository.Lookups;
using GPS.DataAccess.Repository.ReportsSchedule;
using GPS.DataAccess.Repository.SensorAlerts;
using GPS.DataAccess.Repository.Sensors;
using GPS.DataAccess.Repository.SystemSettings;
using GPS.DataAccess.Repository.Users;
using GPS.DataAccess.Repository.Warehouses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.DataAccess.Repository.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IAccountRepository AccountRepository { get; }
        IUserRepository UserRepository { get; }
        IEventLogRepository EventLogRepository { get; }
        IFleetRepository FleetRepository { get; }
        IBrandRepository BrandRepository { get; }
        ISensorRepository SensorRepository { get; }
        IAgentRepository AgentRepository { get; }
        ILookupsRepository LookupsRepository { get; }
        IInventoryHistoryRepository InventoryHistoryRepository { get; }
        IInventoryRepository InventoryRepository { get; }
        IInventorySensorRepository InventorySensorRepository { get; }
        IGatewayRepository GatewayRepository { get; }
        IWarehouseRepository WarehouseRepository { get; }
        IWaslIntegrationLogRepository WaslIntegrationLogRepository { get; }
        IOnlineInventoryHistoryRepository OnlineInventoryHistoryRepository { get; }
        ISensorAlertRepository SensorAlertRepository { get; }
        IReportScheduleRepository ReportScheduleRepository { get; }
        ICustomAlertRepository CustomAlertRepository { get; }
        IEmailHistoryRepository EmailHistoryRepository { get; }
        IAlertsRepository AlertsRepository { get; }
        IInventoryCustomAlertsWatcherRepository InventoryCustomAlertsWatcherRepository { get; }
        ISystemSettingRepository SystemSettingRepository { get; }
        
    }
}
