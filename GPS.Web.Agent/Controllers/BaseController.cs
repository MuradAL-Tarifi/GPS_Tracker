using GPS.Domain.DTO;
using GPS.Services.Lookups;
using GPS.Web.Agent.AppCode.Helpers;
using GPS.Web.Agent.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GPS.Web.Agent.Controllers
{
    [Authorize(Roles = "agent")]
    public class BaseController : Controller
    {
        protected readonly IViewHelper _viewHelper;
        protected readonly LoggedInUserProfile _loggedUser;
        protected readonly ILookupsService _lookupsService;
        protected UserProfile UserProfile { get; set; }

        public BaseController(IViewHelper viewHelper, LoggedInUserProfile loggedUser)
        {
            _viewHelper = viewHelper;
            _loggedUser = loggedUser;

            UserProfile = new UserProfile()
            {
                Id = loggedUser.UserId,
                UserName = loggedUser.UserName,
                Name = loggedUser.Name,
                AgentId = loggedUser.AgentId,
                FleetId = loggedUser.FleetId,
                FleetName = loggedUser.FleetName,
                PrivilegeTypeIds = loggedUser.UserPrivilegesTypeIds,
                UserWarehouses = loggedUser.UserWarehouses,
                UserInventoryIds = loggedUser.UserInventories
            };
        }
        public BaseController(IViewHelper viewHelper, LoggedInUserProfile loggedUser, ILookupsService lookupsService)
        {
            _viewHelper = viewHelper;
            _loggedUser = loggedUser;
            _lookupsService = lookupsService;

            UserProfile = new UserProfile()
            {
                Id = loggedUser.UserId,
                UserName = loggedUser.UserName,
                Name = loggedUser.Name,
                AgentId = loggedUser.AgentId,
                FleetId = loggedUser.FleetId,
                FleetName = loggedUser.FleetName,
                PrivilegeTypeIds = loggedUser.UserPrivilegesTypeIds,
                UserWarehouses = loggedUser.UserWarehouses,
                UserInventoryIds = loggedUser.UserInventories
            };
        }

        public bool IsEnglish
        {
            get
            {
                return Thread.CurrentThread.CurrentCulture.Name.Equals("en-US");
            }
        }
        protected async Task<int> LoadBrands(int? BrandId = null)
        {
            var selectList = await _viewHelper.GetBrand(BrandId);
            ViewBag.Brands = selectList;
            return BrandId ?? Convert.ToInt32(selectList.SelectedValue);
        }
        protected async Task<long> LoadWarehouses(long? warehouseId = null)
        {
            var selectList = await _viewHelper.GetWarehousesByUserId(_loggedUser.UserId, warehouseId);
            ViewBag.Warehouses = selectList;
            return warehouseId ?? Convert.ToInt64(selectList.SelectedValue);
        }
        protected async Task LoadWarehouses(long? FleetId, long? Id = null)
        {
            ViewBag.Warehouses = await _viewHelper.GetWarehouses(FleetId, Id, UserProfile.UserInventoryIds);
        }
        protected async Task LoadInventories(long WarehouseId, long? Id = null)
        {
            ViewBag.Inventories = await _viewHelper.GetInventories(WarehouseId, Id, UserProfile.UserInventoryIds);
        }

        protected async Task LoadInventoriesByUserId(long? InventoryId = null)
        {
            ViewBag.Inventories = await _viewHelper.GetInventoriesByUserId(_loggedUser.UserId, InventoryId);
        }

        protected async Task LoadInventorySensorsBySerial(long? InventoryId, string sensorSerial = null)
        {
            ViewBag.InventorySensors = await _viewHelper.GetInventorySensorsBySerial((long)InventoryId, sensorSerial);
        }

        protected async Task LoadInventorySensorsBySensorId(long? InventoryId, long? sensorId = null)
        {
            ViewBag.InventorySensors = await _viewHelper.GetInventorySensorsBySensorId((long)InventoryId, sensorId);
        }

        protected async Task LoadWarehousesAndInventories(long fleetId, string selectedInventoryIds, bool viewOnly, List<long> PermittedInventoryIds)
        {
            var result = await _lookupsService.GetWarehousesAndInventoriesAsync(fleetId);
            if (result.IsSuccess)
            {
                var jsTreeObjects = JsTreeHelper.FillJsTree(result.Data, selectedInventoryIds, viewOnly, PermittedInventoryIds);
                ViewBag.WarehousesInventories = JsonConvert.SerializeObject(jsTreeObjects);
            }
        }
        protected async Task LoadWarehousesAndInventoriesAndSensors(long fleetId, List<long> PermittedInventoryIds)
        {
            var result = await _lookupsService.GetWarehousesAndInventoriesAsyncAndSensors(fleetId);
            if (result.IsSuccess && result.Data.Count > 0)
            {
                var jsTreeObjects = JsTreeHelper.FillJsTreeWarehousesInventoriesSensors(result.Data, PermittedInventoryIds);
                ViewBag.WarehousesInventoriesSensors = JsonConvert.SerializeObject(jsTreeObjects);
            }
        }
    }
}
