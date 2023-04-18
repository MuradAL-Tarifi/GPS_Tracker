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
   public interface IAlertApiProxy
    {
        /// <summary>
        /// Get Top 100 Alerts By User Id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [Get("/api/v1/alert/top-100-alert/{userId}")]
        Task<ReturnResult<List<AlertViewModel>>> GetTop100Alerts(string userId);

        /// <summary>
        /// Paged Alerts History
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
        [Get("/api/v1/alert/paged-alert-history")]
        Task<ReturnResult<PagedResult<AlertViewModel>>> PagedAlertsHistory(string userId, long? warehouseId, long? inventoryId, long? sensorId,
            long? alertType, long? alertId, string fromDate, string toDate, int pageNumber, int pageSize);

        /// <summary>
        /// Update Alerts As Read By User
        /// </summary>
        /// <param name="listParam"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [Post("/api/v1/alert/read-alert-by-user/{userId}")]
        Task<ReturnResult<bool>> UpdateAlertsAsRead(string userId, ListParam listParam);
    }
}
