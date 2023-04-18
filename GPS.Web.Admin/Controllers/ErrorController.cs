using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GPS.Web.Admin.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            switch (statusCode)
            {
                case 400:
                    return View("BadRequest");
                case 401:
                    return View("Unauthorized");
                case 404:
                    return View("NotFound");
                case 500:
                    return View("ServerError");
                default:
                    break;
            }
            return View("NotFound");
        }
    }
}
