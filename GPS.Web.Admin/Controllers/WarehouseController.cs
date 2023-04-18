using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GPS.Domain.DTO;
using GPS.Domain.Views;
using GPS.Services.SystemSettings;
using GPS.Services.WareHouses;
using GPS.Web.Admin.AppCode;
using GPS.Web.Admin.AppCode.Extensions.Alerts;
using GPS.Web.Admin.AppCode.Helpers;
using GPS.Web.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using X.PagedList;

namespace GPS.Web.Admin.Controllers
{
    public class WarehouseController : BaseController
    {
        private readonly IWarehouseService _warehouseService;
        private readonly IStringLocalizer<WarehouseController> _localizer;
        private readonly ISystemSettingService _systemSettingService;

        public WarehouseController(IWarehouseService warehouseService, IViewHelper viewHelper, ISystemSettingService systemSettingService,
            IStringLocalizer<WarehouseController> localizer, LoggedInUserProfile loggedUser)
            : base(viewHelper, loggedUser)
        {
            _warehouseService = warehouseService;
            _localizer = localizer;
            _systemSettingService = systemSettingService;
        }

        [UserPrivilege(Privilege = AdminPrivilegeTypeEnum.ViewWarehouses)]
        public async Task<IActionResult> Index(int? agentId = null, long? fleetId = null, int? page = 1, int? show = 100,
            string search = "", string returnURL = "", int? waslLinkStatus = null, int? isActive = null)
        {
            if (!string.IsNullOrEmpty(returnURL))
            {
                return Redirect(returnURL);
            }

            fleetId = fleetId == 0 ? null : fleetId;
            var pageNumber = page ?? 1;
            var pageSize = show ?? 10;
            waslLinkStatus = waslLinkStatus < 0 ? null : waslLinkStatus;
            isActive = isActive < 0 ? null : isActive;

            // Save current query in ViewBag for pager
            ViewBag.CurrentQuery = new Dictionary<string, string>() 
            { { "agentId", agentId.ToString() },
                { "fleetId", fleetId.ToString() },
                { "waslLinkStatus", waslLinkStatus.ToString() },
                { "isActive", isActive.ToString() },
                { "search", search },
                { "show", pageSize.ToString() } };

            agentId = await LoadAgents(agentId);
            await LoadFleets(agentId, fleetId);

            var result = await _warehouseService.SearchAsync(fleetId, waslLinkStatus, isActive, search, pageNumber, pageSize);
            if (!result.IsSuccess)
            {
                return View(_viewHelper.GetErrorPage(result.HttpCode));
            }

            // Convert result data list to StaticPagedList
            var PagedResult = new StaticPagedList<WarehouseView>(result.Data.List, pageNumber, pageSize, result.Data.TotalRecords);

            // Check if request is ajax request
            if (HttpContext.Request.Headers["x-requested-with"] == "XMLHttpRequest")
            {
                return PartialView("_data", PagedResult);
            }

            return View(PagedResult);
        }

        [UserPrivilege(Privilege = AdminPrivilegeTypeEnum.ViewWarehouses)]
        public async Task<IActionResult> Details(long? id, string returnURL)
        {
            if (id == null)
            {
                return View("NotFound");
            }

            var result = await _warehouseService.FindDetailedWarehouseByIdAsync((long)id);
            if (!result.IsSuccess)
            {
                ViewBag.Errors = result.ErrorList;
                return View(_viewHelper.GetErrorPage(result.HttpCode));
            }

            ViewBag.ReturnURL = returnURL;
            return View(result.Data);
        }

        [UserPrivilege(Privilege = AdminPrivilegeTypeEnum.AddUpdateWarehouses)]
        public async Task<IActionResult> Create(long? id, long? FleetId, string returnURL)
        {
            if (id == null)
            {
                ViewBag.ReturnURL = returnURL;
                return View(new WarehouseView() {  FleetId = FleetId ?? 0 });
            }
            else
            {
                var result = await _warehouseService.FindByIdAsync(id);
                if (!result.IsSuccess)
                {
                    ViewBag.Errors = result.ErrorList;
                    return View(_viewHelper.GetErrorPage(result.HttpCode));
                }
                var resultSystemSetting = await _systemSettingService.LoadSystemSettingAsync();
                if (resultSystemSetting.Data != null)
                {
                    ViewBag.GoogleApiKey = resultSystemSetting.Data.GoogleApiKey;
                }
                ViewBag.ReturnURL = returnURL;
                return View(result.Data);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] WarehouseView model)
        {
            if (model.Id > 0)
            {
                model.UpdatedBy = _loggedUser.UserId;
            }
            else
            {
                model.CreatedBy = _loggedUser.UserId;
            }

            var result = await _warehouseService.SaveAsync(model);
            return StatusCode((int)result.HttpCode, result);
        }

        [HttpPost, ValidateAntiForgeryToken, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(long ItemId, string returnURL)
        {
            var result = await _warehouseService.DeleteAsync(ItemId, _loggedUser.UserId);
            if (!result.IsSuccess)
            {
                return View(_viewHelper.GetErrorPage(result.HttpCode));
            }

            return RedirectToAction(nameof(Index), new { returnURL }).WithSuccess(_localizer["DeleteSuccess"], string.Join("<br>", result.ErrorList));
        }

        [HttpPost]
        public async Task<IActionResult> LinkWithWasl(long id)
        {
            var result = await _warehouseService.LinkWithWasl(id, _loggedUser.UserId);
            return StatusCode((int)result.HttpCode, result);
        }

        [HttpPost]
        public async Task<IActionResult> UnlinkWithWasl(long id)
        {
            var result = await _warehouseService.UnlinkWithWasl(id, _loggedUser.UserId);
            return StatusCode((int)result.HttpCode, result);
        }


    }
}

