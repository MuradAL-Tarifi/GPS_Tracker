using GPS.Domain.DTO;
using GPS.Domain.ViewModels;
using GPS.Domain.Views;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Services.Lookups
{
    public interface ILookupsService
    {

        /// <summary>
        /// Get All Alert Types
        /// </summary>
        Task<ReturnResult<List<AlertTypeLookupView>>> GetAlertTypesAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<ReturnResult<AdminDashboardView>> GetAdminDasboardAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<ReturnResult<AgentDashboardView>> GetAgentDasboardAsync(string userId);

        /// <summary>
        /// Get Wasl Result Code or Rejection Reasons
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        Task<WaslCodeView> GetWaslResultCode(string code);

        Task<ReturnResult<List<AlertTypeLookupView>>> GetCustomAlertTypesAsync();

        Task<ReturnResult<List<GatewayView>>> GetGatewaysAsync();

        Task<ReturnResult<List<WarehouseView>>> GetWarehousesAsync(long? FleetId);

        Task<ReturnResult<List<InventoryView>>> GetInventoriesAsync(long WarehouseId);

        Task<ReturnResult<List<InventorySensorView>>> GetInventorySensorsAsync(long InventoryId);

        /// <summary>
        /// Get Report Types
        /// </summary>
        /// <returns></returns>
        Task<ReturnResult<List<ReportTypeLookupView>>> GetReportTypesAsync();

        /// <summary>
        /// Get Days Of Week
        /// </summary>
        /// <returns></returns>
        Task<ReturnResult<List<DayOfWeekLookupView>>> GetDaysOfWeekAsync();

        Task<ReturnResult<List<InventoryView>>> GetInventoriesByUserIdAsync(string UserId);

        Task<ReturnResult<List<UserView>>> GetUsersAsync(long FleetId);

        Task<ReturnResult<byte[]>> GetDefaultExcelFileToAddNewSensorsAsync();

        ReturnResult<List<SensorView>> ImportSensorsFromExcelAsync(IFormFile file);

        Task<ReturnResult<List<WarehouseView>>> GetWarehousesAndInventoriesAsync(long? FleetId);

        Task<ReturnResult<List<WarehouseView>>> GetWarehousesByUserIdAsync(string UserId);
        Task<ReturnResult<List<WarehouseView>>> GetWarehousesAndInventoriesAsyncAndSensors(long? FleetId);
    }
}
