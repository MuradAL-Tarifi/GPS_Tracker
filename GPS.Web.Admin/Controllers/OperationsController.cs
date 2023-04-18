using GPS.Domain.DTO;
using GPS.Domain.Views;
using GPS.Helper;
using GPS.Services.EventLogs;
using GPS.Web.Admin.AppCode;
using GPS.Web.Admin.AppCode.Helpers;
using GPS.Web.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace GPS.Web.Admin.Controllers
{
    public class OperationsController : BaseController
    {
        private readonly IEventLogService _eventLogService;
        private readonly IStringLocalizer<OperationsController> _localizer;


        public OperationsController(IEventLogService eventLogService,
            IStringLocalizer<OperationsController> localizer,
            IViewHelper viewHelper, LoggedInUserProfile loggedUser)
            : base(viewHelper, loggedUser)
        {
            _eventLogService = eventLogService;
            _localizer = localizer;

        }

        // GET: Operations
        [UserPrivilege(Privilege = AdminPrivilegeTypeEnum.ViewOperations)]
        public async Task<IActionResult> Index(string type = "", string fromDate = "", string toDate = "", int? page = 1, int? show = 100, string search = "")
        {
            type = type ?? "";
            fromDate = fromDate ?? "";
            toDate = toDate ?? "";
            var pageNumber = page ?? 1;
            var pageSize = show ?? 50;

            // Save current query in ViewBag for pager
            ViewBag.CurrentQuery = new Dictionary<string, string>() { { "type", type.ToString() }, { "fromDate", fromDate.ToString() }, { "toDate", toDate.ToString() }, { "search", search }, { "show", pageSize.ToString() } };

            DateTime? FromDate = null;
            DateTime? ToDate = null;
            if (!string.IsNullOrEmpty(fromDate))
            {
                FromDate = GPSHelper.StringToDateTime(fromDate);
            }
            if (!string.IsNullOrEmpty(toDate))
            {
                ToDate = GPSHelper.StringToDateTime(toDate);
            }

            var result = await _eventLogService.SearchAsync(type.ToLower(), FromDate, ToDate, search, pageNumber, pageSize);
            if (!result.IsSuccess)
            {
                return View(_viewHelper.GetErrorPage(result.HttpCode));
            }

            // Convert result data list to StaticPagedList
            var PagedResult = new StaticPagedList<EventLogView>(result.Data.List, pageNumber, pageSize, result.Data.TotalRecords);

            // Check if request is ajax request
            if (HttpContext.Request.Headers["x-requested-with"] == "XMLHttpRequest")
            {
                return PartialView("_Operations", PagedResult);
            }

            return View(PagedResult);
        }
    }
}
