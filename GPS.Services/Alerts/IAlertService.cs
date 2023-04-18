using GPS.Domain.DTO;
using GPS.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Services.Alerts
{
   public interface IAlertService
    {
        /// <summary>
        /// Get Top 100 Alerts
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<ReturnResult<List<AlertViewModel>>> GetTop100AlertsAsync(string userId);
        /// <summary>
        /// Get Paged Alerts History
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="warehouseId"></param>
        /// <param name="inventoryId"></param>
        /// <param name="sensorId"></param>
        /// <param name="alertType"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        Task<ReturnResult<PagedResult<AlertViewModel>>> GetPagedAlertsHistoryAsync(string userId, long? warehouseId, long? inventoryId, long? sensorId,
            long? alertType, long? alertId, string fromDate, string toDate, int pageNumber, int pageSize);
        /// <summary>
        /// Update Alerts As Read By UserId
        /// </summary>
        /// <param name="alertIds"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<ReturnResult<bool>> UpdateAlertsAsReadAsync(List<long> alertIds, string userId);
        
    }
}
