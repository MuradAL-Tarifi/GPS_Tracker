using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GPS.Web.Agent.Controllers
{
    public class HeaderController : Controller
    {
        public ActionResult LogOut()
        {
            return Redirect("/");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult SetLanguage()
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
                "GPS.Lang.Web.Agent.Cookie",
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}
