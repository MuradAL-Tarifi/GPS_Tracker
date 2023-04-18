using GPS.DataAccess.Context;
using GPS.Domain.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace GPS.DataAccess.Repository.Warehouses
{
    public class WaslIntegrationLogRepository : IWaslIntegrationLogRepository
    {
        private readonly TrackerDBContext _dbContext;

        public WaslIntegrationLogRepository(TrackerDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(WaslIntegrationLog waslIntegrationLog)
        {
            waslIntegrationLog.LogDate = DateTime.Now;
            await _dbContext.WaslIntegrationLog.AddAsync(waslIntegrationLog);
            await _dbContext.SaveChangesAsync();
        }
    }
}
