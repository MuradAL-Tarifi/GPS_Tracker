using GPS.Domain.DTO;
using GPS.Domain.ViewModels;
using GPS.Domain.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Services.Inventorys
{
    public interface IInventoryHistoryService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="gatewayHistoryView"></param>
        /// <returns></returns>
        Task<ReturnResult<long>> SaveAsync(InventoryHistoryView gatewayHistoryView);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inventoryId"></param>
        /// <param name="sensorSerial"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        Task<ReturnResult<List<InventoryHistoryView>>> SensorHistory(long inventoryId, string sensorSerial,
        string fromDate, string toDate);

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
        /// Paged Sensor Temperature And Humidity History
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
        Task<ReturnResult<TemperatureAndHumiditySensorHistoryReportResult>> PagedSensorTemperatureAndHumidityHistoryAsync(long inventoryId, string sensorSerial,
            string fromDate, string toDate, int pageNumber, int pageSize, string groupUpdatesByType, int? groupUpdatesValue, bool isEnglish);

        /// <summary>
        /// Get Grouped Temperature And Humidity History
        /// </summary>
        /// <param name="inventoryId"></param>
        /// <param name="sensorSerial"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="groupUpdatesByType"></param>
        /// <param name="groupUpdatesValue"></param>
        /// <param name="isEnglish"></param>
        /// <returns></returns>
        Task<ReturnResult<TemperatureAndHumiditySensorHistoryReportResult>> GetGroupedSensorTemperatureAndHumidityHistoryAsync(long inventoryId, string sensorSerial,
            string fromDate, string toDate, string groupUpdatesByType, int? groupUpdatesValue, bool isEnglish);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inventoryId"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        Task<ReturnResult<List<InventoryHistoryView>>> GetByInventoryIdAsync(long inventoryId, string fromDate, string toDate);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inventoryId"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        Task<ReturnResult<List<GroupedInventoryAverageTemperatureAndHumidity>>> InventorySensorsAverageTemperatureAndHumidityByHourAsync(long inventoryId, string fromDate, string toDate);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lsSensorSerial"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="groupUpdatesByType"></param>
        /// <param name="groupUpdatesValue"></param>
        /// <param name="isEnglish"></param>
        /// <returns></returns>
        Task<ReturnResult<List<TemperatureAndHumiditySensorHistoryReportResult>>> GetListGroupedSensorsTemperatureAndHumidityHistoryAsync(FilterInventoryHistory filterInventoryHistory);
    }
}
