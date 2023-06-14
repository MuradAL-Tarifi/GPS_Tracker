using GPS.Domain.DTO;
using GPS.Domain.Views;
using GPS.Services.Lookups;
using GPS.Services.Sensors;
using GPS.Web.Admin.AppCode;
using GPS.Web.Admin.AppCode.Extensions.Alerts;
using GPS.Web.Admin.AppCode.Helpers;
using GPS.Web.Admin.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace GPS.Web.Admin.Controllers
{
    public class SensorsController : BaseController
    {
        private readonly ISensorService _SensorService;
        private readonly IStringLocalizer<SensorsController> _localizer;
        private readonly ILookupsService _lookupsService;


        public SensorsController(ISensorService SensorService, IViewHelper viewHelper,
            IStringLocalizer<SensorsController> localizer, LoggedInUserProfile loggedUser,
            ILookupsService lookupsService)
            : base(viewHelper, loggedUser)
        {
            _SensorService = SensorService;
            _localizer = localizer;
            _lookupsService = lookupsService;
        }

        // GET: Sensors
        [UserPrivilege(Privilege = AdminPrivilegeTypeEnum.ViewSensors)]
        public async Task<IActionResult> Index(int? SensorStatus,int? BrandId = null, int? page = 1, int? show = 100, string search = "", string returnURL = "")
        {
            if (!string.IsNullOrEmpty(returnURL))
            {
                return Redirect(returnURL);
            }

            BrandId = BrandId == 0 ? null : BrandId;
            var pageNumber = page ?? 1;
            var pageSize = show ?? 10;
            SensorStatus = SensorStatus < 0 ? null : SensorStatus;

            // Save current query in ViewBag for pager
            ViewBag.CurrentQuery = new Dictionary<string, string>() { 
                { "BrandId", BrandId.ToString() },
                { "SensorStatus", SensorStatus.ToString() },
                { "search", search },
                { "show", pageSize.ToString() } };


            await LoadBrands(BrandId);

            var result = await _SensorService.SearchAsync(null,SensorStatus, BrandId, search, pageNumber, pageSize);
            if (!result.IsSuccess)
            {
                return View(_viewHelper.GetErrorPage(result.HttpCode));
            }

            // Convert result data list to StaticPagedList
            var PagedResult = new StaticPagedList<SensorView>(result.Data.List, pageNumber, pageSize, result.Data.TotalRecords);

            // Check if request is ajax request
            if (HttpContext.Request.Headers["x-requested-with"] == "XMLHttpRequest")
            {
                return PartialView("_data", PagedResult);
            }

            return View(PagedResult);
        }

        // GET: Sensors/Create
        [UserPrivilege(Privilege = AdminPrivilegeTypeEnum.AddUpdateSensors)]
        public async Task<IActionResult> Create(long? id, string returnURL, int? inventoryId)
        {
            if (id == null)
            {
                ViewBag.ReturnURL = returnURL;
                return View(new SensorView());
            }
            else
            {
                var result = await _SensorService.FindbyIdAsync(id);
                if (!result.IsSuccess)
                {
                    ViewBag.Errors = result.ErrorList;
                    return View(_viewHelper.GetErrorPage(result.HttpCode));
                }

                ViewBag.ReturnURL = returnURL;
                ViewBag.inventoryId = inventoryId;
                return View(result.Data);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SensorView model, long? inventoryId)
        {
            ReturnResult<bool> result;

            if (model.Id > 0)
            {
                model.UpdatedBy = _loggedUser.UserId;
                result = await _SensorService.SaveAsync(model);
            }
            else
            {
                model.CreatedBy = _loggedUser.UserId;
                result = await _SensorService.SaveRangeAsync(model.SensorsList, model.CreatedBy, inventoryId);
            }

            if(inventoryId > 0)
            {
                RedirectToAction(nameof(Index), "Inventory");
            }
            return StatusCode((int)result.HttpCode, result);
        }


        // GET: Details/Edit/5
        [UserPrivilege(Privilege = AdminPrivilegeTypeEnum.ViewSensors)]
        public async Task<IActionResult> Details(long? id, string returnURL)
        {
            if (id == null)
            {
                return View("NotFound");
            }

            var result = await _SensorService.FindbyIdAsync(id);
            if (!result.IsSuccess)
            {
                ViewBag.Errors = result.ErrorList;
                return View(_viewHelper.GetErrorPage(result.HttpCode));
            }

            ViewBag.ReturnURL = returnURL;
            return View(result.Data);
        }

        // GET: Sensors/Edit/5
        [UserPrivilege(Privilege = AdminPrivilegeTypeEnum.AddUpdateSensors)]
        public async Task<IActionResult> Edit(long? id, string returnURL)
        {
            if (id == null)
            {
                return View("NotFound");
            }

            var result = await _SensorService.FindbyIdAsync(id);
            if (!result.IsSuccess)
            {
                ViewBag.Errors = result.ErrorList;
                return View(_viewHelper.GetErrorPage(result.HttpCode));
            }

            await LoadBrands(result.Data.BrandId);

            ViewBag.ReturnURL = returnURL;
            return View(result.Data);
        }

        // POST: Sensors/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, SensorView Sensor, string ReturnURL)
        {
            if (id != Sensor.Id)
            {
                return View("NotFound");
            }
            if (ModelState.IsValid)
            {
                Sensor.UpdatedBy = _loggedUser.UserId;
                var result = await _SensorService.SaveAsync(Sensor);
                if (!result.IsSuccess)
                {
                    return View(_viewHelper.GetErrorPage(result.HttpCode));
                }
                return RedirectToAction(nameof(Edit), new { ReturnURL }).WithSuccessOptions(_localizer["UpdateSuccess"], "", _localizer["ContinueEdit"], ReturnURL);
            }
            await LoadBrands(Sensor.BrandId);
            ViewBag.ReturnURL = ReturnURL;
            return View(Sensor);
        }

        // POST: Sensors/Delete/5
        [HttpPost, ValidateAntiForgeryToken, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(long ItemId, string returnURL)
        {
            var result = await _SensorService.DeleteAsync(ItemId, _loggedUser.UserId);
            if (!result.IsSuccess)
            {
                return View(_viewHelper.GetErrorPage(result.HttpCode));
            }
            return RedirectToAction(nameof(Index), new { returnURL }).WithSuccess(_localizer["DeleteSuccess"], string.Join("<br>", result.ErrorList));
        }

        public async Task<bool> ValidateSensor(string SensorSN)
        {
            return await _SensorService.IsSensorExistsAsync(SensorSN);
        }

        [ActionName("ExportDefaultExcelFile")]
        public async Task<IActionResult> DownloadDefaultExcelFile()
        {
           var result = await _lookupsService.GetDefaultExcelFileToAddNewSensorsAsync();
           return File(result.Data, "application/octetstream", "DefaultExcelFile.xlsx");
        }


        [HttpPost]
        public async Task<string> UploadExcelFile(IFormFile ExcelFile)
        {
            bool isSuccess = false;
            if (ExcelFile != null)
            {
                if (System.IO.Path.GetExtension(ExcelFile.FileName) == ".xlsx")
                {
                    var resultSensors =  _lookupsService.ImportSensorsFromExcelAsync(ExcelFile);
                    if (resultSensors.IsSuccess)
                    {
                        var resultAddRangeSensors = await _SensorService.SaveRangeAsync(resultSensors.Data,_loggedUser.UserId,null);
                        isSuccess = resultAddRangeSensors.IsSuccess;
                    }
                }
            }
            return JsonConvert.SerializeObject(isSuccess);
        }
        [HttpGet]
        public async Task<SelectList> GetInventorySensorsByInventoryId(long InventoryId)
        {
            return await GetSensoresBase(InventoryId);
        }
    }
}
