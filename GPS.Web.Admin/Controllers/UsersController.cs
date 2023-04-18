using GPS.Domain.DTO;
using GPS.Domain.Models;
using GPS.Domain.Views;
using GPS.Domain.ViewModels;
using GPS.Helper;
using GPS.Services.Users;
using GPS.Web.Admin.AppCode;
using GPS.Web.Admin.AppCode.Extensions.Alerts;
using GPS.Web.Admin.AppCode.Helpers;
using GPS.Web.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;
using GPS.Services.Lookups;

namespace GPS.Web.Admin.Controllers
{
    public class UsersController : BaseController
    {
        private readonly IStringLocalizer<UsersController> _localizer;
        private readonly IUserService _userService;

        public UsersController(IViewHelper viewHelper, IStringLocalizer<UsersController> localizer,
            LoggedInUserProfile loggedUser, IUserService userService, ILookupsService lookupsService)
            : base(viewHelper, loggedUser, lookupsService)
        {
            _localizer = localizer;
            _userService = userService;
        }

        // GET: Users
        [UserPrivilege(Privilege = AdminPrivilegeTypeEnum.ViewUsers)]
        public async Task<IActionResult> Index(int? agentId = null, long? fleetId = null, long? warehouseId = null,
            int? isActive = null, int? page = 1, int? show = 100, string search = "", string returnURL = "")
        {
            if (!string.IsNullOrEmpty(returnURL))
            {
                return Redirect(returnURL);
            }

            fleetId = fleetId <= 0 ? null : fleetId;
            warehouseId = warehouseId <= 0 ? null : warehouseId;
            isActive = isActive < 0 ? null : isActive;
            var pageNumber = page ?? 1;
            var pageSize = show ?? 10;

            // Save current query in ViewBag for pager
            ViewBag.CurrentQuery = new Dictionary<string, string>()
            {
                { "agentId", agentId.ToString() },
                { "fleetId", fleetId.ToString() },
                { "warehouseId", warehouseId.ToString() },
                { "isActive", isActive.ToString() },
                { "search", search }, { "show", pageSize.ToString() }
            };

            agentId = await LoadAgents(agentId);


            await LoadFleets(agentId, fleetId);
            await LoadWarehouses(fleetId, warehouseId);

            var result = await _userService.SearchAsync(agentId, fleetId, warehouseId, isActive, search, _loggedUser.IsSuperAdmin, false, pageNumber, pageSize);
            if (!result.IsSuccess)
            {
                return View(_viewHelper.GetErrorPage(result.HttpCode));
            }

            // Convert result data list to StaticPagedList
            var PagedResult = new StaticPagedList<UserView>(result.Data.List, pageNumber, pageSize, result.Data.TotalRecords);

            // Check if request is ajax request
            if (HttpContext.Request.Headers["x-requested-with"] == "XMLHttpRequest")
            {
                return PartialView("_data", PagedResult);
            }

            return View(PagedResult);
        }

        // GET: Users/Create
        [UserPrivilege(Privilege = AdminPrivilegeTypeEnum.AddUpdateUsers)]
        public async Task<IActionResult> Create(int? AgentId, long? FleetId, string returnURL)
        {
            AgentId = await LoadAgents(AgentId);
            FleetId = await LoadFleets(AgentId, selectFleetId: true);
            //await LoadWarehouses(FleetId: FleetId);
            await LoadRoles();
            await LoadWarehousesAndInventories((long)FleetId, null,false);
            UserView userView = new UserView()
            {
                ExpirationDateText = DateTime.Now.AddYears(1).ToString("yyyy/MM/dd", new CultureInfo("en").DateTimeFormat)
            };

            ViewBag.ReturnURL = returnURL;
            return View(userView);
        }

        // POST: Users/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserView user, int? AgentId, string Role, string returnURL)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(user.ExpirationDateText))
                {
                    user.ExpirationDate = GPSHelper.StringToDateTime(user.ExpirationDateText);
                }

                user.CreatedBy = _loggedUser.UserId;
                user.Role.Name = Role;
                var result = await _userService.SaveAsync(user);
                if (!result.IsSuccess)
                {
                    return View(_viewHelper.GetErrorPage(result.HttpCode));
                }

                return RedirectToAction(nameof(Create), new { AgentId, user.FleetId,returnURL }).WithSuccessOptions(_localizer["AddSuccess"], "", _localizer["AddNewUser"], returnURL);
            }

            await LoadAgents(AgentId);
            await LoadFleets(AgentId, user.FleetId);
            //await LoadWarehouses(FleetId: user.FleetId);
            await LoadWarehousesAndInventories((long)user.FleetId, user.InventoriesIds, false);
            await LoadRoles(Role);

            ViewBag.ReturnURL = returnURL;
            return View(user);
        }

        // GET: Details/Details/5
        [UserPrivilege(Privilege = AdminPrivilegeTypeEnum.ViewUsers)]
        public async Task<IActionResult> Details(string id, string returnURL)
        {
            if (id == null)
            {
                return View("NotFound");
            }

            var result = await _userService.FindAsync(id);
            if (!result.IsSuccess)
            {
                ViewBag.Errors = result.ErrorList;
                return View(_viewHelper.GetErrorPage(result.HttpCode));
            }
            if (result.Data.FleetId > 0)
            {
                await LoadWarehousesAndInventories((long)result.Data.FleetId, result.Data.InventoriesIds,true);
            }
            ViewBag.ReturnURL = returnURL;
            return View(result.Data);
        }

        // GET: Groups/Edit/5
        [UserPrivilege(Privilege = AdminPrivilegeTypeEnum.AddUpdateUsers)]
        public async Task<IActionResult> Edit(string id, string returnURL)
        {
            if (id == null)
            {
                return View("NotFound");
            }

            var result = await _userService.FindAsync(id);
            if (!result.IsSuccess)
            {
                ViewBag.Errors = result.ErrorList;
                return View(_viewHelper.GetErrorPage(result.HttpCode));
            }

            await LoadAgents(result.Data.AgentId);
            await LoadFleets(result.Data.AgentId, result.Data.FleetId);
            //await LoadWarehouses(FleetId: result.Data.FleetId);
            await LoadRoles(result.Data.Role.Name);
            if (result.Data.FleetId > 0)
            {
                await LoadWarehousesAndInventories((long)result.Data.FleetId, result.Data.InventoriesIds, false);
            }
            
            if (result.Data.ExpirationDate.HasValue)
            {
                result.Data.ExpirationDateText = result.Data.ExpirationDate.Value.ToString("yyyy/MM/dd", new CultureInfo("en").DateTimeFormat);
            }

            ViewBag.ReturnURL = returnURL;
            return View(result.Data);
        }

        // POST: Groups/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, UserView user, int? AgentId, string Role, string ReturnURL)
        {
            if (id != user.Id)
            {
                return View("NotFound");
            }

            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(user.ExpirationDateText))
                {
                    user.ExpirationDate = GPSHelper.StringToDateTime(user.ExpirationDateText);
                }

                user.Role.Name = Role;
                user.UpdatedBy = _loggedUser.UserId;
                var result = await _userService.SaveAsync(user);
                if (!result.IsSuccess)
                {
                    return View(_viewHelper.GetErrorPage(result.HttpCode));
                }

                return RedirectToAction(nameof(Edit), new { ReturnURL }).WithSuccessOptions(_localizer["UpdateSuccess"], "", _localizer["ContinueEdit"], ReturnURL);
            }

            await LoadAgents(AgentId);
            await LoadFleets(AgentId, user.FleetId);
            // await LoadWarehouses(FleetId: user.FleetId);
            await LoadWarehousesAndInventories((long)user.FleetId, user.InventoriesIds, false);
            await LoadRoles(Role);

            ViewBag.ReturnURL = ReturnURL;
            return View(user);
        }

        // POST: Groups/Delete/5
        [HttpPost, ValidateAntiForgeryToken, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string ItemId, string returnURL)
        {
            var result = await _userService.DeleteAsync(ItemId, _loggedUser.UserId);
            if (!result.IsSuccess)
            {
                return View(_viewHelper.GetErrorPage(result.HttpCode));
            }

            return RedirectToAction(nameof(Index), new { returnURL }).WithSuccess(_localizer["DeleteSuccess"], "");
        }

        [HttpGet]
        public async Task<bool> ValidateUsername(string Username)
        {
            return await _userService.IsUsernameExistsAsync(Username);
        }

        [HttpGet]
        public async Task<bool> ValidateEmail(string Email)
        {
            return await _userService.IsEmailExistsAsync(Email);
        }


        protected async Task LoadRoles(string Name = "")
        {
            ViewBag.Roles = await _viewHelper.GetRoles(Name);
        }

        [UserPrivilege(Privilege = AdminPrivilegeTypeEnum.EnableUserPrivileges)]
        public async Task<IActionResult> ManagePrivileges(string id, string returnURL)
        {
            if (id == null || string.IsNullOrEmpty(id))
            {
                return View("NotFound");
            }

            var result = await _userService.GetUserPrivilegesAsync(id);
            if (!result.IsSuccess)
            {
                ViewBag.Errors = result.ErrorList;
                return View(_viewHelper.GetErrorPage(result.HttpCode));
            }

            ViewBag.ReturnURL = returnURL;
            return View(result.Data);
        }


        [HttpPost]
        public async Task<string> UpdateUserPrivileges([FromBody] UserPrivilegesViewModel UserPrivileges)
        {
            var result = await _userService.SaveUserPrivilegesAsync(UserPrivileges.User.Id, UserPrivileges.Privileges, _loggedUser.UserId);
            return JsonConvert.SerializeObject(result.IsSuccess);
        }

    }
}
