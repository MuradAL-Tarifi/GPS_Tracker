using GPS.Domain.DTO;
using GPS.Domain.ViewModels;
using GPS.Domain.Views;
using GPS.Services.Agents;
using GPS.Services.Alerts;
using GPS.Services.Brands;
using GPS.Services.Fleets;
using GPS.Services.Lookups;
using GPS.Services.Sensors;
using GPS.Services.SystemSettings;
using GPS.Services.Users;
using GPS.Services.WareHouses;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GPS.Web.Agent.AppCode.Helpers
{
    public interface IViewHelper
    {
        /// <summary>
        /// Get Default Page sizes
        /// </summary>
        /// <returns>10, 25, 50, 100, 500</returns>
        SelectList GetPageSizes();

        /// <summary>
        /// 10, 25, 50, 100, 500 
        /// </summary>
        /// <returns></returns>
        SelectList GetPageSmallSizes();
        /// <summary>
        /// Get Agents SelectList, select Agent by AgentId if not null
        /// </summary>
        Task<SelectList> GetAgents(int? AgentId = null);

        /// <summary>
        /// Get Fleets SelectList by AgentId, select Fleet by FleetId if not null
        /// </summary>
        Task<SelectList> GetFleets(int? AgentId = null, long? FleetId = null, bool selectFleetId = false);

        /// <summary>
        /// Get FleetsWASL SelectList by AgentId, select Fleet by FleetId if not null
        /// </summary>
        Task<SelectList> GetFleetsWASL(int? AgentId = null, long? FleetId = null, bool selectFleetId = false);

        /// <summary>
        /// Get Sensors SelectList, select Sensors by SensorId if not null
        /// </summary>
        Task<SelectList> GetSensors(int? BrandId = null, long? SensorId = null, bool selectSensorId = false);

        /// <summary>
        /// Get Brands SelectList, select Brands by BandId if not null
        /// </summary>
        Task<SelectList> GetBrand(int? brandId = null);



        /// <summary>
        /// Get ErrorView Name by status Code
        /// </summary>
        /// <param name="statusCode"></param>
        /// <returns>ErrorView Name</returns>
        string GetErrorPage(HttpCode statusCode);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        Task<SelectList> GetRoles(string Name);

        Task<SelectList> GetGateways(int? Id);

        Task<SelectList> GetWarehouses(long? FleetId, long? Id, List<long> PermittedWarehouseIds);

        Task<SelectList> GetInventories(long WarehouseId, long? Id, List<long> PermittedInventoryIds);
        Task<SelectList> GetWarehousesByUserId(string UserId, long? WarehouseId = null);
        Task<SelectList> GetInventorySensorsBySerial(long InventoryId, string sensorSerial);
        Task<SelectList> GetInventorySensorsBySensorId(long InventoryId, long? sensorId);

        /// <summary>
        /// Get Report Type Lookup
        /// </summary>
        /// <returns></returns>
        Task<SelectList> GetReportTypeLookup();

        /// <summary>
        /// Get Days Of Week Lookup
        /// </summary>
        /// <returns></returns>
        Task<SelectList> GetDaysOfWeekLookup();

        /// <summary>
        /// Get Alert Types
        /// </summary>
        /// <returns></returns>
        Task<SelectList> GetAlertTypes();

        Task<SelectList> GetInventoriesByUserId(string UserId, long? Id);

        Task<SystemSettingView> GetSystemSettings();

        Task<SelectList> GetUsersByFleetId(long fleetId);

    }

    public class ViewHelper : IViewHelper
    {
        private readonly ICultureHelper cultureHelper;
        private readonly IAgentService agentService;
        private readonly IFleetService fleetService;
        private readonly IBrandService brandService;
        private readonly ISensorService sensorService;
        private readonly ILookupsService lookupsService;
        private readonly IUserService userService;
        private readonly ISystemSettingService systemSettingService;
        public ViewHelper(ICultureHelper _cultureHelper,
            IFleetService _fleetService,
            IAgentService _agentService,
            ILookupsService _lookupsService,
            IBrandService _brandService,
            ISensorService _sensorService,
            IUserService _userService,
            ISystemSettingService _systemSettingService)
        {
            cultureHelper = _cultureHelper;
            fleetService = _fleetService;
            sensorService = _sensorService;
            brandService = _brandService;
            agentService = _agentService;
            lookupsService = _lookupsService;
            userService = _userService;
            systemSettingService = _systemSettingService;
        }
        public SelectList GetPageSizes()
        {
            return new SelectList(new[] { 10, 25, 50, 100, 500 });
        }
        public SelectList GetPageSmallSizes()
        {
            return new SelectList(new[] { 10, 25, 50, 100, 500 });
        }
        public async Task<SelectList> GetAgents(int? AgentId = null)
        {
            var result = await agentService.GetAllAsync();
            if (!AgentId.HasValue && result.Data.Count > 0)
            {
                AgentId = result.Data[0].Id;
            }
            return result.IsSuccess ? new SelectList(result.Data, "Id", cultureHelper.GetLocalizedName("Name", "NameEn"), AgentId) : null;
        }

        public async Task<SelectList> GetFleets(int? AgentId = null, long? FleetId = null, bool selectFleetId = false)
        {
            var result = await fleetService.GetAllAsync(AgentId);
            if (selectFleetId && !FleetId.HasValue && result.Data.Count > 0)
            {
                FleetId = result.Data[0].Id;
            }
            return result.IsSuccess ? new SelectList(result.Data, "Id", cultureHelper.GetLocalizedName("Name", "NameEn"), FleetId) : null;
        }
        public async Task<SelectList> GetBrand(int? BrandId = null)
        {
            var result = await brandService.GetAllAsync();
            if (!BrandId.HasValue && result.Data.Count > 0)
            {
                BrandId = result.Data[0].Id;
            }
            return result.IsSuccess ? new SelectList(result.Data, "Id", cultureHelper.GetLocalizedName("Name", "NameEn"), BrandId) : null;
        }

        public async Task<SelectList> GetFleetsWASL(int? AgentId = null, long? FleetId = null, bool selectFleetId = false)
        {
            var result = await fleetService.GetFleetsWASLAsync(AgentId);
            if (selectFleetId && !FleetId.HasValue && result.Data.Count > 0)
            {
                FleetId = result.Data[0].Id;
            }
            return result.IsSuccess ? new SelectList(result.Data, "Id", cultureHelper.GetLocalizedName("Name", "NameEn"), FleetId) : null;
        }

        public async Task<SelectList> GetSensors(int? BrandId = null, long? SensorId = null, bool selectSensorId = false)
        {
            var result = await sensorService.GetAllAsync(BrandId);
            if (selectSensorId && !BrandId.HasValue && result.Data.Count > 0)
            {
                SensorId = result.Data[0].Id;
            }
            return result.IsSuccess ? new SelectList(result.Data, "Id", cultureHelper.GetLocalizedName("Name", "NameEn"), SensorId) : null;

        }

        public async Task<SelectList> GetRoles(string Role)
        {
            var result = await userService.GetRolesAsync();
            return result.IsSuccess ? new SelectList(result.Data, "Name", cultureHelper.GetLocalizedName("DisplayName", "DisplayNameEn"), Role) : null;
        }


        public async Task<SelectList> GetGateways(int? Id)
        {
            var result = await lookupsService.GetGatewaysAsync();
            return result.IsSuccess ? new SelectList(result.Data, "Id", cultureHelper.GetLocalizedName("Name", "NameEn"), Id) : null;
        }

        public async Task<SelectList> GetWarehouses(long? FleetId, long? Id, List<long> PermittedWarehouseIds)
        {
            List<WarehouseView> deleteWarehouses = new List<WarehouseView>();
            var result = await lookupsService.GetWarehousesAsync(FleetId);
            foreach (var warehouse in result.Data)
            {
                if (!PermittedWarehouseIds.Any(x => x == warehouse.Id))
                {
                    deleteWarehouses.Add(warehouse);
                }
            }
            foreach (var deleteWarehouse in deleteWarehouses)
            {
                result.Data.Remove(deleteWarehouse);
            }
            return result.IsSuccess ? new SelectList(result.Data, "Id", "Name", Id) : null;
        }
        public async Task<SelectList> GetWarehousesByUserId(string userId, long? warehouseId)
        {
            var result = await lookupsService.GetWarehousesByUserIdAsync(userId);
            return result.IsSuccess ? new SelectList(result.Data, "Id", "Name", warehouseId) : null;
        }
        public async Task<SelectList> GetInventories(long WarehouseId, long? Id, List<long> PermittedInventoryIds)
        {
            List<InventoryView> deleteInventories = new List<InventoryView>();
            var result = await lookupsService.GetInventoriesAsync(WarehouseId);
            foreach (var inventory in result.Data)
            {
                if (!PermittedInventoryIds.Any(x => x == inventory.Id))
                {
                    deleteInventories.Add(inventory);
                }
            }
            foreach(var deleteInventory in deleteInventories)
            {
                result.Data.Remove(deleteInventory);
            }
            return result.IsSuccess ? new SelectList(result.Data, "Id", "Name", Id) : null;
        }

        public async Task<SelectList> GetInventorySensorsBySerial(long InventoryId, string sensorSerial)
        {
            var result = await lookupsService.GetInventorySensorsAsync(InventoryId);
            if (result.IsSuccess)
            {
                if (!string.IsNullOrEmpty(sensorSerial))
                    return new SelectList(result.Data.Select(x => x.SensorView).ToList(), "Serial", "Name", sensorSerial.ToLower());
                else
                    return new SelectList(result.Data.Select(x => x.SensorView).ToList(), "Serial", "Name");
            }
            return null;
        }
        public async Task<SelectList> GetInventorySensorsBySensorId(long InventoryId, long? sensorId)
        {
            var result = await lookupsService.GetInventorySensorsAsync(InventoryId);
            return result.IsSuccess ? new SelectList(result.Data.Select(x => x.SensorView).ToList(), "Id", "Name", sensorId) : null;
        }
        
        public async Task<SelectList> GetReportTypeLookup()
        {
            var result = await lookupsService.GetReportTypesAsync();
            return result.IsSuccess ? new SelectList(result.Data, "Id", cultureHelper.GetLocalizedName("Name", "NameEn")) : null;
        }
        public async Task<SelectList> GetDaysOfWeekLookup()
        {
            var result = await lookupsService.GetDaysOfWeekAsync();
            return result.IsSuccess ? new SelectList(result.Data, "Id", cultureHelper.GetLocalizedName("Name", "NameEn")) : null;
        }

        public async Task<SelectList> GetAlertTypes()
        {
            var result = await lookupsService.GetAlertTypesAsync();
            return result.IsSuccess ? new SelectList(result.Data, "Id", cultureHelper.GetLocalizedName("Name", "NameEn")) : null;
        }
        public string GetErrorPage(Domain.DTO.HttpCode statusCode)
        {
            switch (statusCode)
            {
                case HttpCode.BadRequest:
                    return "NotFound";
                case HttpCode.Unauthorized:
                    return "ServerError";
                case HttpCode.NotFound:
                    return "NotFound";
                case HttpCode.ServerError:
                    return "ServerError";
            }
            return "NotFound";
        }

        public async Task<SelectList> GetInventoriesByUserId(string UserId, long? Id)
        {
            var result = await lookupsService.GetInventoriesByUserIdAsync(UserId);
            return result.IsSuccess ? new SelectList(result.Data, "Id", "Name", Id) : null;
        }
        public async Task<SystemSettingView> GetSystemSettings()
        {
            var result = await systemSettingService.LoadSystemSettingAsync();
            if (result != null)
                return
                    result.Data;
            else
                return new SystemSettingView();
        }

        public async Task<SelectList> GetUsersByFleetId(long fleetId)
        {
            var result = await lookupsService.GetUsersAsync(fleetId);
            return result.IsSuccess ? new SelectList(result.Data, "Id", "UserName", null) : null;
        }

    }
}
