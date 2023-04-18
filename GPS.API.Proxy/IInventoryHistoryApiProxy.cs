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
    public interface IInventoryHistoryApiProxy
    {
        /// <summary>
        /// Sensor History
        /// </summary>
        /// <param name="inventoryId"></param>
        /// <param name="sensorSerial"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        [Get("/api/v1/inventory-sensor-history/inventory/{inventoryId}/sensor/{sensorSerial}")]
        Task<ReturnResult<List<InventoryHistoryView>>> SensorHistory(long inventoryId, string sensorSerial, string fromDate, string toDate);

        /// <summary>
        /// Paged History
        /// </summary>
        /// <param name="inventoryId"></param>
        /// <param name="sensorSerial"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [Get("/api/v1/inventory-sensor-history/inventory/{inventoryId}/sensor/{sensorSerial}/paged")]
        Task<ReturnResult<GatewayHistoryReport>> PagedHistory(long inventoryId, string sensorSerial, string fromDate, string toDate, int pageNumber, int pageSize);

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
        [Get("/api/v1/inventory-sensor-history/inventory/{inventoryId}/sensor/{sensorSerial}/temperature-and-humidity/paged")]
        Task<ReturnResult<TemperatureAndHumiditySensorHistoryReportResult>> PagedSensorTemperatureAndHumidityHistory(long inventoryId, string sensorSerial, string fromDate,
            string toDate, int pageNumber, int pageSize, string groupUpdatesByType, int? groupUpdatesValue, bool isEnglish);

        /// <summary>
        /// /Grouped Temperature And Humidity History
        /// </summary>
        /// <param name="inventoryId"></param>
        /// <param name="sensorSerial"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="groupUpdatesByType"></param>
        /// <param name="groupUpdatesValue"></param>
        /// <param name="isEnglish"></param>
        /// <returns></returns>
        [Get("/api/v1/inventory-sensor-history/inventory/{inventoryId}/sensor/{sensorSerial}/temperature-and-humidity/grouped")]
        Task<ReturnResult<TemperatureAndHumiditySensorHistoryReportResult>> GroupedSensorTemperatureAndHumidityHistory(long inventoryId, string sensorSerial, string fromDate,
           string toDate, string groupUpdatesByType, int? groupUpdatesValue, bool isEnglish);

        [Get("/api/v1/inventory-history/inventory/{inventoryId}")]
        Task<ReturnResult<List<InventoryHistoryView>>> GetByInventoryId(long inventoryId, string fromDate, string toDate);

        /// <summary>
        /// Sensor Average Temperature And Humidity By Hour
        /// </summary>
        /// <param name="inventoryId"></param>
        /// <param name="sensorSerial"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        [Get("/api/v1/inventory-sensor-history/inventory/{inventoryId}/average-temperature-and-humidity-by-hour")]
        Task<ReturnResult<List<GroupedInventoryAverageTemperatureAndHumidity>>> InventorySensorsAverageTemperatureAndHumidityByHour(long inventoryId, string fromDate, string toDate);

        [Post("/api/v1/inventory-sensor-history/inventory/sensors/temperature-and-humidity/grouped")]
        Task<ReturnResult<List<TemperatureAndHumiditySensorHistoryReportResult>>> GroupedSensorTemperatureAndHumidityHistory(FilterInventoryHistory filterInventoryHistory);
    }
}
