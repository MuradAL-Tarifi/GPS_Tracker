using GPS.Domain.DTO;
using GPS.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Services.Inventorys
{
    public interface IInventoryHistoryReportService
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
        Task<ReturnResult<GatewayHistoryReport>> PagedHistory(long inventoryId, string sensorSerial,
           string fromDate, string toDate, int pageNumber, int pageSize);

        /// <summary>
        /// PagedSensor Temperature And Humidity History
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
        Task<ReturnResult<TemperatureAndHumiditySensorHistoryReportResult>> PagedSensorTemperatureAndHumidityHistory(long inventoryId, string sensorSerial,
            string fromDate, string toDate, int pageNumber, int pageSize, string groupUpdatesByType, int? groupUpdatesValue, bool isEnglish);

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
        Task<ReturnResult<byte[]>> InventorySensorHistoryReportPDF(long inventoryId, string sensorSerial, string fromDate, string toDate, string groupUpdatesByType, int? groupUpdatesValue, bool isEnglish);

        /// <summary>
        /// Inventory Sensor History Report Excel
        /// </summary>
        /// <param name="inventoryId"></param>
        /// <param name="sensorSerial"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="groupUpdatesByType"></param>
        /// <param name="groupUpdatesValue"></param>
        /// <param name="isEnglish"></param>
        /// <returns></returns>
        Task<ReturnResult<byte[]>> InventorySensorHistoryReportExcel(long inventoryId, string sensorSerial,
         string fromDate, string toDate, string groupUpdatesByType, int? groupUpdatesValue, bool isEnglish);

        /// <summary>
        /// Inventory History Report
        /// </summary>
        /// <param name="inventoryId"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        Task<ReturnResult<List<InventorySensorModel>>> InventoryHistoryReport(long inventoryId, string fromDate, string toDate);

        /// <summary>
        /// Inventory History Report PDF
        /// </summary>
        /// <param name="inventoryId"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="groupUpdatesByType"></param>
        /// <param name="groupUpdatesValue"></param>
        /// <param name="isEnglish"></param>
        /// <returns></returns>
        Task<ReturnResult<byte[]>> InventoryHistoryReportPDF(long inventoryId, string sensorsSN, string fromDate, string toDate, string groupUpdatesByType, int? groupUpdatesValue, bool isEnglish);

        /// <summary>
        /// Inventory History Report Excel
        /// </summary>
        /// <param name="inventoryId"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="groupUpdatesByType"></param>
        /// <param name="groupUpdatesValue"></param>
        /// <param name="isEnglish"></param>
        /// <returns></returns>
        Task<ReturnResult<byte[]>> InventoryHistoryReportExcel(long inventoryId, string fromDate, string toDate, string groupUpdatesByType, int? groupUpdatesValue, bool isEnglish);

        /// <summary>
        /// Sensor Average Temperature And Humidity By Hour
        /// </summary>
        /// <param name="inventoryId"></param>
        /// <param name="sensorSerial"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        Task<ReturnResult<List<GroupedInventoryAverageTemperatureAndHumidity>>> InventorySensorsAverageTemperatureAndHumidityByHourAsync(long inventoryId, string fromDate, string toDate);

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
        Task<ReturnResult<List<TemperatureAndHumidityInventoryHistoryReportResult>>> TemperatureAndHumidityInventoryHistoryReport(long inventoryId, string sensorsSN, string fromDate, string toDate, string groupUpdatesByType, int? groupUpdatesValue, bool isEnglish);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterInventoryHistory"></param>
        /// <returns></returns>
        Task<ReturnResult<List<TemperatureAndHumidityInventoryHistoryReportResult>>> TemperatureAndHumidityInventoryHistoryReport(FilterInventoryHistory filterInventoryHistory);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterInventoryHistory"></param>
        /// <returns></returns>
        Task<ReturnResult<byte[]>> InventoryHistoryReportPDF(FilterInventoryHistory filterInventoryHistory);

    }
}
