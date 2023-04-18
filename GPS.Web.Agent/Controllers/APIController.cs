using GPS.API.Proxy;
using GPS.Domain.ViewModels;
using GPS.Helper;
using GPS.Services.Lookups;
using GPS.Web.Agent.AppCode.Helpers;
using GPS.Web.Agent.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GPS.Web.Agent.Controllers
{
    [Route("/[controller]/[action]")]
    public class APIController : BaseController
    {
        private readonly IAlertApiProxy _alertApiProxy;
        public APIController(
            IViewHelper viewHelper,
            LoggedInUserProfile loggedUser,ILookupsService lookupsService, IAlertApiProxy alertApiProxy)
            : base(viewHelper, loggedUser, lookupsService)
        {
            _alertApiProxy = alertApiProxy;
        }

        [HttpGet]
        public async Task<IActionResult> Warehouses()
        {
            var result = await _viewHelper.GetWarehousesByUserId(_loggedUser.UserId, null);
            if (result == null)
            {
                return NotFound();
            }
            return StatusCode(200, result);
        }

        [HttpGet]
        public async Task<IActionResult> Inventories(long warehouseId)
        {
            SelectList result = null;
            if(warehouseId > 0)
            {
                result = await _viewHelper.GetInventories(warehouseId, null, UserProfile.UserInventoryIds);
            }
            else
            {
                result = await _viewHelper.GetInventoriesByUserId(_loggedUser.UserId, null);
            }
             
            if (result == null)
            {
                return NotFound();
            }
            return StatusCode(200, result);
        }

        [HttpGet]
        public async Task<IActionResult> InventorySensors(long inventoryId,string sensorSerial = null, bool bySensorSerial = true)
        {
            var result = bySensorSerial ? await _viewHelper.GetInventorySensorsBySerial(inventoryId, sensorSerial) : 
                await _viewHelper.GetInventorySensorsBySensorId(inventoryId, null);
            if (result == null)
            {
                return NotFound();
            }
            return StatusCode(200, result);
        }

        [HttpGet]
        public async Task<IActionResult> ReportTypeLookup()
        {
            var result = await _viewHelper.GetReportTypeLookup();
            if (result == null)
            {
                return NotFound();
            }
            return StatusCode(200, result);
        }

        [HttpGet]
        public async Task<IActionResult> dayOfWeekLookup()
        {
            var result = await _viewHelper.GetDaysOfWeekLookup();
            if (result == null)
            {
                return NotFound();
            }
            return StatusCode(200, result);
        }
        
        [HttpGet]
        public async Task<string> GetSystemSettings()
        {
            var data = await _viewHelper.GetSystemSettings();
            return JsonConvert.SerializeObject(data);
        }

        [HttpGet]
        public async Task<IActionResult> Users()
        {
            var result = await _viewHelper.GetUsersByFleetId((long)_loggedUser.FleetId);
            if (result == null)
            {
                return NotFound();
            }
            return StatusCode(200, result);
        }

        [HttpGet]
        public async Task<IActionResult> Alerts()
        {
            var result = await _alertApiProxy.GetTop100Alerts(_loggedUser.UserId);
            if (result == null)
            {
                return NotFound();
            }
            return StatusCode(200, result);

        }
        [HttpGet]
        public async Task<IActionResult> AlertTypes()
        {
            var result = await _viewHelper.GetAlertTypes();
            if (result == null)
            {
                return NotFound();
            }
            return StatusCode(200, result);
        }
        [HttpPost]
        public async Task<IActionResult> ReadAlerts(List<long> alertIds)
        {
            var listParam = new ListParam {ids = alertIds };
            var result = await _alertApiProxy.UpdateAlertsAsRead(_loggedUser.UserId, listParam);
            if (result == null)
            {
                return NotFound();
            }
            return StatusCode(200, result);
        }
    }
}
