using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GPS.Job.Controllers
{
    public class HomeController : BaseController
    {
        public IWebHostEnvironment _hostEnvironment { get; }
        public HomeController( IWebHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            if (_hostEnvironment.IsDevelopment())
            {
                return Redirect("/hangfire");
            }
            else
            {
                return Redirect("/GPSJob/hangfire");
            }
        }
    }
}
