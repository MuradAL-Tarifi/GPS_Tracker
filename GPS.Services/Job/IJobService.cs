using GPS.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Services.Job
{
    public interface IJobService
    {


        /// <summary>
        /// Scheduled Reports Watcher
        /// </summary>
        /// <returns></returns>
        Task ScheduledReportsWatcherAsync();


        /// <summary>
        /// Inventory Custom Alerts Watcher
        /// </summary>
        /// <returns></returns>
        Task InventoryCustomAlertsWatcherAsync();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="customAlertWatcher"></param>
        /// <returns></returns>
        Task BackgroundCustomAlertsJobAsync(CustomAlertWatcher customAlertWatcher);

    }
}
