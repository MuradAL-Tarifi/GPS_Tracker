using GPS.DataAccess.Context;
using GPS.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.DataAccess.Repository.EmailHistorys
{
    public class EmailHistoryRepository : IEmailHistoryRepository
    {
        private readonly TrackerDBContext _dbContext;

        public EmailHistoryRepository(TrackerDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        //public async Task<EmailHistory> SearchAsync(long VehicleId)
        //{
        //    var emailHistory = await _dbContext.EmailHistory
        //        .Where(x => x.VehicleId == VehicleId)
        //        .OrderByDescending(x => x.CreatedDate)
        //        .FirstOrDefaultAsync();

        //    return emailHistory;
        //}

        //public async Task<EmailHistory> GetForVehicleCcustomAlertAsync(long VehicleId)
        //{
        //    var emailHistory = await _dbContext.EmailHistory
        //        .Where(x => x.VehicleId == VehicleId)
        //        .OrderByDescending(x => x.SentDate)
        //        .FirstOrDefaultAsync();

        //    return emailHistory;
        //}

        //public async Task<EmailHistory> GetForInventorySensorCcustomAlertAsync(long inventorySensorId)
        //{
        //    var emailHistory = await _dbContext.EmailHistory
        //        .Where(x => x.InventorySensorId == inventorySensorId)
        //        .OrderByDescending(x => x.SentDate)
        //        .FirstOrDefaultAsync();

        //    return emailHistory;
        //}

        public async Task<long> AddAsync(EmailHistory entity)
        {
            entity.CreatedDate = DateTime.Now;
            await _dbContext.EmailHistory.AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<bool> UpdateSentAsync(long id)
        {
            var entity = await _dbContext.EmailHistory.FindAsync(id);
            if (entity == null)
            {
                return false;
            }

            entity.IsSent = true;
            entity.SentDate = DateTime.Now;

            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
