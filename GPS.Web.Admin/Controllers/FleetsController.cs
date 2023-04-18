using GPS.Domain.DTO;
using GPS.Domain.ViewModels;
using GPS.Domain.Views;
using GPS.Services.Fleets;
using GPS.Services.Inventorys;
using GPS.Services.WareHouses;
using GPS.Web.Admin.AppCode;
using GPS.Web.Admin.AppCode.Extensions.Alerts;
using GPS.Web.Admin.AppCode.Helpers;
using GPS.Web.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using X.PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GPS.Web.Admin.Controllers
{
    public class FleetsController : BaseController
    {
        private readonly IFleetService _fleetService;
        private readonly IWarehouseService _warehouseService;
        private readonly IInventoryService _inventoryService;
        private readonly IStringLocalizer<FleetsController> _localizer;

        public FleetsController(IFleetService fleetService,
            IViewHelper viewHelper,
            IWarehouseService warehouseService,
            IInventoryService inventoryService,
            IStringLocalizer<FleetsController> localizer, LoggedInUserProfile loggedUser)
            : base(viewHelper, loggedUser)
        {
            _fleetService = fleetService;
            _localizer = localizer;
            _warehouseService = warehouseService;
            _inventoryService = inventoryService;
        }
        [Route("Companies/Index")]
        [UserPrivilege(Privilege = AdminPrivilegeTypeEnum.ViewFleets)]
        public async Task<IActionResult> Index(int? page = 1, int? show = 100, string search = "",int? waslLinkStatus = null)
        {
            var pageNumber = page ?? 1;
            var pageSize = show ?? 10;
            waslLinkStatus = waslLinkStatus < 0 ? null : waslLinkStatus;

            // Save current query in ViewBag for pager
            ViewBag.CurrentQuery = new Dictionary<string, string>() { 
                { "search", search },
                { "show", pageSize.ToString() },
                { "waslLinkStatus", waslLinkStatus.ToString() }};

            var result = await _fleetService.SearchAsync(SearchString: search, waslLinkStatus: waslLinkStatus, PageNumber: pageNumber, pageSize: pageSize);
            if (!result.IsSuccess)
            {
                return View(_viewHelper.GetErrorPage(result.HttpCode));
            }

            // Convert result data list to StaticPagedList
            var PagedResult = new StaticPagedList<FleetView>(result.Data.List, pageNumber, pageSize, result.Data.TotalRecords);

            // Check if request is ajax request
            if (HttpContext.Request.Headers["x-requested-with"] == "XMLHttpRequest")
            {
                return PartialView("_data", PagedResult);
            }

            return View(PagedResult);
        }

        // GET: Fleets/Create
        [UserPrivilege(Privilege = AdminPrivilegeTypeEnum.AddUpdateFleets)]
        [Route("Companies/Create")]
        public async Task<IActionResult> Create()
        {
            await LoadAgents();
            return View();
        }

        // POST: Fleets/Create
        [HttpPost, ValidateAntiForgeryToken]
        [Route("Companies/Create")]
        public async Task<IActionResult> Create(FleetView fleet)
        {
            if (ModelState.IsValid)
            {
                fleet.CreatedBy = _loggedUser.UserId;
                var result = await _fleetService.AddAsync(fleet);
                if (!result.IsSuccess)
                {
                    return View(_viewHelper.GetErrorPage(result.HttpCode));
                }

                return RedirectToAction(nameof(Create)).WithSuccessOptions(_localizer["AddSuccess"], "", _localizer["AddNewFleet"], Url.Action(nameof(Index)));
            }

            await LoadAgents(fleet.AgentId);

            return View(fleet);
        }

        [UserPrivilege(Privilege = AdminPrivilegeTypeEnum.AddUpdateFleets)]
        [Route("Companies/CreateWasl")]
        public IActionResult CreateWasl(long fleetId)
        {
            return View(new FleetDetailsView() { FleetId = fleetId });
        }

        // GET: Fleets/Details/5
         [UserPrivilege(Privilege = AdminPrivilegeTypeEnum.ViewFleets)]
        [Route("Companies/Details")]
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return View("NotFound");
            }

            var result = await _fleetService.FindByIdAsync(id);
            if (!result.IsSuccess)
            {
                ViewBag.Errors = result.ErrorList;
                return View(_viewHelper.GetErrorPage(result.HttpCode));
            }

            return View(result.Data);
        }

        // GET: Fleets/Edit/5
        // [UserPrivilege(Privilege = AdminPrivilegeTypeEnum.AddUpdateFleets)]
        [HttpGet]
        [Route("Companies/Edit")]
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return View("NotFound");
            }

            var result = await _fleetService.FindByIdAsync(id);
            if (!result.IsSuccess)
            {
                ViewBag.Errors = result.ErrorList;
                return View(_viewHelper.GetErrorPage(result.HttpCode));
            }

            await LoadAgents(result.Data.AgentId);

            return View(result.Data);
        }

        // POST: Fleets/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        [Route("Companies/Edit")]
        public async Task<IActionResult> Edit(FleetView fleet)
        {
            if (fleet.Id <= 0)
            {
                return View("NotFound");
            }

            if (ModelState.IsValid)
            {
                fleet.UpdatedBy = _loggedUser.UserId;
                var result = await _fleetService.UpdateAsync(fleet);
                if (!result.IsSuccess)
                {
                    return View(_viewHelper.GetErrorPage(result.HttpCode));
                }

                return RedirectToAction(nameof(Edit), new { Id = fleet.Id }).WithSuccessOptions(_localizer["UpdateSuccess"], "", _localizer["ContinueEdit"], Url.Action(nameof(Index)));
            }

            await LoadAgents(fleet.AgentId);

            return View(fleet);
        }

        // GET: Fleets/Edit/5
        [UserPrivilege(Privilege = AdminPrivilegeTypeEnum.AddUpdateFleets)]
        [Route("Companies/EditWasl")]
        public async Task<IActionResult> EditWasl(long? id)
        {
            if (id == null)
            {
                return View("NotFound");
            }

            var result = await _fleetService.GetWaslDetailsAsync(id);
            if (!result.IsSuccess)
            {
                ViewBag.Errors = result.ErrorList;
                return View(_viewHelper.GetErrorPage(result.HttpCode));
            }

            return View(result.Data);
        }

        // POST: Fleets/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        [Route("Companies/EditWasl")]
        public async Task<IActionResult> EditWasl(FleetWaslModel model)
        {
            ModelState.Remove("FleetId");

            model.UpdatedBy = _loggedUser.UserId;

            var waslDetailsResult = await _fleetService.GetWaslDetailsAsync(model.FleetId);
            // update wasl info if companies is already linked
            if (waslDetailsResult.IsSuccess && waslDetailsResult.Data != null && waslDetailsResult.Data.IsLinkedWithWasl)
            {
                ModelState.Remove("DateOfBirthHijri");
                ModelState.Remove("CommercialRecordIssueDateHijri");
                ModelState.Remove("EmailAddress");

                if (ModelState.IsValid)
                {
                    var UpdateWaslContactInfoResult = await _fleetService.UpdateWaslContactInfoAsync(model);
                    if (!UpdateWaslContactInfoResult.IsSuccess)
                    {
                        return RedirectToAction(nameof(EditWasl)).WithWarning("", string.Join("<br>", UpdateWaslContactInfoResult.ErrorList));
                    }

                    return RedirectToAction(nameof(EditWasl), new { id = model.FleetId }).WithSuccessOptions(_localizer["UpdateWaslContactInfoSuccess"],
                        string.Join("<br>", UpdateWaslContactInfoResult.ErrorList), _localizer["ContinueEdit"], Url.Action(nameof(Index)));
                }
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var result = await _fleetService.UpdateWaslDetailsAsync(model);
                    if (!result.IsSuccess)
                    {
                        return View(_viewHelper.GetErrorPage(result.HttpCode));
                    }

                    return RedirectToAction(nameof(EditWasl), new { id = model.FleetId }).WithSuccessOptions(_localizer["WaslInfoUpdateSuccess"], string.Join("<br>", result.ErrorList),
                        _localizer["ContinueEdit"], Url.Action(nameof(Index)));
                }
            }

            return View(model);
        }

        // POST: Groups/Delete/5
        [HttpPost, ValidateAntiForgeryToken, ActionName("Delete")]
        [Route("Companies/Delete")]
        public async Task<IActionResult> DeleteConfirmed(long ItemId)
        {
            var result = await _fleetService.DeleteAsync(ItemId, _loggedUser.UserId);
            if (!result.IsSuccess)
            {
                return View(_viewHelper.GetErrorPage(result.HttpCode));
            }

            return RedirectToAction(nameof(Index)).WithSuccess(_localizer["DeleteSuccess"], string.Join("<br>", result.ErrorList));
        }

        [HttpPost]
        public async Task<IActionResult> LinkWithWasl(long id)
        {
            var result = await _fleetService.LinkWithWasl(id, _loggedUser.UserId);
            return StatusCode((int)result.HttpCode, result);
        }

        [HttpPost]
        public async Task<IActionResult> UnlinkWithWasl(long id)
        {
            var result = await _fleetService.UnlinkWithWasl(id, _loggedUser.UserId);
            return StatusCode((int)result.HttpCode, result);
        }

        [HttpPost]
        public async Task<IActionResult> UnlinkWithWaslAll(long id)
        {
            var result = new ReturnResult<bool>();

            var errorList = new List<string>();
            var successList = new List<string>();

            var fleetWarehouses = await _warehouseService.GetFleetLinkedWithWaslWarehousesAsync(id);
            if (fleetWarehouses.IsSuccess && fleetWarehouses.Data != null)
            {
                foreach (var warehouse in fleetWarehouses.Data)
                {
                    bool allInventoriesSuccess = true;
                    foreach (var inventory in warehouse.Inventories)
                    {
                        var inventoryResult = await _inventoryService.UnlinkWithWasl(inventory.Id, _loggedUser.UserId);
                        if (inventoryResult.IsSuccess)
                        {
                            successList.Add($"<span class='text-success'>نجاح فك ربط المخزن ({inventory.Name})</span>");
                        }
                        else
                        {
                            allInventoriesSuccess = false;
                            errorList.Add($"<span class='text-danger'>فشل فك ربط المخزن {inventory.Name} ({string.Join(",", inventoryResult.ErrorList)})</span>");
                        }
                    }

                    if (allInventoriesSuccess)
                    {
                        var warehouseResult = await _warehouseService.UnlinkWithWasl(warehouse.Id, _loggedUser.UserId);
                        if (warehouseResult.IsSuccess)
                        {
                            successList.Add($"<span class='text-success'>نجاح فك ربط المستودع ({warehouse.Name})</span>");
                        }
                        else
                        {
                            allInventoriesSuccess = false;
                            errorList.Add($"<span class='text-danger'>فشل فك ربط المستودع {warehouse.Name} ({string.Join(",", warehouseResult.ErrorList)})</span>");
                        }
                    }
                }
            }

            if (errorList.Count == 0)
            {
                result = await _fleetService.UnlinkWithWasl(id, _loggedUser.UserId);
            }
            else
            {
                result.BadRequest(errorList);
                result.ErrorList = successList;
                result.ErrorList.AddRange(errorList);
            }

            return StatusCode((int)result.HttpCode, result);
        }

        [HttpGet]
        public async Task<bool> ValidateName(int AgentId, string Name)
        {
            return await _fleetService.IsNameExistsAsync(AgentId, Name);
        }

        [HttpGet]
        public async Task<bool> ValidateNameEn(int AgentId, string NameEn)
        {
            return await _fleetService.IsNameEnExistsAsync(AgentId, NameEn);
        }
    }
}
