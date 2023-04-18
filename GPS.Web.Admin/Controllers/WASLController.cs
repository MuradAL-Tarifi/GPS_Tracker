using GPS.Domain.DTO;
using GPS.Domain.Views;
using GPS.Integration.WaslServices.OperatingCompanies;
using GPS.Integration.WaslServices.Warehouse;
using GPS.Services.Fleets;
using GPS.Web.Admin.AppCode;
using GPS.Web.Admin.AppCode.Extensions.Alerts;
using GPS.Web.Admin.AppCode.Helpers;
using GPS.Web.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace GPS.Web.Admin.Controllers
{
    public class WASLController : BaseController
    {
        private readonly IStringLocalizer<WASLController> _localizer;
        private readonly IFleetService _fleetService;
        private readonly ILogger<WASLController> _logger;
        private readonly IWaslOperatingCompaniesService _waslOperatingCompaniesService;
        private readonly IWaslWarehouseService _waslWarehouseService;

        public WASLController(
            IViewHelper viewHelper,
            IStringLocalizer<WASLController> localizer,
            ILogger<WASLController> logger,
            IFleetService fleetService,
            IWaslOperatingCompaniesService waslOperatingCompaniesService,
            IWaslWarehouseService waslWarehouseService,
            LoggedInUserProfile loggedUser)
            : base(viewHelper, loggedUser)
        {
            _fleetService = fleetService;
            _localizer = localizer;
            _logger = logger;
            _waslOperatingCompaniesService = waslOperatingCompaniesService;
            _waslWarehouseService = waslWarehouseService;
        }
        [UserPrivilege(Privilege = AdminPrivilegeTypeEnum.WASLInquiries)]
        public IActionResult Index()
        {
            return RedirectToAction(nameof(OperatingCompanies));
        }
        [UserPrivilege(Privilege = AdminPrivilegeTypeEnum.WASLInquiries)]
        public async Task<IActionResult> OperatingCompanies(long? fleetId = null)
        {
            await LoadFleets(null, fleetId, true);

            ViewBag.FleetId = fleetId;

            if (fleetId != null)
            {
                var fleet = await _fleetService.FindByIdAsync(fleetId);
                if (fleet.IsSuccess && fleet.Data.FleetDetails != null)
                {
                    var result = await _waslOperatingCompaniesService.InquiryAsync(fleet.Data.FleetDetails.IdentityNumber,
                        fleet.Data.FleetDetails.CommercialRecordNumber, fleet.Data.FleetDetails.ActivityType);
                    if (result.IsSuccess)
                    {
                        ViewBag.JsonResponse = JValue.Parse(result.Data.Response).ToString(Formatting.Indented);
                        return View(result.Data);
                    }
                    else
                    {
                        return RedirectToAction(nameof(OperatingCompanies)).WithWarning("", string.Join(",", result.ErrorList));
                    }
                }
            }

            return View();
        }
        [UserPrivilege(Privilege = AdminPrivilegeTypeEnum.WASLInquiries)]
        public async Task<IActionResult> Warehouses(long? fleetId = null)
        {
            await LoadFleets(null, fleetId, true);

            ViewBag.FleetId = fleetId;

            if (fleetId != null)
            {
                var fleet = await _fleetService.FindByIdAsync(fleetId);
                if (fleet.IsSuccess && fleet.Data.FleetDetails != null)
                {
                    var result = await _waslWarehouseService.InquiryAsync(fleet.Data.FleetDetails.ReferanceNumber);
                    if (result.IsSuccess)
                    {
                        ViewBag.JsonResponse = JValue.Parse(result.Data.Response).ToString(Formatting.Indented);
                        return View(result.Data);
                    }
                    else
                    {
                        return RedirectToAction(nameof(OperatingCompanies)).WithWarning("", string.Join(",", result.ErrorList));
                    }
                }
            }

            return View();
        }


    }
}
