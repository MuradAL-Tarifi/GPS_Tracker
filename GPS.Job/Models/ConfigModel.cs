using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GPS.Job.Models
{
    public class ConfigModel
    {
        public string ScheduledReportsCron { get; set; }
        public string InventoryCustomAlertsWatcherCron { get; set; }
    }
}
