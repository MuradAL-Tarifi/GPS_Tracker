using GPS.Domain.DTO;
using GPS.Services.Lookups;
using GPS.Web.Admin.AppCode.Helpers;
using GPS.Web.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GPS.Web.Admin.Controllers
{
    [Authorize(Roles = "administrator")]
    public class BaseController : Controller
    {
        protected readonly IViewHelper _viewHelper;
        protected readonly LoggedInUserProfile _loggedUser;
        protected readonly ILookupsService _lookupsService;

        public BaseController(IViewHelper viewHelper, LoggedInUserProfile loggedUser)
        {
            _viewHelper = viewHelper;
            _loggedUser = loggedUser;
        }
        public BaseController(IViewHelper viewHelper, LoggedInUserProfile loggedUser, ILookupsService lookupsService)
        {
            _viewHelper = viewHelper;
            _loggedUser = loggedUser;
            _lookupsService = lookupsService;
        }

        protected async Task<int> LoadBrands(int? BrandId = null)
        {
            var selectList = await _viewHelper.GetBrand(BrandId);
            ViewBag.Brands = selectList;
            return BrandId ?? Convert.ToInt32(selectList.SelectedValue);
        }

        protected async Task<int> LoadAgents(int? AgentId = null)
        {
            var selectList = await _viewHelper.GetAgents(AgentId);
            ViewBag.Agents = selectList;
            return AgentId ?? Convert.ToInt32(selectList.SelectedValue);
        }

        protected async Task<long> LoadFleets(int? AgentId = null, long? FleetId = null, bool selectFleetId = false)
        {
            var selectList = await _viewHelper.GetFleets(AgentId, FleetId, selectFleetId);
            ViewBag.Fleets = selectList;
            return FleetId ?? Convert.ToInt64(selectList.SelectedValue);
        }
        public bool IsEnglish
        {
            get
            {
                return Thread.CurrentThread.CurrentCulture.Name.Equals("en-US");
            }
        }

        protected async Task LoadWarehouses(long? FleetId, long? Id = null)
        {
            ViewBag.Warehouses = await _viewHelper.GetWarehouses(FleetId, Id);
        }

        protected async Task LoadInventories(long WarehouseId, long? Id = null)
        {
            ViewBag.Inventories = await _viewHelper.GetInventories(WarehouseId, Id);
        }

        protected async Task LoadWarehouseSensors(long InventoryId, long? sensorSerial = null)
        {
            ViewBag.WarehouseSensors = await _viewHelper.GetInventorySensors(InventoryId, sensorSerial);
        }

        [HttpGet]
        public async Task<string> GetAgents(int id)
        {
            var agents = await _viewHelper.GetAgents(id);
            string result = JsonConvert.SerializeObject(agents);
            return result;
        }

        [HttpGet]
        public async Task<string> GetFleets(int AgentId)
        {
            var fleets = await _viewHelper.GetFleets(AgentId: AgentId);
            string result = JsonConvert.SerializeObject(fleets);
            return result;
        }

        [HttpGet]
        public async Task<string> GetSensors(int BrandId)
        {
            var sensors = await _viewHelper.GetSensors(BrandId);
            string result = JsonConvert.SerializeObject(sensors);
            return result;
        }

     
        [HttpGet]
        public async Task<string> GetGateways(int? Id = null)
        {
            var gateways = await _viewHelper.GetGateways(Id);
            return JsonConvert.SerializeObject(gateways);
        }

        [HttpGet]
        public async Task<string> GetBrands(int BrandId)
        {
            var brands = await _viewHelper.GetBrand(brandId: BrandId);
            return JsonConvert.SerializeObject(brands);
        }

        [HttpGet]
        public async Task<string> GetWarehouses(long? FleetId, long? Id)
        {
            var list = await _viewHelper.GetWarehouses(FleetId, Id);
            return JsonConvert.SerializeObject(list);
        }

        [HttpGet]
        public async Task<string> GetSystemSettings()
        {
            var data = await _viewHelper.GetSystemSettings();
            return JsonConvert.SerializeObject(data);
        }
        protected async Task LoadWarehousesAndInventories(long fleetId, string selectedInventoryIds, bool viewOnly)
        {
            var result = await _lookupsService.GetWarehousesAndInventoriesAsync(fleetId);
            if (result.IsSuccess)
            {
                var jsTreeObjects = JsTreeHelper.FillJsTree(result.Data, selectedInventoryIds, viewOnly);
                ViewBag.WarehousesInventories = JsonConvert.SerializeObject(jsTreeObjects);
            }
        }

        [HttpGet]
        public async Task<string> GetWarehousesAndInventories(long fleetId)
        {
            var result = await _lookupsService.GetWarehousesAndInventoriesAsync(fleetId);
            if (result.IsSuccess)
            {
                var jsTreeObjects = JsTreeHelper.FillJsTree(result.Data, null, false);
                return JsonConvert.SerializeObject(jsTreeObjects);
            }
            return null;
        }

    }
}
