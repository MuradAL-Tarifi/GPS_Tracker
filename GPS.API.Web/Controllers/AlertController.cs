using GPS.Domain.DTO;
using GPS.Domain.ViewModels;
using GPS.Services.Alerts;
using GPS.Services.AlertTracker;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GPS.API.Web.Controllers
{
    /// <summary>
    ///  Alert API
    /// </summary>
    [ApiController]
    public class AlertController : ControllerBase
    {
        private readonly IAlertService _alertService;
        private readonly IAlertTrackerService _alertTrackerService;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="alertService"></param>
        public AlertController(IAlertService alertService, IAlertTrackerService alertTrackerService)
        {
            _alertService = alertService;
            _alertTrackerService = alertTrackerService;
        }
        /// <summary>
        /// Get Top 100 Alerts By UserId
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/v1/alert/top-100-alert/{userId}")]
        [Produces(typeof(ReturnResult<List<AlertViewModel>>))]
        public async Task<IActionResult> GetTop100AlertsByUserId(string userId)
        {
            var result = await _alertService.GetTop100AlertsAsync(userId);
            return Ok(result);
        }
        /// <summary>
        /// Paged Alerts History
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="warehouseId"></param>
        /// <param name="inventoryId"></param>
        /// <param name="sensorId"></param>
        /// <param name="alertType"></param>
        /// <param name="alertId"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/v1/alert/paged-alert-history")]
        [Produces(typeof(ReturnResult<PagedResult<AlertViewModel>>))]
        public async Task<IActionResult> PagedAlertsHistory(string userId, long? warehouseId, long? inventoryId, long? sensorId,
            long? alertType, long? alertId, string fromDate, string toDate, int pageNumber, int pageSize)
        {
            var result = await _alertService.GetPagedAlertsHistoryAsync(userId, warehouseId, inventoryId, sensorId, alertType, alertId, fromDate, toDate, pageNumber, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// Update Alerts As Read By UserId
        /// </summary>
        /// <param name="listParam"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/v1/alert/read-alert-by-user/{userId}")]
        [Produces(typeof(ReturnResult<bool>))]
        public async Task<IActionResult> UpdateAlertsAsRead(string userId, ListParam listParam)
        {
            var result = await _alertService.UpdateAlertsAsReadAsync(listParam.ids, userId);
            return Ok(result);
        }
        /// <summary>
        /// Paged Alerts History
        /// </summary>
        /// <param name="warehouseName"></param>
        /// <param name="fleetName"></param>
        /// <param name="sensorNumber"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/v1/alert/paged-alert-tracker")]
        [Produces(typeof(ReturnResult<PagedResult<AlertTrackerViewModel>>))]
        public async Task<IActionResult> PagedAlertsTracker(string warehouseName, string fleetName, string sensorNumber, string fromDate, string toDate, int pageNumber, int pageSize)
        {
            var result = await _alertTrackerService.GetPagedAlertsTrackerAsync(warehouseName, fleetName, sensorNumber, fromDate, toDate, pageNumber, pageSize);
            return Ok(result);
        }
    }
}
