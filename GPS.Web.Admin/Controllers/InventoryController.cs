using System.Collections.Generic;
using System.Threading.Tasks;
using GPS.Web.Admin.AppCode;
using GPS.Web.Admin.AppCode.Extensions.Alerts;
using GPS.Web.Admin.AppCode.Helpers;
using GPS.Web.Admin.Models;
using GPS.Services;
using GPS.Domain;
using GPS.Domain.DTO;
using GPS.Domain.Views;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using X.PagedList;
using GPS.Helper;
using Newtonsoft.Json;
using GPS.Domain.ViewModels;
using GPS.Services.Inventorys;
using GPS.Services.Gateways;

namespace GPS.Web.Admin.Controllers
{
    public class InventoryController : BaseController
    {
        private readonly IInventoryService _inventoryService;
        private readonly IGatewayService _gatewayService;

        private readonly IStringLocalizer<InventoryController> _localizer;

        public InventoryController(
            IInventoryService inventoryService,
            IGatewayService gatewayService,
            IViewHelper viewHelper,
            IStringLocalizer<InventoryController> localizer,
            LoggedInUserProfile loggedUser)
            : base(viewHelper, loggedUser)
        {
            _inventoryService = inventoryService;
            _gatewayService = gatewayService;
            _localizer = localizer;
        }

        [UserPrivilege(Privilege = AdminPrivilegeTypeEnum.ViewInventories)]
        public async Task<IActionResult> Index(int? agentId = null, long? fleetId = null, long? warehouseId = null, int? page = 1, int? show = 100, string search = "", string returnURL = "", int? waslLinkStatus = null, int? isActive = null)
        {
            if (!string.IsNullOrEmpty(returnURL))
            {
                return Redirect(returnURL);

            }
            fleetId = fleetId == 0 ? null : fleetId;
            warehouseId = warehouseId == 0 ? null : warehouseId;
            var pageNumber = page ?? 1;
            var pageSize = show ?? 10;
            waslLinkStatus = waslLinkStatus < 0 ? null : waslLinkStatus;
            isActive = isActive < 0 ? null : isActive;

            // Save current query in ViewBag for pager
            ViewBag.CurrentQuery = new Dictionary<string, string>() { 
                { "agentId", agentId.ToString() },
                { "fleetId", fleetId.ToString() }, 
                { "warehouseId", warehouseId.ToString() }, 
                { "search", search },
                { "waslLinkStatus", waslLinkStatus.ToString() },
                { "isActive", isActive.ToString() },
                { "show", pageSize.ToString() } };

            agentId = await LoadAgents(agentId);
            await LoadFleets(agentId, fleetId);
            await LoadWarehouses(fleetId, warehouseId);

            var result = await _inventoryService.SearchAsync(fleetId, warehouseId, waslLinkStatus, isActive, search, pageNumber, pageSize);
            if (!result.IsSuccess)
            {
                return View(_viewHelper.GetErrorPage(result.HttpCode));
            }

            // Convert result data list to StaticPagedList
            var PagedResult = new StaticPagedList<InventoryView>(result.Data.List, pageNumber, pageSize, result.Data.TotalRecords);

            // Check if request is ajax request
            if (HttpContext.Request.Headers["x-requested-with"] == "XMLHttpRequest")
            {
                return PartialView("_data", PagedResult);
            }

            return View(PagedResult);
        }

        // GET: Fleets/Details/5
        [UserPrivilege(Privilege = AdminPrivilegeTypeEnum.ViewInventories)]
        public async Task<IActionResult> Details(long? id, string returnURL)
        {
            if (id == null)
            {
                return View("NotFound");
            }

            var result = await _inventoryService.FindByIdAsync(id);
            if (!result.IsSuccess)
            {
                ViewBag.Errors = result.ErrorList;
                return View(_viewHelper.GetErrorPage(result.HttpCode));
            }

            ViewBag.ReturnURL = returnURL;
            return View(result.Data);
        }

        // GET: Fleets/Edit/5
        [UserPrivilege(Privilege = AdminPrivilegeTypeEnum.AddUpdateInventories)]
        public async Task<IActionResult> Create(long? id, long? FleetId, long? warehouseId, string returnURL)
        {
            if (id == null)
            {
                ViewBag.ReturnURL = returnURL;
                return View(new InventoryView() { Warehouse = new WarehouseView() { FleetId = FleetId ?? 0 }, WarehouseId = warehouseId ?? 0 });
            }
            else
            {
                var result = await _inventoryService.FindByIdAsync(id);
                if (!result.IsSuccess)
                {
                    ViewBag.Errors = result.ErrorList;
                    return View(_viewHelper.GetErrorPage(result.HttpCode));
                }

                ViewBag.ReturnURL = returnURL;
                return View(result.Data);
            }
        }

        // POST: Fleets/Edit/5
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] InventoryView model)
        {
            if (model.Id > 0)
            {
                model.UpdatedBy = _loggedUser.UserId;
            }
            else
            {
                model.CreatedBy = _loggedUser.UserId;
                model.Warehouse = null;
            }

            var result = await _inventoryService.SaveAsync(model);
            return StatusCode((int)result.HttpCode, result);
        }

        // POST: Groups/Delete/5
        [HttpPost, ValidateAntiForgeryToken, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(long ItemId, string returnURL)
        {
            var result = await _inventoryService.DeleteAsync(ItemId, _loggedUser.UserId);
            if (!result.IsSuccess)
            {
                return View(_viewHelper.GetErrorPage(result.HttpCode));
            }

            return RedirectToAction(nameof(Index), new { returnURL }).WithSuccess(_localizer["DeleteSuccess"], string.Join("<br>", result.ErrorList));
        }

        [HttpGet]
        public async Task<string> GatSensorByGateway(long GatewayId)
        {
            var gateway = await _gatewayService.FindByIdAsync(GatewayId);
            return await GetSensors(gateway.Data.BrandId);
        }

        [HttpGet]
        public async Task<IActionResult> IsInventoryNumberExists(long fleetId, string inventoryNumber)
        {
            var result = await _inventoryService.IsInventoryNumberExists(fleetId, inventoryNumber);
            return StatusCode((int)result.HttpCode, result);
        }

        [HttpPost]
        public async Task<IActionResult> LinkWithWasl(long id)
        {
            var result = await _inventoryService.LinkWithWasl(id, _loggedUser.UserId);
            return StatusCode((int)result.HttpCode, result);
        }

        [HttpPost]
        public async Task<IActionResult> UnlinkWithWasl(long id)
        {
            var result = await _inventoryService.UnlinkWithWasl(id, _loggedUser.UserId);
            return StatusCode((int)result.HttpCode, result);
        }

        public async Task<bool> ValidateInventorySensor(long SensorId)
        {
            return await _inventoryService.IsInventorySensorExistsAsync(SensorId);
        }
    }
}
