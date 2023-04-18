using System.Threading.Tasks;
using GPS.Domain.DTO;
using GPS.Domain.Views;
using GPS.Services.Inventorys;
using GPS.Services.WareHouses;
using Microsoft.AspNetCore.Mvc;

namespace EGPS.API.Web.Controllers
{
    /// <summary>
    /// Warehouse API
    /// </summary>
    [ApiController]
    public class WarehouseController : ControllerBase
    {
        private readonly IInventorySensorService _inventorySensorService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="inventorySensorService"></param>
        public WarehouseController(
            IInventorySensorService inventorySensorService)
        {
            _inventorySensorService = inventorySensorService;
        }

        /// <summary>
        /// Inventory Sensor By Sensor Serial
        /// </summary>
        /// <param name="serial"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/v1/warehouse/sensor/{serial}")]
        [Produces(typeof(ReturnResult<InventorySensorView>))]
        public async Task<IActionResult> InventorySensorBySensorSerial(string serial)
        {
            var result = await _inventorySensorService.FindBySensorSerialAsync(serial);

            if (result.IsSuccess)
            {
                var data = new
                {
                    result.Data.Id,
                    result.Data.InventoryId,
                    result.Data.SensorId,
                };

                return StatusCode((int)result.HttpCode, data);
            }
            return StatusCode((int)result.HttpCode, result);
        }
    }
}
