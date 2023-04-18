using GPS.Domain.Models;
using GPS.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.DataAccess.Repository.Lookups
{
    public interface ILookupsRepository
    {

        Task<List<AlertTypeLookup>> GetAlertTypesAsync();

        Task<AdminDashboardView> GetAdminDasboardAsync();

        Task<AgentDashboardView> GetAgentDasboardAsync(string userId);

        Task<WaslCode> GetWaslResultCode(string code);

        Task<List<AlertTypeLookup>> GetCustomAlertTypesAsync();

        Task<List<Warehouse>> GetWarehousesAsync(long? FleetId);

        Task<List<Gateway>> GetGatewaysAsync();

        Task<List<Inventory>> GetInventoriesAsync(long WarehouseId);

        Task<List<InventorySensor>> GetInventorySensorsAsync(long InventoryId);

        Task<List<ReportTypeLookup>> GetReportTypesAsync();

        Task<List<DayOfWeekLookup>> GetDaysOfWeekAsync();

        Task<List<Inventory>> GetInventoriesByUserIdAsync(string UserId);

        Task<List<User>> GetUsersByFleetIdAsync(long FleetId);
    }
}
