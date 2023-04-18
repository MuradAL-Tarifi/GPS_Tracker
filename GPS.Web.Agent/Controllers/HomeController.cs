using GPS.Domain.ViewModels;
using GPS.Services.Lookups;
using GPS.Web.Agent.AppCode.Helpers;
using GPS.Web.Agent.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace GPS.Web.Agent.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILookupsService lookupsService;

        public HomeController(
            ILookupsService lookupsService,
            IViewHelper viewHelper,
            LoggedInUserProfile loggedUser)
            : base(viewHelper, loggedUser)
        {
            this.lookupsService = lookupsService;
        }

        public async Task<IActionResult> Index()
        {
            var dashboard = new AgentDashboardView();

            var result = await lookupsService.GetAgentDasboardAsync(UserProfile.Id);
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
