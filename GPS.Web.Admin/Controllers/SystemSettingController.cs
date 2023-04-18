using GPS.Domain.DTO;
using GPS.Domain.Views;
using GPS.Services.SystemSettings;
using GPS.Web.Admin.AppCode;
using GPS.Web.Admin.AppCode.Helpers;
using GPS.Web.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GPS.Web.Admin.Controllers
{
    public class SystemSettingController : BaseController
    {
        private readonly ISystemSettingService _systemSettingService;
        private readonly IStringLocalizer<SystemSettingController> _localizer;

        public SystemSettingController(ISystemSettingService systemSettingService,
            IViewHelper viewHelper,
            IStringLocalizer<SystemSettingController> localizer, LoggedInUserProfile loggedUser)
            : base(viewHelper, loggedUser)
        {
            _systemSettingService = systemSettingService;
            _localizer = localizer;
        }
        [HttpGet]
        [UserPrivilege(Privilege = AdminPrivilegeTypeEnum.SystemSettings)]
        public async Task<IActionResult> Index()
        {
            var result = await _systemSettingService.LoadSystemSettingAsync();
            if (!result.IsSuccess)
            {
                return View(_viewHelper.GetErrorPage(result.HttpCode));
            }
            return View(result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> AddUpdate(SystemSettingView model)
        {
            if (ModelState.IsValid)
            {
                if(model.Id > 0)
                {
                    model.UpdatedBy = model.UpdatedBy;
                }
                else
                {
                    model.CreatedBy = _loggedUser.UserId;
                }
 
                if (model.LogoPhoto != null)
                {
                    using (var target = new MemoryStream())
                    {
                        model.LogoPhoto.CopyTo(target);
                        model.LogoPhotoByte = target.ToArray();
                    }
                }

                var result = await _systemSettingService.SaveAsync(model);
                return StatusCode((int)result.HttpCode, result);
            }
            return StatusCode((int)HttpCode.BadRequest);
        }
    }
}
