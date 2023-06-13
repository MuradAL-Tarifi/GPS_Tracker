using GPS.DataAccess.Context;
using GPS.Domain.Views;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.DataAccess.Repository.Smtpchecker
{
    public class SmtpcheckerRepository : ISmtpcheckerRepository
    {
        private readonly TrackerDBContext _dbContext;

        public SmtpcheckerRepository(TrackerDBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Domain.Models.Smtpchecker> AddAsync(Domain.Models.Smtpchecker smtpchecker)
        {
            var obj = await _dbContext.Smtpchecker.AddAsync(smtpchecker);
            await _dbContext.SaveChangesAsync();
            return obj.Entity;
        }
        public async Task<bool> UpdateAsync(AlertSensorView alertSensorView)
        {
            var alertSensor = await _dbContext.AlertBySensor.FindAsync(alertSensorView.Id);
            var smtpcheckerResult = await _dbContext.Smtpchecker.Where(x=>x.Serial.Equals(alertSensor.Serial)).FirstOrDefaultAsync();
            if (smtpcheckerResult == null)
            {
                return false;
            }
            smtpcheckerResult.Serial = alertSensorView.Serial;


            _dbContext.Smtpchecker.Update(smtpcheckerResult);
            return await _dbContext.SaveChangesAsync() > 0;
        }
        public async Task<Domain.Models.Smtpchecker> DeleteAsync(long itemId)
        {
            var alertSensor = await _dbContext.AlertBySensor.FindAsync(Convert.ToInt32(itemId));
            var smtpcheckerResult = await _dbContext.Smtpchecker.Where(x => x.Serial.Equals(alertSensor.Serial)).FirstOrDefaultAsync();
            if (smtpcheckerResult == null)
            {
                return null;
            }

            var deleted = _dbContext.Smtpchecker.Remove(smtpcheckerResult);
            await _dbContext.SaveChangesAsync();

            return deleted.Entity;
        }
    }
}
