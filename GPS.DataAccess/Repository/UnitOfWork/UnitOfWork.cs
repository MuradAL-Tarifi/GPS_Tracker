using GPS.DataAccess.Context;
using GPS.DataAccess.Repository.Accounts;
using GPS.DataAccess.Repository.Agents;
using GPS.DataAccess.Repository.AlertBySensor;
using GPS.DataAccess.Repository.Alerts;
using GPS.DataAccess.Repository.AlertTracker;
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
using GPS.DataAccess.Repository.Smtpchecker;
using GPS.DataAccess.Repository.SystemSettings;
using GPS.DataAccess.Repository.Users;
using GPS.DataAccess.Repository.Warehouses;
using GPS.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.DataAccess.Repository.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TrackerDBContext _dbContext;
        private readonly AppSettings _appSettings;

        public UnitOfWork(TrackerDBContext dbContext, AppSettings appSettings)
        {
            _dbContext = dbContext;
            _appSettings = appSettings;
        }

        private IAccountRepository _accountRepository;
        public IAccountRepository AccountRepository => _accountRepository = _accountRepository ?? new AccountRepository(_dbContext);

        private IUserRepository _userRepository;
        public IUserRepository UserRepository => _userRepository = _userRepository ?? new UserRepository(_dbContext);

        private IEventLogRepository _eventLogRepository;
        public IEventLogRepository EventLogRepository => _eventLogRepository = _eventLogRepository ?? new EventLogRepository(_dbContext);
        private IFleetRepository _fleetRepository;
        public IFleetRepository FleetRepository => _fleetRepository = _fleetRepository ?? new FleetRepository(_dbContext);

        private IBrandRepository _brandRepository;
        public IBrandRepository BrandRepository => _brandRepository = _brandRepository ?? new BrandRepository(_dbContext);

        private ISensorRepository _sensorRepository;
        public ISensorRepository SensorRepository => _sensorRepository = _sensorRepository ?? new SensorRepository(_dbContext);

        private IAgentRepository _agentRepository;
        public IAgentRepository AgentRepository => _agentRepository = _agentRepository ?? new AgentRepository(_dbContext);

        private ILookupsRepository _lookupsRepository;
        public ILookupsRepository LookupsRepository => _lookupsRepository = _lookupsRepository ?? new LookupsRepository(_dbContext);

        private IInventoryHistoryRepository _inventoryHistoryRepository;

        public IInventoryHistoryRepository InventoryHistoryRepository => _inventoryHistoryRepository = _inventoryHistoryRepository ?? new InventoryHistoryRepository(_appSettings);

        private IInventoryRepository _inventoryRepository;
        public IInventoryRepository InventoryRepository => _inventoryRepository = _inventoryRepository ?? new InventoryRepository(_dbContext);
        private IInventorySensorRepository _inventorySensorRepository;
        public IInventorySensorRepository InventorySensorRepository => _inventorySensorRepository = _inventorySensorRepository ?? new InventorySensorRepository(_dbContext);

        private IWarehouseRepository _warehouseRepository;
        public IWarehouseRepository WarehouseRepository => _warehouseRepository = _warehouseRepository ?? new WarehouseRepository(_dbContext);

        private IGatewayRepository _gatewayRepository;
        public IGatewayRepository GatewayRepository => _gatewayRepository = _gatewayRepository ?? new GatewayRepository(_dbContext);

        private IWaslIntegrationLogRepository _waslIntegrationLogRepository;
        public IWaslIntegrationLogRepository WaslIntegrationLogRepository => _waslIntegrationLogRepository = _waslIntegrationLogRepository ?? new WaslIntegrationLogRepository(_dbContext);

        private IOnlineInventoryHistoryRepository _onlineInventoryHistoryRepository;
        public IOnlineInventoryHistoryRepository OnlineInventoryHistoryRepository => _onlineInventoryHistoryRepository = _onlineInventoryHistoryRepository ?? new OnlineInventoryHistoryRepository(_dbContext, _appSettings);

        private ISensorAlertRepository _sensorAlertRepository;
        public ISensorAlertRepository SensorAlertRepository => _sensorAlertRepository = _sensorAlertRepository ?? new SensorAlertRepository(_dbContext);

        private IReportScheduleRepository _reportScheduleRepository;
        public IReportScheduleRepository ReportScheduleRepository => _reportScheduleRepository = _reportScheduleRepository ?? new ReportScheduleRepository(_dbContext);

        private IAlertTrackerRepository _AlertTrackerRepository;
        public IAlertTrackerRepository AlertTrackerRepository => _AlertTrackerRepository = _AlertTrackerRepository ?? new AlertTrackerRepository(_dbContext);

        private ICustomAlertRepository _customAlertRepository;
        public ICustomAlertRepository CustomAlertRepository => _customAlertRepository = _customAlertRepository ?? new CustomAlertRepository(_dbContext);

        private IEmailHistoryRepository _emailHistoryRepository;
        public IEmailHistoryRepository EmailHistoryRepository => _emailHistoryRepository = _emailHistoryRepository ?? new EmailHistoryRepository(_dbContext);

        private IAlertsRepository _alertsRepository;
        public IAlertsRepository AlertsRepository => _alertsRepository = _alertsRepository ?? new AlertsRepository(_dbContext);

        private IInventoryCustomAlertsWatcherRepository _inventoryCustomAlertsWatcherRepository;
        public IInventoryCustomAlertsWatcherRepository InventoryCustomAlertsWatcherRepository => _inventoryCustomAlertsWatcherRepository = _inventoryCustomAlertsWatcherRepository ?? new InventoryCustomAlertsWatcherRepository(_dbContext);
        
        private ISystemSettingRepository _systemSettingRepository;
        public ISystemSettingRepository SystemSettingRepository => _systemSettingRepository = _systemSettingRepository ?? new SystemSettingRepository(_dbContext);
        private IAlertBySensorRepository _alertBySensorRepository;
        public IAlertBySensorRepository AlertBySensorRepository => _alertBySensorRepository = _alertBySensorRepository ?? new AlertBySensorRepository(_dbContext);
        private ISmtpcheckerRepository _smtpcheckerRepository;
        public ISmtpcheckerRepository SmtpcheckerRepository => _smtpcheckerRepository = _smtpcheckerRepository ?? new SmtpcheckerRepository(_dbContext);
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
