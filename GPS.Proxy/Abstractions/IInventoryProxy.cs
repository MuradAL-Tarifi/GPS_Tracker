using GPS.Models;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Proxy.Abstractions
{
    public interface IInventoryProxy
    {
        [Get("/api/InventorySensor/Sensor/{serial}")]
        Task<ApiResponse<InventorySensorView>> GetSensorBySerial([AliasAs("serial")] string serial);

        [Post("/api/InventoryHistory")]
        Task<ApiResponse<string>> SaveWarehouseHistory([Body] InventoryHistoryView gpsRawModel);

        [Get("/api/InventorySensor/SensorsSerialNumber/{inventoryId}")]
        Task<ApiResponse<List<string>>> ListSensorsSerialNumberByInventoryId([AliasAs("inventoryId")] long inventoryId);

        [Get("/api/Sensor/GetSensorSN/{serial}")]
        Task<ApiResponse<string>> GetSensorSN([AliasAs("serial")] string serial);
    }
}
