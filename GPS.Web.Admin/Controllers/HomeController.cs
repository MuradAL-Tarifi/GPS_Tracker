
using GPS.Domain.ViewModels;
using GPS.Services.Lookups;
using GPS.Web.Admin.AppCode.Helpers;
using GPS.Web.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace GPS.Web.Admin.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILookupsService lookupsService;

        public HomeController(IViewHelper viewHelper, ILookupsService lookupsService,
            LoggedInUserProfile loggedUser)
           : base(viewHelper, loggedUser)
        {
            this.lookupsService = lookupsService;
        }

        public async Task<IActionResult> Index()
        {
            var dashboard = new AdminDashboardView();
            var result = await lookupsService.GetAdminDasboardAsync();
            if (result.IsSuccess)
            {
                dashboard = result.Data;
            }
            return View(dashboard);
        }

        public IActionResult Privacy()
        {
            return View();
        }


    }
}
