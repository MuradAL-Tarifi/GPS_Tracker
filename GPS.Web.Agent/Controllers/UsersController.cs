using GPS.Domain.DTO;
using GPS.Domain.ViewModels;
using GPS.Domain.Views;
using GPS.Helper;
using GPS.Services.Lookups;
using GPS.Services.Users;
using GPS.Web.Agent.AppCode;
using GPS.Web.Agent.AppCode.Extensions.Alerts;
using GPS.Web.Agent.AppCode.Helpers;
using GPS.Web.Agent.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace GPS.Web.Agent.Controllers
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
        [UserPrivilege(Privilege = AgentPrivilegeTypeEnum.ViewUsers)]
        public async Task<IActionResult> Index(int? isActive = null, int? page = 1, int? show = 100, string search = "", string returnURL = "")
        {
            if (!string.IsNullOrEmpty(returnURL))
            {
                return Redirect(returnURL);
            }

            isActive = isActive < 0 ? null : isActive;
            var pageNumber = page ?? 1;
            var pageSize = show ?? 10;

            // Save current query in ViewBag for pager
            ViewBag.CurrentQuery = new Dictionary<string, string>()
            {
                { "isActive", isActive.ToString() },
                { "search", search }, { "show", pageSize.ToString() }
            };
            

            var result = await _userService.SearchAsync(_loggedUser.AgentId, _loggedUser.FleetId, null, isActive, search,false,true, pageNumber, pageSize);
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
        [UserPrivilege(Privilege = AgentPrivilegeTypeEnum.AddUpdateUsers)]
        public async Task<IActionResult> Create(string returnURL)
        {
            //await LoadWarehouses(FleetId: _loggedUser.FleetId);
            await LoadWarehousesAndInventories((long)_loggedUser.FleetId, null, false, UserProfile.UserInventoryIds);
            UserView userView = new UserView()
            {
                ExpirationDateText = DateTime.Now.AddYears(1).ToString("yyyy/MM/dd", new CultureInfo("en").DateTimeFormat)
            };

            ViewBag.ReturnURL = returnURL;
            return View(userView);
        }

        // POST: Users/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserView user, string returnURL)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(user.ExpirationDateText))
                {
                    user.ExpirationDate = GPSHelper.StringToDateTime(user.ExpirationDateText);
                }
                user.Role.Name = Roles.agent.ToString();
                user.FleetId = _loggedUser.FleetId;
                user.AgentId = _loggedUser.AgentId;
                user.CreatedBy = _loggedUser.UserId;
                var result = await _userService.SaveAsync(user);
                if (!result.IsSuccess)
                {
                    return View(_viewHelper.GetErrorPage(result.HttpCode));
                }

                return RedirectToAction(nameof(Create), new {user, returnURL}).WithSuccessOptions(_localizer["AddSuccess"], "", _localizer["AddNewUser"], returnURL);
            }
            //await LoadWarehouses(FleetId: _loggedUser.FleetId);
            await LoadWarehousesAndInventories((long)_loggedUser.FleetId, user.InventoriesIds, false, UserProfile.UserInventoryIds);

            ViewBag.ReturnURL = returnURL;
            return View(user);
        }

        // GET: Details/Details/5
        [UserPrivilege(Privilege = AgentPrivilegeTypeEnum.ViewUsers)]
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
            await LoadWarehousesAndInventories((long)_loggedUser.FleetId, result.Data.InventoriesIds, true, UserProfile.UserInventoryIds);
            ViewBag.ReturnURL = returnURL;
            return View(result.Data);
        }

        // GET: Groups/Edit/5
        [UserPrivilege(Privilege = AgentPrivilegeTypeEnum.AddUpdateUsers)]
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

            //await LoadWarehouses(FleetId: _loggedUser.FleetId);
            await LoadWarehousesAndInventories((long)_loggedUser.FleetId, result.Data.InventoriesIds, false, UserProfile.UserInventoryIds);

            if (result.Data.ExpirationDate.HasValue)
            {
                result.Data.ExpirationDateText = result.Data.ExpirationDate.Value.ToString("yyyy/MM/dd", new CultureInfo("en").DateTimeFormat);
            }

            ViewBag.ReturnURL = returnURL;
            return View(result.Data);
        }

        // POST: Groups/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, UserView user, string returnURL)
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

                user.Role.Name = Roles.agent.ToString();
                user.FleetId = _loggedUser.FleetId;
                user.AgentId = _loggedUser.AgentId;
                user.UpdatedBy = _loggedUser.UserId;
                var result = await _userService.SaveAsync(user);
                if (!result.IsSuccess)
                {
                    return View(_viewHelper.GetErrorPage(result.HttpCode));
                }

                return RedirectToAction(nameof(Edit), new { id, user, returnURL }).WithSuccessOptions(_localizer["UpdateSuccess"], "", _localizer["ContinueEdit"], returnURL);
            }

            //await LoadWarehouses(FleetId: user.FleetId);
            await LoadWarehousesAndInventories((long)_loggedUser.FleetId, user.InventoriesIds, false, UserProfile.UserInventoryIds);

            ViewBag.ReturnURL = returnURL;
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
        public async Task<string> ValidateExpirationDate(string ExpirationDate)
        {
            return await _userService.ValidNewUserExpirationDateAsync(GPSHelper.StringToDateTime(ExpirationDate), _loggedUser.UserId);
        }

        [HttpGet]
        public async Task<bool> ValidateEmail(string Email)
        {
            return await _userService.IsEmailExistsAsync(Email);
        }


        [UserPrivilege(Privilege = AgentPrivilegeTypeEnum.EnableUserPrivileges)]
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
