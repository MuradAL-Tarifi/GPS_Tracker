using GPS.Domain.DTO;
using GPS.Domain.ViewModels;
using GPS.Domain.Views;
using GPS.Services.Fleets;
using GPS.Services.SystemSettings;
using GPS.Web.Agent.AppCode;
using GPS.Web.Agent.AppCode.Helpers;
using GPS.Web.Agent.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GPS.Web.Agent.Controllers
{
    public class CompanySettingController : BaseController
    {
        private readonly IFleetService _fleetService;
        private readonly IStringLocalizer<CompanySettingController> _localizer;

        public CompanySettingController(IFleetService fleetService,
            IViewHelper viewHelper,
            IStringLocalizer<CompanySettingController> localizer, LoggedInUserProfile loggedUser)
            : base(viewHelper, loggedUser)
        {
            _fleetService = fleetService;
            _localizer = localizer;
        }
        [HttpGet]
        [UserPrivilege(Privilege = AgentPrivilegeTypeEnum.UpdateCompanySettings)]
        public async Task<IActionResult> Index()
        {
            var result = await _fleetService.LoadCompanySettingsAysnc((long)_loggedUser.FleetId);
            if (!result.IsSuccess)
            {
                return View(_viewHelper.GetErrorPage(result.HttpCode));
            }
            return View(result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> AddUpdate(CompanySettingViewModel model)
        {
            if (ModelState.IsValid)
            {
 
                if (model.LogoPhoto != null)
                {
                    using (var target = new MemoryStream())
                    {
                        model.LogoPhoto.CopyTo(target);
                        model.LogoPhotoByte = target.ToArray();
                    }
                    model.LogoPhotoExtention = Path.GetExtension(model.LogoPhoto.FileName);
                }

                var result = await _fleetService.UpdateCompanySettingsAysnc((long)_loggedUser.FleetId,_loggedUser.UserId,model);
                return StatusCode((int)result.HttpCode, result);
            }
            return StatusCode((int)HttpCode.BadRequest);
        }
    }
}
