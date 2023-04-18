using GPS.Domain.DTO;
using GPS.Domain.ViewModels;
using GPS.Domain.Views;
using GPS.Services.Inventorys;
using Microsoft.AspNetCore.Mvc;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GPS.API.Web.Controllers
{
    /// <summary>
    /// Inventory History Reports API
    /// </summary>
    [ApiController]
    public class InventoryHistoryReportController : ControllerBase
    {
        private readonly IInventoryHistoryReportService _inventoryHistoryReportService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inventoryHistoryReportService"></param>
        public InventoryHistoryReportController(IInventoryHistoryReportService inventoryHistoryReportService)
        {
            _inventoryHistoryReportService = inventoryHistoryReportService;
        }

        /// <summary>
        /// Paged Inventory History
        /// </summary>
        /// <param name="inventoryId"></param>
        /// <param name="sensorSerial"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/v1/inventory-sensor-history/report/inventory/{inventoryId}/sensor/{sensorSerial}/paged")]
        [Produces(typeof(ReturnResult<GatewayHistoryReport>))]
        public async Task<IActionResult> PagedHistory(long inventoryId, string sensorSerial, string fromDate,
            string toDate, int pageNumber, int pageSize)
        {
            var result = await _inventoryHistoryReportService.PagedHistory(inventoryId, sensorSerial, fromDate, toDate, pageNumber, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// Paged Inventory History
        /// </summary>
        /// <param name="inventoryId"></param>
        /// <param name="sensorSerial"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="groupUpdatesByType"></param>
        /// <param name="groupUpdatesValue"></param>
        /// <param name="isEnglish"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/v1/inventory-sensor-history/report/inventory/{inventoryId}/sensor/{sensorSerial}/temperature-and-humidity/paged")]
        [Produces(typeof(ReturnResult<GatewayHistoryReport>))]
        public async Task<IActionResult> PagedTemperatureAndHumidityHistory(long inventoryId, string sensorSerial, string fromDate,
            string toDate, int pageNumber, int pageSize, string groupUpdatesByType, int? groupUpdatesValue, bool isEnglish)
        {
            var result = await _inventoryHistoryReportService.PagedSensorTemperatureAndHumidityHistory(inventoryId, sensorSerial, fromDate, toDate, pageNumber, pageSize, groupUpdatesByType, groupUpdatesValue, isEnglish);
            return Ok(result);
        }

        /// <summary>
        /// Inventory Sensor History Report PDF
        /// </summary>
        /// <param name="inventoryId"></param>
        /// <param name="sensorSerial"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="groupUpdatesByType"></param>
        /// <param name="groupUpdatesValue"></param>
        /// <param name="isEnglish"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/v1/inventory-sensor-history/report/inventory/{inventoryId}/sensor/{sensorSerial}/pdf")]
        [Produces(typeof(ReturnResult<byte[]>))]
        public async Task<IActionResult> InventorySensorHistoryReportPDF(long inventoryId, string sensorSerial, string fromDate, string toDate, string groupUpdatesByType, int? groupUpdatesValue, bool isEnglish)
        {
            var result = await _inventoryHistoryReportService.InventorySensorHistoryReportPDF(inventoryId, sensorSerial, fromDate, toDate, groupUpdatesByType, groupUpdatesValue, isEnglish);
            return Ok(result);
        }

        ///// <summary>
        ///// Inventory Sensor History Report Excel
        ///// </summary>
        ///// <param name="inventoryId"></param>
        ///// <param name="sensorSerial"></param>
        ///// <param name="fromDate"></param>
        ///// <param name="toDate"></param>
        ///// <param name="groupUpdatesByType"></param>
        ///// <param name="groupUpdatesValue"></param>
        ///// <param name="isEnglish"></param>
        ///// <returns></returns>
        //[HttpGet]
        //[Route("api/v1/inventory-sensor-history/report/inventory/{inventoryId}/sensor/{sensorSerial}/excel")]
        //[Produces(typeof(ReturnResult<byte[]>))]
        //public async Task<IActionResult> InventorySensorHistoryReportExcel(long inventoryId, string sensorSerial, string fromDate, string toDate, string groupUpdatesByType, int? groupUpdatesValue, bool isEnglish)
        //{
        //    var result = await _inventoryHistoryReportService.InventorySensorHistoryReportExcel(inventoryId, sensorSerial, fromDate, toDate, groupUpdatesByType, groupUpdatesValue, isEnglish);
        //    return Ok(result);
        //}

        /// <summary>
        /// inventory history report
        /// </summary>
        /// <param name="inventoryId"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/v1/inventory-history/report/inventory/{inventoryId}")]
        [Produces(typeof(ReturnResult<List<InventorySensorModel>>))]
        public async Task<IActionResult> InventoryHistoryReport(long inventoryId, string fromDate, string toDate)
        {
            var result = await _inventoryHistoryReportService.InventoryHistoryReport(inventoryId, fromDate, toDate);
            return Ok(result);
        }

        /// <summary>
        /// Temperature And Humidity Inventory History Report
        /// </summary>
        /// <param name="inventoryId"></param>
        /// <param name="sensorsSN"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="groupUpdatesByType"></param>
        /// <param name="groupUpdatesValue"></param>
        /// <param name="isEnglish"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/v1/inventory-history/report/inventory/{inventoryId}/temperature-and-humidity")]
        [Produces(typeof(ReturnResult<List<TemperatureAndHumidityInventoryHistoryReportResult>>))]
        public async Task<IActionResult> TemperatureAndHumidityInventoryHistoryReport(long inventoryId, string sensorsSN, string fromDate, string toDate, string groupUpdatesByType, int? groupUpdatesValue, bool isEnglish)
        {
            var result = await _inventoryHistoryReportService.TemperatureAndHumidityInventoryHistoryReport(inventoryId, sensorsSN, fromDate, toDate, groupUpdatesByType, groupUpdatesValue, isEnglish);
            return Ok(result);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterInventoryHistory"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/v1/inventory-sensor-history/report/inventory/temperature-and-humidity")]
        [Produces(typeof(ReturnResult<List<TemperatureAndHumidityInventoryHistoryReportResult>>))]
        public async Task<IActionResult> TemperatureAndHumidityInventorySensorHistoryReport([FromBody]FilterInventoryHistory filterInventoryHistory)
        {
            var result = await _inventoryHistoryReportService.TemperatureAndHumidityInventoryHistoryReport(filterInventoryHistory);
            return Ok(result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterInventoryHistory"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/v1/inventory-sensor-history/report/inventory/temperature-and-humidity/pdf")]
        [Produces(typeof(ReturnResult<byte[]>))]
        public async Task<IActionResult> InventoryHistoryReportPDF([FromBody] FilterInventoryHistory filterInventoryHistory)
        {
            var result = await _inventoryHistoryReportService.InventoryHistoryReportPDF(filterInventoryHistory);
            return Ok(result);
        }


        /// <summary>
        /// inventory history report PDF
        /// </summary>
        /// <param name="inventoryId"></param>
        /// <param name="sensorsSN"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="groupUpdatesByType"></param>
        /// <param name="groupUpdatesValue"></param>
        /// <param name="isEnglish"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/v1/inventory-history/report/inventory/{inventoryId}/pdf")]
        [Produces(typeof(ReturnResult<byte[]>))]
        public async Task<IActionResult> InventoryHistoryReportPDF(long inventoryId, string sensorsSN, string fromDate, string toDate, string groupUpdatesByType, int? groupUpdatesValue, bool isEnglish)
        {
            var result = await _inventoryHistoryReportService.InventoryHistoryReportPDF(inventoryId, sensorsSN, fromDate, toDate, groupUpdatesByType, groupUpdatesValue, isEnglish);
            return Ok(result);
        }


        ///// <summary>
        ///// inventory history report Excel
        ///// </summary>
        ///// <param name="inventoryId"></param>
        ///// <param name="fromDate"></param>
        ///// <param name="toDate"></param>
        ///// <param name="groupUpdatesByType"></param>
        ///// <param name="groupUpdatesValue"></param>
        ///// <param name="isEnglish"></param>
        ///// <returns></returns>
        //[HttpGet]
        //[Route("api/v1/inventory-history/report/inventory/{inventoryId}/excel")]
        //[Produces(typeof(ReturnResult<byte[]>))]
        //public async Task<IActionResult> InventoryHistoryReportExcel(long inventoryId, string fromDate, string toDate, string groupUpdatesByType, int? groupUpdatesValue, bool isEnglish)
        //{
        //    var result = await _inventoryHistoryReportService.InventoryHistoryReportExcel(inventoryId, fromDate, toDate, groupUpdatesByType, groupUpdatesValue, isEnglish);
        //    return Ok(result);
        //}

        /// <summary>
        /// Inventory Sensors History By Hour
        /// </summary>
        /// <param name="inventoryId"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>

        [HttpGet]
        [Route("api/v1/inventory-sensor-history/report/inventory/{inventoryId}/average-temperature-and-humidity-by-hour")]
        [Produces(typeof(ReturnResult<List<GroupedInventoryAverageTemperatureAndHumidity>>))]
        public async Task<IActionResult> InventorySensorsAverageTemperatureAndHumidityByHour(long inventoryId, string fromDate, string toDate)
        {
            var result = await _inventoryHistoryReportService.InventorySensorsAverageTemperatureAndHumidityByHourAsync(inventoryId, fromDate, toDate);
            return Ok(result);
        }
    }
}
