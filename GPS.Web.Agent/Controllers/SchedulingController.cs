using System.Collections.Generic;
using System.Threading.Tasks;
using GPS.Web.Agent.AppCode;
using GPS.Web.Agent.AppCode.Extensions.Alerts;
using GPS.Web.Agent.AppCode.Helpers;
using GPS.Web.Agent.Controllers;
using GPS.Web.Agent.Models;
using GPS.Services;
using GPS.Domain.DTO;
using GPS.Domain.ViewModels;
using GPS.Domain.Views;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using X.PagedList;
using GPS.Resources;
using GPS.Services.ReportsSchedule;

namespace GPS.Web.Agent.Controllers
{
    public class SchedulingController : BaseController
    {
        private readonly IStringLocalizer<SchedulingController> _localizer;
        private readonly IStringLocalizer<SharedResources> _sharedLocalizer;
        protected readonly IReportScheduleService _reportScheduleService;

        public SchedulingController(
            IViewHelper viewHelper,
            IStringLocalizer<SchedulingController> localizer,
            IReportScheduleService reportScheduleService,
            IStringLocalizer<SharedResources> sharedLocalizer,
            LoggedInUserProfile loggedUser) : base(viewHelper, loggedUser)
        {
            _localizer = localizer;
            _reportScheduleService = reportScheduleService;
            _sharedLocalizer = sharedLocalizer;
        }

        [UserPrivilege(Privilege = AgentPrivilegeTypeEnum.ManageReportSchedule)]
        public async Task<IActionResult> Index( long? reportTypeId = null,
            int? page = 1, int? show = 100, string search = "")
        {
            var pageNumber = page ?? 1;
            var pageSize = show ?? 10;

            // Save current query in ViewBag for pager
            ViewBag.CurrentQuery = new Dictionary<string, string>() {
                { "reportTypeId", reportTypeId?.ToString() },
                { "search", search }, { "show", pageSize.ToString() }
            };

            var searchModel = new GenericSearchModel()
            {
                UserId = UserProfile.Id,
                FleetId = UserProfile.FleetId,
                ReportTypeLookupId = reportTypeId,
                SearchString = search,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await _reportScheduleService.SearchAsync(searchModel);
            if (!result.IsSuccess)
            {
                return View(_viewHelper.GetErrorPage(result.HttpCode));
            }

            // Convert result data list to StaticPagedList
            var PagedResult = new StaticPagedList<ReportScheduleView>(result.Data.List, pageNumber, pageSize, result.Data.TotalRecords);

            // Check if request is ajax request
            if (HttpContext.Request.Headers["x-requested-with"] == "XMLHttpRequest")
            {
                return PartialView("_Reports", PagedResult);
            }
            return View(PagedResult);
        }

        [UserPrivilege(Privilege = AgentPrivilegeTypeEnum.ManageReportSchedule)]
        public IActionResult Create(long? id)
        {
            ViewBag.FleetName = UserProfile.FleetName;
            ViewBag.ReportScheduleId = id;
            return View();
        }

        [UserPrivilege(Privilege = AgentPrivilegeTypeEnum.ManageReportSchedule)]
        public IActionResult Details(long? id)
        {
            ViewBag.FleetName = UserProfile.FleetName;
            ViewBag.ReportScheduleId = id;
            return View();
        }

        public async Task<IActionResult> GetReportDetails(long id)
        {
            var result = await _reportScheduleService.GetReportDetailsAsync(id);
            return StatusCode((int)result.HttpCode, result.Data);
        }

        public async Task<IActionResult> GetReportById(long id)
        {
            var result = await _reportScheduleService.GetByIdAsync(id);
            return StatusCode((int)result.HttpCode, result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromBody] ReportOptionsModel reportOptions)
        {
            reportOptions.UserId = UserProfile.Id;
            reportOptions.FleetId = UserProfile.FleetId;
            reportOptions.IsEnglish = IsEnglish;

            var result = await _reportScheduleService.SaveAsync(reportOptions);

            return StatusCode((int)result.HttpCode, result.Data);
        }

        [HttpPost, ValidateAntiForgeryToken, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(long ItemId)
        {
            var result = await _reportScheduleService.DeleteAsync(ItemId, UserProfile.Id);
            if (!result.IsSuccess)
            {
                return View(_viewHelper.GetErrorPage(result.HttpCode));
            }
            return RedirectToAction(nameof(Index)).WithSuccess(_sharedLocalizer["DeleteSuccess"], "");
        }

        [HttpPost]
        public async Task<IActionResult> Activate(long id)
        {
            var result = await _reportScheduleService.ActiveStatusAsync(id, true, UserProfile.Id);
            return StatusCode((int)result.HttpCode, result);
        }

        [HttpPost]
        public async Task<IActionResult> DeActivate(long id)
        {
            var result = await _reportScheduleService.ActiveStatusAsync(id, false, UserProfile.Id);
            return StatusCode((int)result.HttpCode, result);
        }

    }
}
