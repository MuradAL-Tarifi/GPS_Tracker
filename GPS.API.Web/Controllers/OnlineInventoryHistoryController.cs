using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GPS.Services;
using GPS.Domain.DTO;
using GPS.Domain.ViewModels;
using Microsoft.AspNetCore.Mvc;
using GPS.Services.OnlineInventoryHistorys;

namespace GPS.API.Web.Controllers
{
    /// <summary>
    /// Online Inventory History
    /// </summary>
    [ApiController]
    public class OnlineInventoryHistoryController : ControllerBase
    {
        private readonly IOnlineInventoryHistoryService _onlineInventoryHistoryService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="gatewayHistoryOnlineService"></param>
        public OnlineInventoryHistoryController(IOnlineInventoryHistoryService gatewayHistoryOnlineService)
        {
            _onlineInventoryHistoryService = gatewayHistoryOnlineService;
        }

        /// <summary>
        /// online inventory history report
        /// </summary>
        /// <param name="fleetId"></param>
        /// <param name="warehouseId"></param>
        /// <param name="inventoryId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/v1/online-inventory-history/fleet/{fleetId}/report")]
        [Produces(typeof(ReturnResult<GatewayHistoryOnlineReport>))]
        public async Task<IActionResult> Report(long fleetId, long? warehouseId = null, long? inventoryId = null)
        {
            var finalResult = new ReturnResult<GatewayHistoryOnlineReport>();

            var result = await _onlineInventoryHistoryService.SearchAsync(fleetId, warehouseId, inventoryId);
            if (result.IsSuccess && result.Data.Count > 0)
            {
                var maxTemperature = result.Data.Max(x => x.Temperature);
                var maxHumidity = result.Data.Max(x => x.Humidity);

                finalResult = new ReturnResult<GatewayHistoryOnlineReport>()
                {
                    IsSuccess = true,
                    HttpCode = HttpCode.Success,
                    Data = new GatewayHistoryOnlineReport()
                    {
                        MaxTemperature = maxTemperature,
                        MaxHumidity = maxHumidity,
                        Records = result.Data
                    }
                };
            }
            else
            {
                finalResult.NotFound(result.ErrorList);
            }

            return Ok(finalResult);
        }

        /// <summary>
        /// online inventory history by user Id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/v1/online-inventory-history/user/{userId}")]
        [Produces(typeof(ReturnResult<GatewayHistoryOnlineReport>))]
        public async Task<IActionResult> GetByUser(string userId)
        {
            var result = await _onlineInventoryHistoryService.GetByUserIdAsync(userId);
            return Ok(result);
        }

        /// <summary>
        /// online sensor temperture
        /// </summary>
        /// <param name="inventoryId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/v1/online-inventory-history/inventory/{inventoryId}/online-sensor-temperture")]
        [Produces(typeof(ReturnResult<GatewayHistoryOnlineReport>))]
        public async Task<IActionResult> OnlineSensorTemperture(long inventoryId)
        {
            var result = await _onlineInventoryHistoryService.GetOnlineSensorTempertureAsync(inventoryId);
            return Ok(result);
        }

        /// <summary>
        /// online inventories sensors temperture and humidity data
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/v1/online-inventories-history")]
        [Produces(typeof(ReturnResult<OnlineInventorySensors>))]
        public async Task<IActionResult> GetOnlineInventoriesSensorData(ListParam listParam)
        {
            var result = await _onlineInventoryHistoryService.GetOnlineInventoriesSensorData(listParam.ids);
            return Ok(result);
        }
    }
}
