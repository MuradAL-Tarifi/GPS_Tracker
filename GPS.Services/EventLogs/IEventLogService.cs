using GPS.Domain.DTO;
using GPS.Domain.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Services.EventLogs
{
    public interface IEventLogService
    {
        /// <summary>
        /// Log Change Event
        /// </summary>
        /// <param name="Type"></param>
        /// <param name="ObjectId"></param>
        /// <param name="Data"></param>
        /// <param name="UserId"></param>
        Task LogEventAsync(Event Type, Object ObjectId, Object Data, string UserId);

        /// <summary>
        /// Search Event Log
        /// </summary>
        /// <param name="FromDate"></param>
        /// <param name="ToDate"></param>
        /// <param name="PageNumber"></param>
        /// <param name="PageSize"></param>
        Task<ReturnResult<PagedResult<EventLogView>>> SearchAsync(string Type, DateTime? FromDate = null, DateTime? ToDate = null, string SearchString = "", int PageNumber = 1, int pageSize = 100);
    }
}
