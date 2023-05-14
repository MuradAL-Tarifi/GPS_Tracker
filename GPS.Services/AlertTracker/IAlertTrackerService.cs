using GPS.Domain.DTO;
using GPS.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Services.AlertTracker
{
    public interface IAlertTrackerService
    {        /// <summary>
             /// Get Paged Alerts History
             /// </summary>
             /// <param name="warehouseName"></param>
             /// <param name="fleetName"></param>
             /// <param name="sensorNumber"></param>
             /// <param name="fromDate"></param>
             /// <param name="toDate"></param>
             /// <param name="pageNumber"></param>
             /// <param name="pageSize"></param>
             /// <returns></returns>
        Task<ReturnResult<PagedResult<AlertTrackerViewModel>>> GetPagedAlertsTrackerAsync(string warehouseName, string fleetName, string sensorNumber, string fromDate, string toDate, int pageNumber, int pageSize);
    }
}
