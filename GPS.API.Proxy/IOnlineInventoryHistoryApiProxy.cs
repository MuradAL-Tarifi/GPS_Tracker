using GPS.Domain.DTO;
using GPS.Domain.ViewModels;
using GPS.Domain.Views;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.API.Proxy
{
    public interface IOnlineInventoryHistoryApiProxy
    {
        /// <summary>
        /// online inventory history report
        /// </summary>
        /// <param name="fleetId"></param>
        /// <param name="warehouseId"></param>
        /// <param name="inventoryId"></param>
        /// <returns></returns>
        [Get("/api/v1/online-inventory-history/fleet/{fleetId}/report")]
        Task<ReturnResult<GatewayHistoryOnlineReport>> Report(long fleetId, long? warehouseId = null, long? inventoryId = null);

        /// <summary>
        /// online inventory history by user Id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [Get("/api/v1/online-inventory-history/user/{userId}")]
        Task<ReturnResult<List<WarehouseView>>> GetByUser(string userId);

        /// <summary>
        /// online sensor temperture
        /// </summary>
        /// <param name="inventoryId"></param>
        /// <returns></returns>
        [Get("/api/v1/online-inventory-history/inventory/{inventoryId}/online-sensor-temperture")]
        Task<ReturnResult<List<InventorySensorTemperatureModel>>> GetOnlineSensorTemperture(long inventoryId);

        /// <summary>
        /// Online Inventories Sensor Data
        /// </summary>
        /// <returns></returns>
        [Post("/api/v1/online-inventories-history")]
        Task<ReturnResult<List<OnlineInventorySensors>>> GetOnlineInventoriesSensorData(ListParam listParam);
    }
}
