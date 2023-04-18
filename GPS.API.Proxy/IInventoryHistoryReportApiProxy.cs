using GPS.Domain.DTO;
using GPS.Domain.ViewModels;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.API.Proxy
{
    public interface IInventoryHistoryReportApiProxy
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inventoryId"></param>
        /// <param name="sensorSerial"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [Get("/api/v1/inventory-sensor-history/report/inventory/{inventoryId}/sensor/{sensorSerial}/paged")]
        Task<ReturnResult<GatewayHistoryReport>> PagedHistory(long inventoryId, long? sensorSerial, string fromDate, string toDate, int pageNumber, int pageSize);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inventoryId"></param>
        /// <param name="sensorSerial"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="pageNumber"></param>
        /// <param name="groupUpdatesByType"></param>
        /// <param name="groupUpdatesValue"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [Get("/api/v1/inventory-sensor-history/report/inventory/{inventoryId}/sensor/{sensorSerial}/temperature-and-humidity/paged")]
        Task<ReturnResult<TemperatureAndHumiditySensorHistoryReportResult>> PagedTemperatureAndHumidityHistory(long inventoryId, string sensorSerial, string fromDate, string toDate, int pageNumber, int pageSize, string groupUpdatesByType, int? groupUpdatesValue, bool isEnglish);

        /// <summary>
        /// Sensor History Report PDF
        /// </summary>
        /// <param name="inventoryId"></param>
        /// <param name="sensorSerial"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="groupUpdatesByType"></param>
        /// <param name="groupUpdatesValue"></param>
        /// <param name="isEnglish"></param>
        /// <returns></returns>
        [Get("/api/v1/inventory-sensor-history/report/inventory/{inventoryId}/sensor/{sensorSerial}/pdf")]
        Task<ReturnResult<byte[]>> SensorHistoryReportPDF(long inventoryId, long sensorSerial, string fromDate, string toDate, string groupUpdatesByType, int? groupUpdatesValue, bool isEnglish);

        /// <summary>
        /// Sensor History Report Excel
        /// </summary>
        /// <param name="inventoryId"></param>
        /// <param name="sensorSerial"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        ///  /// <param name="groupUpdatesByType"></param>
        /// <param name="groupUpdatesValue"></param>
        /// <param name="isEnglish"></param>
        /// <returns></returns>
        [Get("/api/v1/inventory-sensor-history/report/inventory/{inventoryId}/sensor/{sensorSerial}/excel")]
        Task<ReturnResult<byte[]>> SensorHistoryReportExcel(long inventoryId, long sensorSerial, string fromDate, string toDate, string groupUpdatesByType, int? groupUpdatesValue, bool isEnglish);

        /// <summary>
        /// Inventory History Report
        /// </summary>
        /// <param name="inventoryId"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        [Get("/api/v1/inventory-history/report/inventory/{inventoryId}")]
        Task<ReturnResult<List<InventorySensorModel>>> InventoryHistoryReport(long inventoryId, string fromDate, string toDate);

        /// <summary>
        /// Temperature And Humidity Inventory History Report
        /// </summary>
        /// <param name="inventoryId"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="groupUpdatesByType"></param>
        /// <param name="groupUpdatesValue"></param>
        /// <param name="isEnglish"></param>
        /// <returns></returns>
        [Get("/api/v1/inventory-history/report/inventory/{inventoryId}/temperature-and-humidity")]
        Task<ReturnResult<List<TemperatureAndHumidityInventoryHistoryReportResult>>> TemperatureAndHumidityInventoryHistoryReport(long inventoryId, string sensorsSN, string fromDate, string toDate, string groupUpdatesByType, int? groupUpdatesValue, bool isEnglish);

        /// <summary>
        /// Inventory History Report
        /// </summary>
        /// <param name="inventoryId"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="groupUpdatesByType"></param>
        /// <param name="groupUpdatesValue"></param>
        /// <param name="isEnglish"></param>
        /// <returns></returns>
        [Get("/api/v1/inventory-history/report/inventory/{inventoryId}/pdf")]
        Task<ReturnResult<byte[]>> InventoryHistoryReportPDF(long inventoryId, string sensorsSN, string fromDate, string toDate, string groupUpdatesByType, int? groupUpdatesValue, bool isEnglish);

        /// <summary>
        /// Inventory History Report
        /// </summary>
        /// <param name="inventoryId"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="groupUpdatesByType"></param>
        /// <param name="groupUpdatesValue"></param>
        /// <param name="isEnglish"></param>
        /// <returns></returns>
        [Get("/api/v1/inventory-history/report/inventory/{inventoryId}/excel")]
        Task<ReturnResult<byte[]>> InventoryHistoryReportExcel(long inventoryId, string fromDate, string toDate, string groupUpdatesByType, int? groupUpdatesValue, bool isEnglish);

        /// <summary>
        /// Inventory Sensors Average Temperature And Humidity By Hour
        /// </summary>
        /// <param name="inventoryId"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        [Get("/api/v1/inventory-sensor-history/report/inventory/{inventoryId}/average-temperature-and-humidity-by-hour")]
        Task<ReturnResult<List<GroupedInventoryAverageTemperatureAndHumidity>>> InventorySensorsAverageTemperatureAndHumidityByHour(long inventoryId, string fromDate, string toDate);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterInventoryHistory"></param>
        /// <returns></returns>
        [Post("/api/v1/inventory-sensor-history/report/inventory/temperature-and-humidity")]
        Task<ReturnResult<List<TemperatureAndHumidityInventoryHistoryReportResult>>> TemperatureAndHumiditySensorsHistoryReport(FilterInventoryHistory filterInventoryHistory);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterInventoryHistory"></param>
        /// <returns></returns>
        [Post("/api/v1/inventory-sensor-history/report/inventory/temperature-and-humidity/pdf")]
        Task<ReturnResult<byte[]>> InventorySensorHistoryReportPDF(FilterInventoryHistory filterInventoryHistory); 
    }
}
