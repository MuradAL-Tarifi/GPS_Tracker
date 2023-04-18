using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using GPS.Domain.DTO;
using GPS.Domain.Views;
using GPS.Helper;
using GPS.Services.Gateways;
using GPS.Web.Admin.AppCode;
using GPS.Web.Admin.AppCode.Extensions.Alerts;
using GPS.Web.Admin.AppCode.Helpers;
using GPS.Web.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using X.PagedList;

namespace GPS.Web.Admin.Controllers
{
    public class GatewayController : BaseController
    {
        private readonly IGatewayService _gatewayService;
        private readonly IStringLocalizer<GatewayController> _localizer;

        public GatewayController(IGatewayService gatewayService,
            IViewHelper viewHelper, IStringLocalizer<GatewayController> localizer, 
            LoggedInUserProfile loggedUser)
            : base(viewHelper, loggedUser)
        {
            _gatewayService = gatewayService;
            _localizer = localizer;
            
        }

        [UserPrivilege(Privilege = AdminPrivilegeTypeEnum.ViewGateways)]
        public async Task<IActionResult> Index(int? page = 1, int? show = 100, string search = "")
        {
            var pageNumber = page ?? 1;
            var pageSize = show ?? 10;

            // Save current query in ViewBag for pager
            ViewBag.CurrentQuery = new Dictionary<string, string>() { { "search", search }, { "show", pageSize.ToString() } };

            var result = await _gatewayService.SearchAsync(SearchString: search, PageNumber: pageNumber, pageSize: pageSize);
            if (!result.IsSuccess)
            {
                return View(_viewHelper.GetErrorPage(result.HttpCode));
            }

            // Convert result data list to StaticPagedList
            var PagedResult = new StaticPagedList<GatewayView>(result.Data.List, pageNumber, pageSize, result.Data.TotalRecords);

            // Check if request is ajax request
            if (HttpContext.Request.Headers["x-requested-with"] == "XMLHttpRequest")
            {
                return PartialView("_Data", PagedResult);
            }

            return View(PagedResult);
        }

        // GET: Gateway/Create
        [UserPrivilege(Privilege = AdminPrivilegeTypeEnum.AddUpdateGateways)]
        public async Task<IActionResult> Create()
        {
            GatewayView gatewayView = new GatewayView()
            {
                ExpirationDateText = DateTime.Now.AddYears(1).ToString("yyyy/MM/dd", new CultureInfo("en").DateTimeFormat),
                ActivationDateText = DateTime.Now.ToString("yyyy/MM/dd", new CultureInfo("en").DateTimeFormat),
            };
            await LoadBrands();
            return View("Create", gatewayView);
        }

        // POST: Gateway/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GatewayView gateway)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(gateway.ExpirationDateText))
                {
                    gateway.ExpirationDate = GPSHelper.StringToDateTime(gateway.ExpirationDateText);
                }

                if (!string.IsNullOrEmpty(gateway.ActivationDateText))
                {
                    gateway.ActivationDate = GPSHelper.StringToDateTime(gateway.ActivationDateText);
                }
                if (!string.IsNullOrEmpty(gateway.SIMCardExpirationDateText))
                {
                    gateway.SIMCardExpirationDate = GPSHelper.StringToDateTime(gateway.SIMCardExpirationDateText);
                }

                gateway.CreatedBy = _loggedUser.UserId;
                var result = await _gatewayService.AddAsync(gateway);
                if (!result.IsSuccess)
                {
                    return View(_viewHelper.GetErrorPage(result.HttpCode));
                }

                return RedirectToAction(nameof(Create)).WithSuccessOptions(_localizer["AddSuccess"], "", _localizer["AddNewGateway"], Url.Action(nameof(Index)));
            }
            await LoadBrands(gateway.BrandId);
            return View(gateway);
        }

        // GET: Gateway/Details/5
        [UserPrivilege(Privilege = AdminPrivilegeTypeEnum.ViewGateways)]
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return View("NotFound");
            }

            var result = await _gatewayService.FindByIdAsync(id);
            if (!result.IsSuccess)
            {
                ViewBag.Errors = result.ErrorList;
                return View(_viewHelper.GetErrorPage(result.HttpCode));
            }
            if (!GPSHelper.IsEmptyDatetime(result.Data.ActivationDate))
            {
                result.Data.ActivationDateText = result.Data.ActivationDate.Value.ToString("yyyy/MM/dd", new CultureInfo("en").DateTimeFormat);
            }
            if (!GPSHelper.IsEmptyDatetime(result.Data.SIMCardExpirationDate))
            {
                result.Data.SIMCardExpirationDateText = result.Data.SIMCardExpirationDate.Value.ToString("yyyy/MM/dd", new CultureInfo("en").DateTimeFormat);
            }
            return View(result.Data);
        }

        // GET: Gateway/Edit/5
        [UserPrivilege(Privilege = AdminPrivilegeTypeEnum.AddUpdateGateways)]
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return View("NotFound");
            }

            var result = await _gatewayService.FindByIdAsync(id);
            if (!result.IsSuccess)
            {
                ViewBag.Errors = result.ErrorList;
                return View(_viewHelper.GetErrorPage(result.HttpCode));
            }

            result.Data.ExpirationDateText = result.Data.ExpirationDate.ToString("yyyy/MM/dd", new CultureInfo("en").DateTimeFormat);

            if (!GPSHelper.IsEmptyDatetime(result.Data.ActivationDate))
            {
                result.Data.ActivationDateText = result.Data.ActivationDate.Value.ToString("yyyy/MM/dd", new CultureInfo("en").DateTimeFormat);
            }
            if (!GPSHelper.IsEmptyDatetime(result.Data.SIMCardExpirationDate))
            {
                result.Data.SIMCardExpirationDateText = result.Data.SIMCardExpirationDate.Value.ToString("yyyy/MM/dd", new CultureInfo("en").DateTimeFormat);
            }
            await LoadBrands(result.Data.BrandId);
            return View(result.Data);
        }

        // POST: Gateway/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(GatewayView gateway)
        {
            if (gateway.Id <= 0)
            {
                return View("NotFound");
            }

            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(gateway.ExpirationDateText))
                {
                    gateway.ExpirationDate = GPSHelper.StringToDateTime(gateway.ExpirationDateText);
                }
                if (!string.IsNullOrEmpty(gateway.ActivationDateText))
                {
                    gateway.ActivationDate = GPSHelper.StringToDateTime(gateway.ActivationDateText);
                }
                if (!string.IsNullOrEmpty(gateway.SIMCardExpirationDateText))
                {
                    gateway.SIMCardExpirationDate = GPSHelper.StringToDateTime(gateway.SIMCardExpirationDateText);
                }
                gateway.UpdatedBy = _loggedUser.UserId;
                var result = await _gatewayService.UpdateAsync(gateway);
                if (!result.IsSuccess)
                {
                    return View(_viewHelper.GetErrorPage(result.HttpCode));
                }

                return RedirectToAction(nameof(Edit)).WithSuccessOptions(_localizer["UpdateSuccess"], "", _localizer["ContinueEdit"], Url.Action(nameof(Index)));
            }
            await LoadBrands(gateway.BrandId);
            return View(gateway);
        }

        // POST: Groups/Delete/5
        [HttpPost, ValidateAntiForgeryToken, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(long ItemId)
        {
            var result = await _gatewayService.DeleteAsync(ItemId, _loggedUser.UserId);
            if (!result.IsSuccess)
            {
                return View(_viewHelper.GetErrorPage(result.HttpCode));
            }

            return RedirectToAction(nameof(Index)).WithSuccess(_localizer["DeleteSuccess"], string.Join("<br>", result.ErrorList));
        }

        [HttpGet]
        public async Task<bool> ValidateName(string Name)
        {
            return await _gatewayService.IsNameExistsAsync(Name);
        }

        [HttpGet]
        public async Task<bool> ValidateIMEI(string IMEI)
        {
            return await _gatewayService.IsIMEIExistsAsync(IMEI);
        }

        public async Task<bool> IsGatewayLinkedToInventory(long GatewayId)
        {
            return await _gatewayService.IsGatewayLinkedToInventoryAsync(GatewayId);
        }
    }
}
