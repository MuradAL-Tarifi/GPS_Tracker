using GPS.Services.Job;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GPS.API.Web.Controllers
{   
    /// <summary>
     /// Job API
     /// </summary>
    [ApiController]
    public class JobController : ControllerBase
    {
        private readonly IJobService _jobService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="jobService"></param>
        public JobController(IJobService jobService)
        {
            _jobService = jobService;
        }


        /// <summary>
        /// Scheduled Reports Watcher
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/v1/job/watcher/scheduled-reports")]
        public async Task<IActionResult> ScheduledReportsWatcher()
        {
            await _jobService.ScheduledReportsWatcherAsync();
            return Ok();
        }

        /// <summary>
        /// Inventory Custom Alerts Watcher
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/v1/job/watcher/inventory-custom-alerts")]
        public async Task<IActionResult> InventoryCustomAlertsWatcher()
        {
            await _jobService.InventoryCustomAlertsWatcherAsync();
            return Ok();
        }

    }
}
