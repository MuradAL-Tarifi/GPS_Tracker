
using DXApplicationFDA.Infra.Services;
using GPS.API.Proxy;
using GPS.Domain.DTO;
using GPS.Domain.ViewModels;
using GPS.Domain.Views;
using GPS.Helper;
using GPS.Services.Alerts;
using GPS.Services.Lookups;
using GPS.Services.Sensors;
using GPS.Web.Agent.AppCode;
using GPS.Web.Agent.AppCode.Helpers;
using GPS.Web.Agent.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace GPS.Web.Agent.Controllers
{
    public class ReportsController : BaseController
    {
        private readonly IInventoryHistoryReportApiProxy _inventoryHistoryReportApiProxy;
        private readonly IOnlineInventoryHistoryApiProxy _onlineInventoryHistoryApiProxy;
        private readonly IAlertApiProxy _alertApiProxy;
        private readonly ISensorService _SensorService;
        public ReportsController(
            IInventoryHistoryReportApiProxy inventoryHistoryReportApiProxy,
            IOnlineInventoryHistoryApiProxy onlineInventoryHistoryApiProxy,
            IViewHelper viewHelper,
            LoggedInUserProfile loggedUser,
            ISensorService SensorService,
            IAlertApiProxy alertApiProxy,
            ILookupsService lookupsService) : base(viewHelper, loggedUser, lookupsService)
        {
            _inventoryHistoryReportApiProxy = inventoryHistoryReportApiProxy;
            _onlineInventoryHistoryApiProxy = onlineInventoryHistoryApiProxy;
            _alertApiProxy = alertApiProxy;
            _SensorService = SensorService;
        }


        [UserPrivilege(Privilege = AgentPrivilegeTypeEnum.ViewInventoryHistoryReport)]
        public IActionResult Inventory()
        {
            return View();
        }


        [UserPrivilege(Privilege = AgentPrivilegeTypeEnum.ShowWarehouseMenuItem)]
        public IActionResult Warehouse()
        {
            return View();
        }


        [UserPrivilege(Privilege = AgentPrivilegeTypeEnum.ViewInventoryHistoryReport)]
        public async Task<IActionResult> InventoryHistoryReport(long inventoryId, string fromDate, string toDate)
        {
            var result = await _inventoryHistoryReportApiProxy.InventoryHistoryReport(inventoryId, fromDate, toDate);
            return StatusCode((int)result.HttpCode, result.Data);
        }

        [UserPrivilege(Privilege = AgentPrivilegeTypeEnum.ViewInventoryHistoryReport)]
        [HttpGet("[Controller]/InventoryReport/TemperatureAndHumidityInventoryHistoryReport")]
        public async Task<IActionResult> TemperatureAndHumidityInventoryHistoryReport(long inventoryId, string sensorsSN, string fromDate, string toDate, string groupUpdatesByType = "", int? groupUpdatesValue = null)
        {
            var result = await _inventoryHistoryReportApiProxy.TemperatureAndHumidityInventoryHistoryReport(inventoryId, sensorsSN, fromDate, toDate, groupUpdatesByType, groupUpdatesValue, IsEnglish);
            return StatusCode((int)result.HttpCode, result.Data);
        }

        [UserPrivilege(Privilege = AgentPrivilegeTypeEnum.ViewInventoryHistoryReport)]
        [HttpGet("[Controller]/InventoryReport/PrintPDF/{inventoryId}")]
        public async Task<FileResult> InventoryHistoryReportPrintPDF(long inventoryId, string sensorsSN, string fromDate, string toDate, string groupUpdatesByType = "", int? groupUpdatesValue = null)
        {
            var result = await _inventoryHistoryReportApiProxy.InventoryHistoryReportPDF(inventoryId, sensorsSN, fromDate, toDate, groupUpdatesByType, groupUpdatesValue, IsEnglish);
            return File(result.Data, "application/pdf", "Report.pdf");
        }


        [UserPrivilege(Privilege = AgentPrivilegeTypeEnum.ViewWarehousesHistoryReport)]
        public async Task<IActionResult> InventorySensor(long? warehouseId, long inventoryId, string sensorSerial, string fromDate, string toDate,
            int? page = 1, int? show = 100, string groupUpdatesByType = "", int? groupUpdatesValue = null)
        {
            warehouseId = warehouseId == 0 ? null : warehouseId;
            var pageNumber = page ?? 1;
            var pageSize = show ?? 100;

            warehouseId = await LoadWarehouses(warehouseId);
            await LoadInventories(warehouseId.Value, inventoryId);
            await LoadInventorySensorsBySerial(inventoryId, sensorSerial);

            var PagedResult = new StaticPagedList<TemperatureAndHumiditySensorReportMonthHistory>(new List<TemperatureAndHumiditySensorReportMonthHistory>(), pageNumber, pageSize, 0);
            if (!warehouseId.HasValue || string.IsNullOrEmpty(fromDate) || string.IsNullOrEmpty(toDate))
            {
                return View(PagedResult);
            }

            // Save current query in ViewBag for pager
            ViewBag.CurrentQuery = new Dictionary<string, string>()
            {
                { "warehouseId", warehouseId.ToString() },
                { "inventoryId", inventoryId.ToString() },
                { "sensorSerial", sensorSerial.ToString() },
                { "fromDate", fromDate },
                { "toDate", toDate },
                { "show", pageSize.ToString() },
                { "groupUpdatesByType", groupUpdatesByType },
                { "groupUpdatesValue", groupUpdatesValue.ToString() }
            };
            ViewBag.Action = "InventorySensor";

            var result = await _inventoryHistoryReportApiProxy.PagedTemperatureAndHumidityHistory(inventoryId, sensorSerial, fromDate, toDate, pageNumber, pageSize, groupUpdatesByType, groupUpdatesValue, IsEnglish);
            if (!result.IsSuccess && result.HttpCode != HttpCode.NotFound)
            {
                return View(_viewHelper.GetErrorPage(result.HttpCode));
            }

            // Convert result data list to StaticPagedList
            if (result.IsSuccess)
            {
                PagedResult = new StaticPagedList<TemperatureAndHumiditySensorReportMonthHistory>(result.Data.MonthList, pageNumber, pageSize, result.Data.TotalRecords);

                ViewBag.groupUpdatesByType = groupUpdatesByType;
                ViewBag.HeaderInfo = result.Data.HeaderInfo;
            }

            // Check if request is ajax request
            if (HttpContext.Request.Headers["x-requested-with"] == "XMLHttpRequest")
            {
                return PartialView("_InventorySensor", PagedResult);
            }
            return View(PagedResult);
        }

        [HttpGet("[Controller]/InventorySensor/PrintPDF/{inventoryId}/{sensorSerial}")]
        public async Task<FileResult> PrintPDF(long inventoryId, long sensorSerial, string fromDate, string toDate, string groupUpdatesByType = "", int? groupUpdatesValue = null)
        {
            var result = await _inventoryHistoryReportApiProxy.SensorHistoryReportPDF(inventoryId, sensorSerial, fromDate, toDate, groupUpdatesByType, groupUpdatesValue, IsEnglish);
            return File(result.Data, "application/pdf", "Report.pdf");
        }

        public async Task<IActionResult> OnlineHistory()
        {
            var result = await _onlineInventoryHistoryApiProxy.GetByUser(UserProfile.Id);
            return StatusCode((int)result.HttpCode, result.Data);
        }

        public async Task<IActionResult> InventoryOnlineHistory(long inventoryId)
        {
            var result = await _onlineInventoryHistoryApiProxy.GetOnlineSensorTemperture(inventoryId);
            return StatusCode((int)result.HttpCode, result.Data);
        }
        public async Task<IActionResult> InventoriesOnlineHistory(string inventoryIds)
        {
            var result = await _onlineInventoryHistoryApiProxy.GetOnlineInventoriesSensorData(new ListParam() { ids = GPSHelper.StringToListLong(inventoryIds) });
            return StatusCode((int)result.HttpCode, result.Data);
        }

        [UserPrivilege(Privilege = AgentPrivilegeTypeEnum.ViewAverageDailyTemperatureAndHumidityReport)]
        public IActionResult AverageDailyTemperatureAndHumidity()
        {
            return View();
        }

        [UserPrivilege(Privilege = AgentPrivilegeTypeEnum.ViewAverageDailyTemperatureAndHumidityReport)]
        public async Task<IActionResult> GetAverageDailyTemperatureAndHumidityReport(long inventoryId, string fromDate, string toDate)
        {
            var result = await _inventoryHistoryReportApiProxy.InventorySensorsAverageTemperatureAndHumidityByHour(inventoryId, fromDate, toDate);
            if (!result.IsSuccess && (int)result.HttpCode == 0)
            {
                return StatusCode(404, result.Data);
            }

            return StatusCode((int)result.HttpCode, result.Data);
        }

        [UserPrivilege(Privilege = AgentPrivilegeTypeEnum.ViewAlertsReport)]
        public async Task<IActionResult> Alert(long? warehouseId, long? inventoryId, long? sensorId, long? alertType, long? alertId, string fromDate, string toDate,
            int? page = 1, int? show = 100)
        {
            warehouseId = warehouseId == 0 ? null : warehouseId;
            inventoryId = inventoryId == 0 ? null : inventoryId;
            sensorId = sensorId == 0 ? null : sensorId;
            alertType = alertType == 0 ? null : alertType;
            var pageNumber = page ?? 1;
            var pageSize = show ?? 100;

            //warehouseId = await LoadWarehouses(warehouseId);
            //await LoadInventories(warehouseId.Value, inventoryId);
            //await LoadInventorySensorsBySensorId(inventoryId, sensorId);

            var PagedResult = new StaticPagedList<AlertViewModel>(new List<AlertViewModel>(), pageNumber, pageSize, 0);
            //if (!warehouseId.HasValue || string.IsNullOrEmpty(fromDate) || string.IsNullOrEmpty(toDate))
            //{
            //    return View(PagedResult);
            //}

            // Save current query in ViewBag for pager
            ViewBag.CurrentQuery = new Dictionary<string, string>()
            {
                { "warehouseId", warehouseId.ToString() },
                { "inventoryId", inventoryId.ToString() },
                { "sensorId", sensorId.ToString() },
                { "alertType", alertType.ToString() },
                { "fromDate", fromDate },
                { "toDate", toDate },
                { "show", pageSize.ToString() }
            };
            ViewBag.Action = "Alert";

            var result = await _alertApiProxy.PagedAlertsHistory(_loggedUser.UserId,warehouseId, inventoryId, sensorId, alertType, alertId, fromDate, toDate, pageNumber, pageSize);
            if (!result.IsSuccess && result.HttpCode != HttpCode.NotFound)
            {
                return View(_viewHelper.GetErrorPage(result.HttpCode));
            }

            // Convert result data list to StaticPagedList
            if (result.IsSuccess)
            {
                PagedResult = new StaticPagedList<AlertViewModel>(result.Data.List, pageNumber, pageSize, result.Data.TotalRecords);
            }

            // Check if request is ajax request
            if (HttpContext.Request.Headers["x-requested-with"] == "XMLHttpRequest")
            {
                return PartialView("_Alerts", PagedResult);
            }
            return View(PagedResult);
        }
        // GET: Sensors
        [UserPrivilege(Privilege = AgentPrivilegeTypeEnum.ViewWorkingAndNotWorkingSensorReport)]
        public async Task<IActionResult> Sensor(int? SensorStatus, int? BrandId = null, int? page = 1, int? show = 100, string search = "")
        {
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

            ViewBag.Action = "Sensor";
            await LoadBrands(BrandId);

            var result = await _SensorService.SearchAsync(_loggedUser.FleetId, SensorStatus, BrandId, search, pageNumber, pageSize);
            if (!result.IsSuccess)
            {
                return View(_viewHelper.GetErrorPage(result.HttpCode));
            }

            // Convert result data list to StaticPagedList
            var PagedResult = new StaticPagedList<SensorView>(result.Data.List, pageNumber, pageSize, result.Data.TotalRecords);

            // Check if request is ajax request
            if (HttpContext.Request.Headers["x-requested-with"] == "XMLHttpRequest")
            {
                return PartialView("_Sensors", PagedResult);
            }

            return View(PagedResult);
        }
        // GET: ExportSensorsData
        [UserPrivilege(Privilege = AgentPrivilegeTypeEnum.ViewWorkingAndNotWorkingSensorReport)]
        [HttpGet("[Controller]/Sensor/ExportSensorsData")]
        public async Task<IActionResult> ExportSensorsData([FromBody] int? SensorStatus, int? BrandId = null ,string search = "")
        {
            BrandId = BrandId == 0 ? null : BrandId;
            var pageNumber = 1;
            //var pageSize =  10;
            SensorStatus = SensorStatus < 0 ? null : SensorStatus;

            ViewBag.Action = "Export Sensors Data";

            var result = await _SensorService.SearchAsync(_loggedUser.FleetId, SensorStatus, BrandId, search, pageNumber, 999999999);
            if (!result.IsSuccess)
            {
                return View(_viewHelper.GetErrorPage(result.HttpCode));
            }
            DataTable dtHeader=new DataTable();
            dtHeader.Columns.Add("BrandName");
            dtHeader.Columns.Add("SensorName");
            dtHeader.Columns.Add("InventoryName");
            dtHeader.Columns.Add("WarehouseName");
            dtHeader.Columns.Add("CalibrationDate");
            dtHeader.Columns.Add("Serial");
            dtHeader.Rows.Add("Brand Name", "Sensor Name" , "Inventory Name", "Warehouse Name",
                "Calibration Date" , "Serial Number");
            List<SensorsData> sensorsData = new List<SensorsData>();
            foreach (var item in result.Data.List)
            {
                sensorsData.Add(new SensorsData
                {
                    BrandName=item.Brand.Name,
                    CalibrationDate=item.CalibrationDate,
                    InventoryName=item.InventoryName,
                    SensorName=item.Name,
                    Serial = item.Serial,
                    WarehouseName = item.WarehouseName
                });
            }
            ExportDto fileData = ExportDataService.ExportDataToFile(sensorsData,dtHeader, "Sensor Data Report",null);
            var file = new FileContentResult(fileData.Content, fileData.ContentType);
            file.FileDownloadName = "WorkingAndNotWorkingSensorReport"+ ".xlsx";
            
            return File(fileData.Content, fileData.ContentType, file.FileDownloadName);
        }
        [UserPrivilege(Privilege = AgentPrivilegeTypeEnum.ViewInventoryHistoryReport)]
        public IActionResult WarehouseInventory()
        {
            return View();
        }
        [UserPrivilege(Privilege = AgentPrivilegeTypeEnum.ViewInventoryHistoryReport)]
        public async Task<IActionResult> WarehouseInventorySensor()
        {
            await LoadWarehousesAndInventoriesAndSensors((long)_loggedUser.FleetId,_loggedUser.UserInventories);
            return View();
        }

        [UserPrivilege(Privilege = AgentPrivilegeTypeEnum.ViewInventoryHistoryReport)]
        [HttpPost("[Controller]/WarehouseInventorySensorReport/TemperatureAndHumiditySensorsHistoryReport")]
        public async Task<IActionResult> TemperatureAndHumiditySensorsHistoryReport([FromBody] FilterInventoryHistory filterInventoryHistory)
        {
            filterInventoryHistory.isEnglish = IsEnglish;
            var result = await _inventoryHistoryReportApiProxy.TemperatureAndHumiditySensorsHistoryReport(filterInventoryHistory);
            return StatusCode((int)result.HttpCode, result.Data);
        }

        [UserPrivilege(Privilege = AgentPrivilegeTypeEnum.ViewInventoryHistoryReport)]
        [HttpPost("[Controller]/WarehouseInventorySensorReport/TemperatureAndHumiditySensorsHistoryReport/PrintPDF")]
        public async Task<IActionResult> PrintPDF([FromBody] FilterInventoryHistory filterInventoryHistory)
        {
            var result = await _inventoryHistoryReportApiProxy.InventorySensorHistoryReportPDF(filterInventoryHistory);
            return File(result.Data, "application/pdf", "Report.pdf");
        }
    }
}
