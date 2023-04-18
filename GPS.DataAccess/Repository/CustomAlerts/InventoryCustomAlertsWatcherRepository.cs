using GPS.DataAccess.Context;
using GPS.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.DataAccess.Repository.CustomAlerts
{
   public class InventoryCustomAlertsWatcherRepository: IInventoryCustomAlertsWatcherRepository
    {
        private readonly TrackerDBContext _dbContext;
        public InventoryCustomAlertsWatcherRepository(TrackerDBContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task<CustomAlertWatcher> GetLastAsync()
        {
            var entity = await _dbContext.CustomAlertWatcher.OrderBy(x => x.Id).LastOrDefaultAsync();
            return entity;
        }

        public async Task AddAsync(DateTime fromDate, DateTime toDate)
        {
            await _dbContext.CustomAlertWatcher.AddAsync(new CustomAlertWatcher()
            {
                FromDate = fromDate,
                ToDate = toDate,
                CreatedDate = DateTime.Now
            });
            await _dbContext.SaveChangesAsync();
        }
    }
}
