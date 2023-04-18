using GPS.Domain.DTO;
using GPS.Domain.ViewModels;
using GPS.Domain.Views;
using GPS.Services.Inventorys;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GPS.API.History.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    public class InventoryHistoryController : ControllerBase
    {
        private readonly IInventoryHistoryService _inventoryHistoryService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gatewayHistoryService"></param>
        public InventoryHistoryController(IInventoryHistoryService gatewayHistoryService)
        {
            _inventoryHistoryService = gatewayHistoryService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inventoryId"></param>
        /// <param name="sensorSerial"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/v1/inventory-sensor-history/inventory/{inventoryId}/sensor/{sensorSerial}")]
        [Produces(typeof(ReturnResult<List<InventoryHistoryView>>))]
        public async Task<IActionResult> SensorHistory(long inventoryId, string sensorSerial, string fromDate, string toDate)
        {
            var result = await _inventoryHistoryService.SensorHistory(inventoryId, sensorSerial, fromDate, toDate);
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
        /// <returns></returns>
        [HttpGet]
        [Route("api/v1/inventory-sensor-history/inventory/{inventoryId}/sensor/{sensorSerial}/paged")]
        [Produces(typeof(ReturnResult<GatewayHistoryReport>))]
        public async Task<IActionResult> PagedHistory(long inventoryId, string sensorSerial, string fromDate,
            string toDate, int pageNumber, int pageSize)
        {
            var result = await _inventoryHistoryService.PagedHistory(inventoryId, sensorSerial, fromDate, toDate, pageNumber, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// Paged Temperature And Humidity History
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
        [Route("api/v1/inventory-sensor-history/inventory/{inventoryId}/sensor/{sensorSerial}/temperature-and-humidity/paged")]
        [Produces(typeof(ReturnResult<GatewayHistoryReport>))]
        public async Task<IActionResult> PagedSensorTemperatureAndHumidityHistoryAsync(long inventoryId, string sensorSerial,
            string fromDate, string toDate, int pageNumber, int pageSize, string groupUpdatesByType, int? groupUpdatesValue, bool isEnglish)
        {
            var result = await _inventoryHistoryService.PagedSensorTemperatureAndHumidityHistoryAsync(inventoryId, sensorSerial, fromDate, toDate, pageNumber, pageSize, groupUpdatesByType, groupUpdatesValue, isEnglish);
            return Ok(result);
        }

        /// <summary>
        /// Grouped Temperature And Humidity History
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
        [Route("api/v1/inventory-sensor-history/inventory/{inventoryId}/sensor/{sensorSerial}/temperature-and-humidity/grouped")]
        [Produces(typeof(ReturnResult<TemperatureAndHumidityHistoryReportResult>))]
        public async Task<IActionResult> GroupedSensorTemperatureAndHumidityHistory(long inventoryId, string sensorSerial, string fromDate,
           string toDate, int pageNumber, int pageSize, string groupUpdatesByType, int? groupUpdatesValue, bool isEnglish)
        {
            var result = await _inventoryHistoryService.GetGroupedSensorTemperatureAndHumidityHistoryAsync(inventoryId, sensorSerial, fromDate, toDate, groupUpdatesByType, groupUpdatesValue, isEnglish);
            return Ok(result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterInventoryHistory"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/v1/inventory-sensor-history/inventory/sensors/temperature-and-humidity/grouped")]
        [Produces(typeof(ReturnResult<List<TemperatureAndHumiditySensorHistoryReportResult>>))]
        public async Task<IActionResult> GroupedSensorTemperatureAndHumidityHistory([FromBody] FilterInventoryHistory filterInventoryHistory)
        {
            var result = await _inventoryHistoryService.GetListGroupedSensorsTemperatureAndHumidityHistoryAsync(filterInventoryHistory);
            return Ok(result);
        }
        /// <summary>
        /// inventory history by inventory Id
        /// </summary>
        /// <param name="inventoryId"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/v1/inventory-history/inventory/{inventoryId}")]
        [Produces(typeof(ReturnResult<List<InventoryHistoryView>>))]
        public async Task<IActionResult> GetByInventoryId(long inventoryId, string fromDate, string toDate)
        {
            var result = await _inventoryHistoryService.GetByInventoryIdAsync(inventoryId, fromDate, toDate);
            return Ok(result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inventoryId"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/v1/inventory-sensor-history/inventory/{inventoryId}/average-temperature-and-humidity-by-hour")]
        [Produces(typeof(ReturnResult<List<GroupedInventoryAverageTemperatureAndHumidity>>))]
        public async Task<IActionResult> InventorySensorsAverageTemperatureAndHumidityByHour(long inventoryId, string fromDate, string toDate)
        {
            var result = await _inventoryHistoryService.InventorySensorsAverageTemperatureAndHumidityByHourAsync(inventoryId, fromDate, toDate);
            return Ok(result);
        }
    }
}
