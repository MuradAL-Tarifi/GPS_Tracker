using DocumentFormat.OpenXml.Spreadsheet;
using GPS.Domain.DTO;
using GPS.Domain.Models;
using GPS.Domain.Views;
using GPS.Services.AlertBySensor;
using GPS.Services.AlertTracker;
using GPS.Services.SystemSettings;
using GPS.Services.WareHouses;
using GPS.Web.Admin.AppCode.Extensions.Alerts;
using GPS.Web.Admin.AppCode.Helpers;
using GPS.Web.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using X.PagedList;

namespace GPS.Web.Admin.Controllers
{
    public class AlertTrakerController : BaseController
    {
        private readonly IWarehouseService _warehouseService;
        private readonly IStringLocalizer<WarehouseController> _localizer;
        private readonly IAlertBySensorService _alertBySensorService;
        private readonly ISystemSettingService _systemSettingService;

        public AlertTrakerController(IWarehouseService warehouseService, IViewHelper viewHelper, ISystemSettingService systemSettingService,
            IStringLocalizer<WarehouseController> localizer, LoggedInUserProfile loggedUser, IAlertBySensorService alertBySensorService)
            : base(viewHelper, loggedUser)
        {
            _warehouseService = warehouseService;
            _localizer = localizer;
            _alertBySensorService = alertBySensorService;
            _systemSettingService = systemSettingService;
        }
        public async Task<IActionResult> Index(int? warehouseId = null, long? inventoryId = null, string serial = "",
     int? page = 1, int? show = 25, string returnURL = "")
        {
            await LoadAllWarehouses();

            if (!string.IsNullOrEmpty(returnURL))
            {
                return Redirect(returnURL);
            }

            warehouseId = warehouseId <= 0 ? null : warehouseId;
            inventoryId = inventoryId <= 0 ? null : inventoryId;
            serial = serial == "null" || serial == null ? "" : serial;
            var pageNumber = page ?? 1;
            var pageSize = show ?? 10;

            // Save current query in ViewBag for pager
            ViewBag.CurrentQuery = new Dictionary<string, string>()
            {
                { "warehouseId", warehouseId.ToString() },
                { "inventoryId", inventoryId.ToString() },
                { "serial", serial.ToString() },
                { "show", pageSize.ToString() }
            };

            var result = await _alertBySensorService.SearchAsync(warehouseId, inventoryId, serial, pageNumber, pageSize);
            if (!result.IsSuccess)
            {
                return View(_viewHelper.GetErrorPage(result.HttpCode));
            }

            // Convert result data list to StaticPagedList
            var PagedResult = new StaticPagedList<AlertSensorView>(result.Data.List, pageNumber, pageSize, result.Data.TotalRecords);

            // Check if request is ajax request
            if (HttpContext.Request.Headers["x-requested-with"] == "XMLHttpRequest")
            {
                return PartialView("_data", PagedResult);
            }

            return View(PagedResult);
        }
        public async Task<bool> ValidateAlertBySensor(string SensorSN)
        {
            return await _alertBySensorService.IsAlertBySensorExistsAsync(SensorSN);
        }
        // GET: Sensors/Create
        public async Task<IActionResult> Create(long? id, string returnURL)
        {
            if (id == null)
            {
                ViewBag.ReturnURL = returnURL;
                return View(new AlertSensorView());
            }
            else
            {
                var result = await _alertBySensorService.FindbyId(id);
                if (!result.IsSuccess)
                {
                    ViewBag.Errors = result.ErrorList;
                    return View(_viewHelper.GetErrorPage(result.HttpCode));
                }

                ViewBag.ReturnURL = returnURL;
                return View(result.Data);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AlertSensorView model)
        {
            ReturnResult<bool> result;

            if (model.Id > 0)
            {
                model.AlertTypeLookupId = 2;
                model.Interval = 60;
                model.CreatedDate = DateTime.Now;
                result = await _alertBySensorService.SaveAsync(model, _loggedUser.UserId) ;
            }
            else
            {
                model.AlertTypeLookupId = 2;
                model.Interval = 60;
                model.CreatedDate = DateTime.Now;
                result = await _alertBySensorService.SaveAsync(model, _loggedUser.UserId);
            }
            return StatusCode((int)result.HttpCode, result);
        }
        // POST: Sensors/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(long ItemId, string returnURL)
        {
            var result = await _alertBySensorService.Delete(ItemId, _loggedUser.UserId);
            if (!result.IsSuccess)
            {
                return View(_viewHelper.GetErrorPage(result.HttpCode));
            }
            return RedirectToAction(nameof(Index), new { returnURL }).WithSuccess(_localizer["DeleteSuccessAlertBySensor"], string.Join("<br>", result.ErrorList));
        }

    }
}
