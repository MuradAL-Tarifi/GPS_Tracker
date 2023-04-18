using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Refit;

namespace GPS.API.Proxy
{
    public interface IJobApiProxy
    {
        /// <summary>
        /// Scheduled Reports Watcher
        /// </summary>
        /// <returns></returns>
        [Get("/api/v1/job/watcher/scheduled-reports")]
        Task ScheduledReportsWatcher();

        /// <summary>
        /// Inventory Custom Alerts Watcher
        /// </summary>
        /// <returns></returns>
        [Get("/api/v1/job/watcher/inventory-custom-alerts")]
        Task InventoryCustomAlertsWatcher();

    }
}
