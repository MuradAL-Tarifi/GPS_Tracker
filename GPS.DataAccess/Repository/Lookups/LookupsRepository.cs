using GPS.DataAccess.Context;
using GPS.Domain.DTO;
using GPS.Domain.Models;
using GPS.Domain.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.DataAccess.Repository.Lookups
{
    public class LookupsRepository : ILookupsRepository
    {
        private readonly TrackerDBContext _dbContext;

        public LookupsRepository(TrackerDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<DeviceType>> GetDeviceTypesAsync()
        {
            var deviceType = await _dbContext.DeviceType.Where(x => !x.IsDeleted).ToListAsync();
            return deviceType;
        }


        public async Task<List<AlertTypeLookup>> GetAlertTypesAsync()
        {
            var alertType = await _dbContext.AlertTypeLookup.OrderByDescending(x => x.RowOrder).ToListAsync();
            return alertType;
        }

        public async Task<AdminDashboardView> GetAdminDasboardAsync()
        {
            var gateways = await _dbContext.Gateway.Include(x => x.Brand).ToListAsync();

            var _tempSensors = await _dbContext.Sensor.Include(x => x.Brand).ToListAsync();
            var sensors = _tempSensors.GroupBy(x => x.Serial).Select(x => x.First()).ToList();

            var fleetsCount = await _dbContext.Fleet.Where(x => !x.IsDeleted).CountAsync();

            var warehouses = await _dbContext.Warehouse.ToListAsync();

            var inventories = await _dbContext.Inventory.ToListAsync();

            int waslLinkedInventoriesCount = inventories.Where(x => x.IsLinkedWithWasl).Count();

            int waslLinkedWarehousesCount = warehouses.Where(x => x.IsLinkedWithWasl).Count();

            var groupedGateways = gateways.GroupBy(x => x.Brand.NameEn).Select(x => new GroupedGateways
            { GatewayBrandName = x.Key, CountPerBrand = x.Count(), PercentageAmongBrands = ((double)x.Count() / (double)gateways.Count()) * 100 }).ToList();
            var groupedSensors = sensors.GroupBy(x => x.Brand.NameEn).Select(x => new GroupedSensors
            { SensorBrandName = x.Key, CountPerBrand = x.Count(), PercentageAmongBrands = ((double)x.Count() / (double)sensors.Count()) * 100 }).ToList();


            int workingSensorsCount = await _dbContext.OnlineInventoryHistory
                    .Where(x => x.GpsDate > DateTime.Now.AddDays(-1)).GroupBy(x => x.Serial).CountAsync();

            var notWorkingSensors = await _dbContext.OnlineInventoryHistory.Where(x => x.GpsDate < DateTime.Now.AddDays(-1))
                .GroupBy(x => x.Serial).CountAsync();


            var dashboard = new AdminDashboardView()
            {
                FleetsCount = fleetsCount,
                UsersCount = await _dbContext.User.Where(x => !x.IsDeleted).CountAsync(),
                WASLOperatingCompaniesCount = await _dbContext.FleetDetails.Where(x => !x.IsDeleted && x.IsLinkedWithWasl).CountAsync(),
                WASLNotLinkedOperatingCompaniesCount = fleetsCount - (await _dbContext.FleetDetails.Where(x => !x.IsDeleted && !x.IsLinkedWithWasl).CountAsync()),
                WaslLinkedInventoriesCount = waslLinkedInventoriesCount,
                WaslLinkedWarehousesCount = waslLinkedWarehousesCount,
                WaslNotLinkedInventoriesCount = (inventories.Count() - waslLinkedInventoriesCount),
                WaslNotLinkedWarehousesCount = (warehouses.Count() - waslLinkedWarehousesCount),
                GatewaysCount = gateways.Count(),
                SensorsCount = sensors.Count(),
                WarehousesCount = warehouses.Count(),
                InventoriesCount = inventories.Count(),
                WorkingSensorsCount = workingSensorsCount,
                NotWorkingSensorsCount = (sensors.Count() - workingSensorsCount),
                LsGroupedGateways = groupedGateways,
                LsGroupedSensors = groupedSensors,
            };

            return dashboard;
        }

        public async Task<AgentDashboardView> GetAgentDasboardAsync(string userId)
        {
            var dashboard = new AgentDashboardView();

            var user = await _dbContext.User.Where(x => x.Id == userId).FirstOrDefaultAsync();
            if (user != null)
            {

                int reportsCount = await _dbContext.ReportSchedule.Where(x => !x.IsDeleted && x.UserId == user.Id &&
                (!user.FleetId.HasValue || x.FleetId == user.FleetId))
                    .CountAsync();

                int customAlertsCount = await _dbContext.CustomAlert.Where(x => !x.IsDeleted &&
                (!user.FleetId.HasValue || x.FleetId == user.FleetId))
                    .CountAsync();

                var warehouses = await _dbContext.Warehouse.Where(x => x.FleetId == user.FleetId).ToListAsync();

                var warehouseIds = warehouses.Select(x => x.Id).ToList();

                var inventories = await _dbContext.Inventory.Where(x => warehouseIds.Contains(x.WarehouseId)).ToListAsync();

                var inventoryIds = inventories.Select(x => x.Id).ToList();

                int waslLinkedInventoriesCount = inventories.Where(x => x.IsLinkedWithWasl).Count();

                int waslLinkedWarehousesCount = warehouses.Where(x => x.IsLinkedWithWasl).Count();

                var gateways = await _dbContext.Inventory.Where(x => warehouseIds.Contains(x.WarehouseId)).Include(x => x.Gateway).ThenInclude(x => x.Brand).Select(x => x.Gateway).ToListAsync();

                var sensors = await _dbContext.InventorySensor.Where(x => inventoryIds.Contains(x.InventoryId))
                    .Include(x => x.Sensor).ThenInclude(x => x.Brand).Select(x => x.Sensor).ToListAsync();

                var groupedGateways = gateways.GroupBy(x => x.Brand.NameEn).Select(x => new GroupedGateways
                { GatewayBrandName = x.Key, CountPerBrand = x.Count(), PercentageAmongBrands = ((double)x.Count() / (double)gateways.Count()) * 100 }).ToList();

                var groupedSensors = sensors.GroupBy(x => x.Brand.NameEn).Select(x => new GroupedSensors
                { SensorBrandName = x.Key, CountPerBrand = x.Count(), PercentageAmongBrands = ((double)x.Count() / sensors.Count()) * 100 }).ToList();


                var sensorsSNs = sensors.Select(x => x.Serial).ToList();

                int workingSensorsCount = await _dbContext.OnlineInventoryHistory
                    .Where(x => sensorsSNs.Contains(x.Serial) && x.GpsDate > DateTime.Now.AddDays(-1)).CountAsync();

                //int notWorkingSensorsCount = await _dbContext.OnlineInventoryHistory
                //    .Where(x => sensorsSNs.Contains(x.Serial) && x.GpsDate < DateTime.Now.AddDays(-1)).CountAsync();

                dashboard = new AgentDashboardView()
                {
                    ReportsCount = reportsCount,
                    CustomAlertsCount = customAlertsCount,
                    GatewaysCount = gateways.Count(),
                    SensorsCount = sensors.Count(),
                    WorkingSensorsCount = workingSensorsCount,
                    NotWorkingSensorsCount = (sensors.Count() - workingSensorsCount),
                    LsGroupedGateways = groupedGateways,
                    LsGroupedSensors = groupedSensors,
                    InventoriesCount = inventoryIds.Count(),
                    WarehousesCount = warehouseIds.Count(),
                    WaslLinkedInventoriesCount = waslLinkedInventoriesCount,
                    WaslLinkedWarehousesCount = waslLinkedWarehousesCount,
                    WaslNotLinkedInventoriesCount = (inventoryIds.Count() - waslLinkedInventoriesCount),
                    WaslNotLinkedWarehousesCount = (warehouseIds.Count() - waslLinkedWarehousesCount)
                };
            }

            return dashboard;
        }


        public async Task<WaslCode> GetWaslResultCode(string code)
        {
            var entity = await _dbContext.WaslCode.Where(x => x.Code.Equals(code)).FirstOrDefaultAsync();
            return entity;
        }


        public async Task<List<AlertTypeLookup>> GetCustomAlertTypesAsync()
        {
            var alertTypes = await _dbContext.AlertTypeLookup.OrderByDescending(x => x.RowOrder).ToListAsync();
            return alertTypes;
        }

        public async Task<List<Gateway>> GetGatewaysAsync()
        {
            var properties = await _dbContext.Gateway.ToListAsync();
            return properties;
        }

        public async Task<List<Warehouse>> GetWarehousesAsync(long? FleetId)
        {
            var warehouses = await _dbContext.Warehouse
                .Where(x => !x.IsDeleted &&
                (!FleetId.HasValue || x.FleetId == FleetId))
                .ToListAsync();

            return warehouses;
        }

        public async Task<List<Inventory>> GetInventoriesAsync(long WarehouseId)
        {
            var inventories = await _dbContext.Inventory
                .Where(x => !x.IsDeleted && x.WarehouseId == WarehouseId)
                .ToListAsync();

            return inventories;
        }

        public async Task<List<InventorySensor>> GetInventorySensorsAsync(long InventoryId)
        {
            var inventorySensors = await _dbContext.InventorySensor
                .Where(x => !x.IsDeleted && x.InventoryId == InventoryId)
                .Include(x => x.Sensor)
                .ToListAsync();

            return inventorySensors;
        }

        public async Task<List<ReportTypeLookup>> GetReportTypesAsync()
        {
            var properties = await _dbContext.ReportTypeLookup.ToListAsync();
            return properties;
        }

        public async Task<List<DayOfWeekLookup>> GetDaysOfWeekAsync()
        {
            var properties = await _dbContext.DayOfWeekLookup.OrderBy(x => x.RowOrder).ToListAsync();
            return properties;
        }

        public async Task<List<Inventory>> GetInventoriesByUserIdAsync(string UserId)
        {
            var warehousesIdByUserId = await _dbContext.UserWarehouse.Where(x => x.UserId == UserId)
                .Select(x => x.WarehouseId).ToListAsync();
            var inventories = await _dbContext.Inventory
               .Where(x => !x.IsDeleted &&  warehousesIdByUserId.Contains(x.WarehouseId))
               .ToListAsync();

            return inventories;
        }

        public async Task<List<User>> GetUsersByFleetIdAsync(long FleetId)
        {
            return await _dbContext.User.Where(x => x.FleetId == FleetId).ToListAsync();
        }

    }
}
