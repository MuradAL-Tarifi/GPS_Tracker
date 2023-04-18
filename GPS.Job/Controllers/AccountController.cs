using GPS.Domain.DTO;
using GPS.Domain.ViewModels;
using GPS.Services.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;


namespace GPS.Job.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        public readonly IHttpContextAccessor _context;
        public IWebHostEnvironment _hostEnvironment { get; }

        public AccountController(
            IUserService userService,
            IHttpContextAccessor context,
            IWebHostEnvironment hostEnvironment)
        {
            _userService = userService;
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            return RedirectToAction(nameof(Login));
        }

        public IActionResult Login()
        {
            //if (User.Identity.IsAuthenticated)
            //{
            if (_hostEnvironment.IsDevelopment())
            {
                return Redirect("/hangfire");
            }
            else
            {
                return Redirect("/GPSJob/hangfire");
            }
            //}
            //return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginView login)
        {
            if (string.IsNullOrEmpty(login.CaptchaValue))
            {
                ModelState.AddModelError(string.Empty, "Enter Captcha");
            }
            else if (login?.CaptchaValue != login?.CaptchaCreatedValue.Replace(" ", String.Empty))
            {
                ModelState.AddModelError(string.Empty, "رمز التحقق غير صحيح");
            }

            if (ModelState.IsValid)
            {
                var userResult = await _userService.GetByUserNameAndPasswordAsync(login.UserName, login.Password);
                if (userResult.IsSuccess && userResult.Data != null && userResult.Data.IsAdmin)
                {
                    if (!userResult.Data.IsActive || userResult.Data.ExpirationDate.Value < DateTime.Now)
                    {
                        ModelState.AddModelError(string.Empty, "الحساب غير مفعل");
                        return View(login);
                    }

                    var userPrivilegesTypeIds = await _userService.GetActivePrivilegeTypeIdsAsync(userResult.Data.Id);
                    if (userPrivilegesTypeIds.Data.Contains((int)AdminPrivilegeTypeEnum.ManageJobs))
                    {
                        var claims = new List<Claim>()
                        {
                            new Claim(ClaimTypes.NameIdentifier, userResult.Data.Id),
                            new Claim(ClaimTypes.Name, userResult.Data.UserName),
                            new Claim(ClaimTypes.Role, userResult.Data.Role.Name),
                            new Claim(ClaimTypes.GivenName, userResult.Data.Name),
                            new Claim("full_name", userResult.Data.Name),
                            new Claim("user_privileges_type_ids", String.Join(",", userPrivilegesTypeIds.Data))
                        };

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                        var authProperties = new AuthenticationProperties
                        {
                            AllowRefresh = true,
                            IsPersistent = false,
                            RedirectUri = "home/index",
                        };

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                        if (_hostEnvironment.IsDevelopment())
                        {
                            return Redirect("/hangfire");
                        }
                        else
                        {
                            return Redirect("/GPSJob/hangfire");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "ليس لديك الصلاحية");
                        return View(login);
                    }

                }
                else if (userResult.HttpCode == HttpCode.ServerError)
                {
                    ModelState.AddModelError(string.Empty, userResult.ErrorList.FirstOrDefault());
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "خطأ في اسم المستخدم أو كلمة المرور");
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
    }
}
