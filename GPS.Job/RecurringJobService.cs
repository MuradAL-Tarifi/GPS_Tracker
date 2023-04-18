using GPS.API.Proxy;
using GPS.Helper;
using GPS.Job.Models;
using Hangfire;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GPS.Job
{
    public class RecurringJobService : BackgroundService
    {
        private readonly IRecurringJobManager _recurringJobs;
        private readonly ConfigModel _hangfireConfig;
        private readonly ILogger<RecurringJobService> _logger;

        public RecurringJobService(
            IRecurringJobManager recurringJobs,
            ConfigModel hangfireConfig,
            ILogger<RecurringJobService> logger)
        {
            _recurringJobs = recurringJobs;
            _hangfireConfig = hangfireConfig;
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                _recurringJobs.AddOrUpdate<IJobApiProxy>("Scheduled Reports Watcher", service => service.ScheduledReportsWatcher(), _hangfireConfig.ScheduledReportsCron);
                _recurringJobs.AddOrUpdate<IJobApiProxy>("Inventory Custom Alerts Watcher", service => service.InventoryCustomAlertsWatcher(), _hangfireConfig.InventoryCustomAlertsWatcherCron);
            }
            catch (Exception ex)
            {
                GPSHelper.LogHistory($"GPS Job Exception {ex}");
                _logger.LogError(ex, ex.Message);
            }

            return Task.CompletedTask;
        }
    }
}
