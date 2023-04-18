using GPS.Domain.ViewModels;
using GPS.Services.Users;
using GPS.Web.Admin.AppCode.Helpers;
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

namespace GPS.Web.Admin.Controllers
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
                "Tracker.Lang.Web.Admin.Cookie",
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginView login)
        {
            if (ModelState.IsValid)
            {
                var userResult = await _userService.GetByUserNameAndPasswordAsync(login.UserName, login.Password);
                if (userResult.IsSuccess && userResult.Data != null && userResult.Data.IsAdmin)
                {
                    if (!userResult.Data.IsActive || userResult.Data.ExpirationDate.Value < DateTime.Now)
                    {
                        ModelState.AddModelError(string.Empty, _localizer["AccountDisabled"]);
                        return View(login);
                    }

                    var userPrivilegesTypeIds = await _userService.GetActivePrivilegeTypeIdsAsync(userResult.Data.Id);

                    var claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.NameIdentifier, userResult.Data.Id),
                        new Claim(ClaimTypes.Name, userResult.Data.UserName),
                        new Claim(ClaimTypes.Role, userResult.Data.Role.Name),
                        new Claim(ClaimTypes.GivenName, userResult.Data.Name),
                        new Claim("full_name", userResult.Data.Name),
                        new Claim("user_privileges_type_ids", String.Join(",", userPrivilegesTypeIds.Data)),
                        new Claim("is_admin", userResult.Data.IsAdmin.ToString()),
                        new Claim("is_super_admin", userResult.Data.IsSuperAdmin.ToString()),
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        AllowRefresh = true,
                        IsPersistent = false,
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
    }
}
