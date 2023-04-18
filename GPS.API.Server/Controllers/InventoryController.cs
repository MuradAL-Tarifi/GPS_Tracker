using GPS.API.Server.Services;
using GPS.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GPS.API.Server.Controllers
{
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryHistoryService _iInventoryHistoryService;

        public InventoryController(IInventoryHistoryService iInventoryHistoryService)
        {
            _iInventoryHistoryService = iInventoryHistoryService;
        }

        [HttpGet()]
        [Route("api/InventorySensor/Sensor/{serial}")]
        [Produces(typeof(ReturnResult<InventorySensorView>))]
        public async Task<IActionResult> GetSensorBySerial(string serial)
        {
            var result = await _iInventoryHistoryService.GetInventorySensor(serial);

            if (result.IsSuccess)
                return Ok(result.Data);
            else
                return StatusCode((int)result.HttpCode, result);
        }

        [HttpGet()]
        [Route("api/Sensor/GetSensorSN/{serial}")]
        [Produces(typeof(ReturnResult<string>))]
        public async Task<IActionResult> GetSensorSN(string serial)
        {
            var result = await _iInventoryHistoryService.GetSensorSN(serial);

            if (result.IsSuccess)
                return Ok(result.Data);
            else
                return StatusCode((int)result.HttpCode, result);
        }


        /// <summary>
        /// save inventory history
        /// </summary>
        /// <param name="gatewayHistoryView"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces(typeof(ReturnResult<long>))]
        [Route("api/InventoryHistory")]
        public async Task<IActionResult> SaveGatewayHistory(InventoryHistoryView gatewayHistoryView)
        {
            var result = await _iInventoryHistoryService.SaveAsync(gatewayHistoryView);
            return StatusCode((int)result.HttpCode, result);
        }

        [HttpGet()]
        [Route("api/InventorySensor/SensorsSerialNumber/{inventoryId}")]
        [Produces(typeof(ReturnResult<List<string>>))]
        public async Task<IActionResult> ListSensorsSerialNumberByInventoryId(long inventoryId)
        {
            var result = await _iInventoryHistoryService.GetListSensorsSNByInventoryId(inventoryId);

            if (result.IsSuccess)
                return Ok(result.Data);
            else
                return StatusCode((int)result.HttpCode, result);
        }
    }
}
