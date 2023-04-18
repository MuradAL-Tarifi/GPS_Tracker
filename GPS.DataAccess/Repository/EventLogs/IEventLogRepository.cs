using GPS.Domain.DTO;
using GPS.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.DataAccess.Repository.EventLogs
{
   public interface IEventLogRepository
    {
        Task LogEventAsync(Event Type, Object ObjectId, Object Data, string UserId);

        Task<PagedResult<EventLog>> SearchAsync(string type, DateTime? fromDate, DateTime? toDate, string searchString, int pageNumber, int PpageSize);
    }
}
