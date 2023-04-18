using GPS.Domain.DTO;
using GPS.Domain.Views;
using GPS.Services.Agents;
using GPS.Services.Brands;
using GPS.Services.Fleets;
using GPS.Services.Lookups;
using GPS.Services.Sensors;
using GPS.Services.SystemSettings;
using GPS.Services.Users;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GPS.Web.Admin.AppCode.Helpers
{
    public interface IViewHelper
    {
        /// <summary>
        /// Get Default Page sizes
        /// </summary>
        /// <returns>10, 25, 50, 100, 500</returns>
        SelectList GetPageSizes();

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

        Task<SelectList> GetWarehouses(long? FleetId, long? Id);

        Task<SelectList> GetInventories(long WarehouseId, long? Id);

        Task<SelectList> GetInventorySensors(long InventoryId, long? sensorSerial);

        Task<SystemSettingView> GetSystemSettings();
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
        public ViewHelper(ICultureHelper _cultureHelper, IFleetService _fleetService, IAgentService _agentService,
            ILookupsService _lookupsService, IBrandService _brandService,
            ISensorService _sensorService, IUserService _userService, ISystemSettingService _systemSettingService)
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
            return result.IsSuccess ? new SelectList(result.Data, "Id", cultureHelper.GetLocalizedName("Serial", "Serial"), SensorId) : null;

        }

        public async Task<SelectList> GetRoles(string Role)
        {
            var result = await userService.GetRolesAsync();
            return result.IsSuccess ? new SelectList(result.Data, "Name", cultureHelper.GetLocalizedName("DisplayName", "DisplayNameEn"), Role) : null;
        }


        public async Task<SelectList> GetGateways(int? Id)
        {
            var result = await lookupsService.GetGatewaysAsync();
            return result.IsSuccess ? new SelectList(result.Data, "Id", "Name", Id) : null;
        }

        public async Task<SelectList> GetWarehouses(long? FleetId, long? Id)
        {
            var result = await lookupsService.GetWarehousesAsync(FleetId);
            return result.IsSuccess ? new SelectList(result.Data, "Id", "Name", Id) : null;
        }

        public async Task<SelectList> GetInventories(long WarehouseId, long? Id)
        {
            var result = await lookupsService.GetInventoriesAsync(WarehouseId);
            return result.IsSuccess ? new SelectList(result.Data, "Id", "Name", Id) : null;
        }

        public async Task<SelectList> GetInventorySensors(long InventoryId, long? sensorSerial)
        {
            var result = await lookupsService.GetInventorySensorsAsync(InventoryId);
            return result.IsSuccess ? new SelectList(result.Data, "Serial", "Name", sensorSerial) : null;
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

        public async Task<SystemSettingView> GetSystemSettings()
        {
            var result = await systemSettingService.LoadSystemSettingAsync();
            if (result != null)
                return
                    result.Data;
            else
                return new SystemSettingView();
        }
    }
}
