using GPS.Domain.DTO;
using GPS.Domain.Views;
using GPS.Services.CustomAlerts;
using GPS.Web.Agent.AppCode;
using GPS.Web.Agent.AppCode.Helpers;
using GPS.Web.Agent.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace GPS.Web.Agent.Controllers
{
    public class CustomAlertController : BaseController
    {
        private readonly ICustomAlertService _customAlertService;
        private readonly AppSettings _appSettings;
        public CustomAlertController(
            ICustomAlertService customAlertService,
            IViewHelper viewHelper,
            AppSettings appSettings,
            LoggedInUserProfile loggedUser) : base(viewHelper, loggedUser)
        {
            _customAlertService = customAlertService;
            _appSettings = appSettings;
        }

        [UserPrivilege(Privilege = AgentPrivilegeTypeEnum.ManageCustomAlerts)]
        public async Task<IActionResult> Index(long? warehouseId = null, long? inventoryId = null, int? isActive = null, int? page = 1,
            int? show = 100, string search = "")
        {
            warehouseId = warehouseId <= 0 ? null : warehouseId;
            inventoryId = inventoryId <= 0 ? null : inventoryId;
            isActive = isActive < 0 ? null : isActive;
            var pageNumber = page ?? 1;
            var pageSize = show ?? 10;

            // Save current query in ViewBag for pager
            ViewBag.CurrentQuery = new Dictionary<string, string>()
            {
                { "warehouseId", warehouseId.ToString() },
                { "inventoryId", inventoryId.ToString() },
                { "isActive", isActive.ToString() },
                { "search", search }, { "show", pageSize.ToString() }
            };
            await LoadWarehouses();
            await LoadInventoriesByUserId();
            ViewBag.MinInterval = _appSettings.CustomAlerts.MinIntervalMinutes;
            var result = await _customAlertService.SearchAsync((long)_loggedUser.FleetId,warehouseId, inventoryId, isActive, SearchString: search, PageNumber: pageNumber, PageSize: pageSize);
            if (!result.IsSuccess)
            {
                return View(_viewHelper.GetErrorPage(result.HttpCode));
            }

            // Convert result data list to StaticPagedList
            var PagedResult = new StaticPagedList<CustomAlertView>(result.Data.List, pageNumber, pageSize, result.Data.TotalRecords);

            // Check if request is ajax request
            if (HttpContext.Request.Headers["x-requested-with"] == "XMLHttpRequest")
            {
                return PartialView("_customAlert", PagedResult);
            }

            return View(PagedResult);
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromBody] CustomAlertView model)
        {
            var result = new ReturnResult<bool>();
            if (model.Id > 0)
            {
                model.UpdatedBy = UserProfile.Id;
                model.FleetId = (long)_loggedUser.FleetId;
                result = await _customAlertService.UpdateAsync(model);
            }
            else
            {
                model.CreatedBy = UserProfile.Id;
                model.FleetId = (long)_loggedUser.FleetId;
                result = await _customAlertService.AddAsync(model);
            }

            return StatusCode((int)result.HttpCode, result);
        }

        [HttpGet]
        public async Task<IActionResult> GetCustomAlertById(long customAlertId)
        {
            var result = await _customAlertService.CustomAlertByIdAsync(customAlertId);
            return StatusCode((int)result.HttpCode, result);
        }

        [Route("/CustomAlert/Delete/{id}"), HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] long id)
        {
            var result = await _customAlertService.DeleteAsync(id, UserProfile.Id);
            return StatusCode((int)result.HttpCode, result);
        }
    }
}
