using GPS.Domain.DTO;
using GPS.Domain.ViewModels;
using GPS.Domain.Views;
using GPS.Services.Users;
using GPS.Web.Agent.AppCode.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace GPS.Web.Agent.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly IStringLocalizer<AccountController> _localizer;
        private readonly IViewHelper _viewHelper;
        public AccountController(IUserService userService, IStringLocalizer<AccountController> localizer, IViewHelper viewHelper)
        {
            _userService = userService;
            _localizer = localizer;
            _viewHelper = viewHelper;
        }

        public IActionResult Index()
        {
            return RedirectToAction(nameof(Login));
        }

        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        public async Task<IActionResult> SignInUser(string Username, string Password)
        {
            if (!string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password))
            {
                #region login
                var login = new LoginView() { UserName = Username, Password = Password };
                var userResult = await _userService.GetByUserNameAndPasswordAsync(login.UserName, login.Password);
                if (userResult.IsSuccess && userResult.Data != null && userResult.Data.Role.Name.Equals(Roles.agent.ToString()))
                {
                    if (!userResult.Data.IsActive || userResult.Data.ExpirationDate.Value < DateTime.Now)
                    {
                        ModelState.AddModelError(string.Empty, _localizer["AccountDisabled"]);
                        return RedirectToAction(nameof(Login), new { Username = Username, Password = Password });
                    }

                    var userPrivilegesTypeIds = await _userService.GetActivePrivilegeTypeIdsAsync(userResult.Data.Id);
                    var resultUserInventories = await _userService.GetUserInventoriesAndWarehouesAsync(userResult.Data.Id);
                    var userWarehouses = UserWarehouses(resultUserInventories.Data);
                    var userInventories = UserInventories(resultUserInventories.Data);


                    var claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.NameIdentifier, userResult.Data.Id),
                        new Claim(ClaimTypes.Name, userResult.Data.UserName),
                        new Claim(ClaimTypes.Role, userResult.Data.Role.Name),
                        new Claim(ClaimTypes.GivenName, userResult.Data.Name),
                        new Claim("full_name", userResult.Data.Name),
                        new Claim("agent_id", userResult.Data.AgentId?.ToString()),
                        new Claim("fleet_id", userResult.Data.FleetId?.ToString()),
                        new Claim("fleet_name", userResult.Data.Fleet.Name),
                        new Claim("fleet_name_en", userResult.Data.Fleet.NameEn),
                        new Claim("user_privileges_type_ids", String.Join(",", userPrivilegesTypeIds.Data)),
                        new Claim("user_warehouses", JsonConvert.SerializeObject(userWarehouses)),
                        new Claim("user_inventories", String.Join(",", userInventories)),
                        new Claim("sub_admin_agent", userResult.Data.IsSubAdminAgent.ToString()),
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        AllowRefresh = true,
                        IsPersistent = true,
                        RedirectUri = "home/index",
                    };
                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                    return RedirectToAction("Index", "Home");
                }
                #endregion
            }
            return RedirectToAction(nameof(Login));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult ChangeLanguage()
        {
            string culture;
            if (Thread.CurrentThread.CurrentCulture.Name.Equals("en-US"))
            {
                culture = "ar-SA";
            }
            else
            {
                culture = "en-US";
            }

            Response.Cookies.Append(
                "Ewe.Tracker.Web.Agent.Lang.Cookie",
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost,ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginView login)
        {

            if (ModelState.IsValid)
            {
                var userResult = await _userService.GetByUserNameAndPasswordAsync(login.UserName, login.Password);
                if (userResult.IsSuccess && userResult.Data != null && userResult.Data.Role.Name.Equals(Roles.agent.ToString()))
                {
                    if (!userResult.Data.IsActive || userResult.Data.ExpirationDate.Value < DateTime.Now)
                    {
                        ModelState.AddModelError(string.Empty, _localizer["AccountDisabled"]);
                        return View(login);
                    }

                    var userPrivilegesTypeIds = await _userService.GetActivePrivilegeTypeIdsAsync(userResult.Data.Id);
                    var resultUserInventories = await _userService.GetUserInventoriesAndWarehouesAsync(userResult.Data.Id);
                    var userWarehouses = UserWarehouses(resultUserInventories.Data);
                    var userInventories = UserInventories(resultUserInventories.Data);
                    

                    var claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.NameIdentifier, userResult.Data.Id),
                        new Claim(ClaimTypes.Name, userResult.Data.UserName),
                        new Claim(ClaimTypes.Role, userResult.Data.Role.Name),
                        new Claim(ClaimTypes.GivenName, userResult.Data.Name),
                        new Claim("full_name", userResult.Data.Name),
                        new Claim("agent_id", userResult.Data.AgentId?.ToString()),
                        new Claim("fleet_id", userResult.Data.FleetId?.ToString()),
                        new Claim("fleet_name", userResult.Data.Fleet.Name),
                        new Claim("fleet_name_en", userResult.Data.Fleet.NameEn),
                        new Claim("user_privileges_type_ids", String.Join(",", userPrivilegesTypeIds.Data)),
                        new Claim("user_warehouses", JsonConvert.SerializeObject(userWarehouses)),
                        new Claim("user_inventories", String.Join(",", userInventories)),
                        new Claim("sub_admin_agent", userResult.Data.IsSubAdminAgent.ToString()),
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        AllowRefresh = true,
                        IsPersistent = true,
                        RedirectUri = "home/index",
                    };

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                    return RedirectToAction("Index", "Home");
                }
                else if (userResult.HttpCode == Domain.DTO.HttpCode.ServerError)
                {
                    ModelState.AddModelError(string.Empty, userResult.ErrorList.FirstOrDefault());
                }
                else
                {
                    ModelState.AddModelError(string.Empty, _localizer["Invalid"]);
                }
            }

            return View(login);
        }


        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public async Task<string> GetSystemSettings()
        {
            var data = await _viewHelper.GetSystemSettings();
            return JsonConvert.SerializeObject(data);
        }

        public List<LookupModel> UserWarehouses(List<InventoryView> inventories)
        {
            List<LookupModel> warehoueses = new List<LookupModel>();
            foreach(var inventory in inventories)
            {
                if (!warehoueses.Any(x => x.Id == inventory.WarehouseId))
                {
                    warehoueses.Add(new LookupModel()
                    {
                        Id = inventory.Warehouse.Id,
                        Name = inventory.Warehouse.Name
                    });
                }
            }
            return warehoueses.ToList();
        }
        public List<long> UserInventories(List<InventoryView> inventories)
        {
            List<long> inventoryIds = new List<long>();
            foreach (var inventory in inventories)
            {
                if (!inventoryIds.Any(x => x == inventory.Id))
                {
                    inventoryIds.Add(inventory.Id);
                }
            }
            return inventoryIds;
        }
    }
}
