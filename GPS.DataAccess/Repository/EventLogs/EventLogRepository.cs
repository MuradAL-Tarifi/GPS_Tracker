using GPS.DataAccess.Context;
using GPS.Domain.DTO;
using GPS.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.DataAccess.Repository.EventLogs
{
    public class EventLogRepository : IEventLogRepository
    {
        private readonly TrackerDBContext _dbContext;

        public EventLogRepository(TrackerDBContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task LogEventAsync(Event type, Object objectId, Object data, string userId)
        {
            if (userId == "System")
                return;
            EventLog eventLog = new EventLog()
            {
                Type = type.ToString(),
                ObjectId = objectId.ToString(),
                ObjectType = data.GetType().FullName,
                Data = JsonConvert.SerializeObject(data, Formatting.Indented, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }),
                UserId = userId,
                CreationDate = DateTime.Now
            };

            await _dbContext.EventLog.AddAsync(eventLog);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<PagedResult<EventLog>> SearchAsync(string type, DateTime? fromDate, DateTime? toDate, string searchString, int pageNumber, int pageSize)
        {

            var pagedList = new PagedResult<EventLog>();
            var skip = (pageNumber - 1) * pageSize;

            pagedList.TotalRecords = await _dbContext.EventLog.Where(x =>
             (string.IsNullOrEmpty(type) || x.Type == type) &&
            (!fromDate.HasValue || x.CreationDate.Date >= fromDate.Value.Date) &&
             (!toDate.HasValue || x.CreationDate.Date <= toDate.Value.Date) &&
            (string.IsNullOrEmpty(searchString) || (x.User.Name.Contains(searchString) || x.User.NameEn.Contains(searchString) || x.ObjectId.Contains(searchString) || x.ObjectType.Contains(searchString))))
                .CountAsync();

            pagedList.List = await _dbContext.EventLog.Where(x =>
            (string.IsNullOrEmpty(type) || x.Type == type) &&
            (!fromDate.HasValue || x.CreationDate.Date >= fromDate.Value.Date) &&
             (!toDate.HasValue || x.CreationDate.Date <= toDate.Value.Date) &&
            (string.IsNullOrEmpty(searchString) || (x.User.Name.Contains(searchString) || x.User.NameEn.Contains(searchString) || x.ObjectId.Contains(searchString) || x.ObjectType.Contains(searchString))))
                .Include(x => x.User)
                .OrderByDescending(x => x.CreationDate)
                .Skip(skip).Take(pageSize)
                .AsNoTracking().ToListAsync();

            return pagedList;
        }
    }
}
