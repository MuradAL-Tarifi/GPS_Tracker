using GPS.DataAccess.Context;
using GPS.Domain.DTO;
using GPS.Domain.Models;
using GPS.Domain.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.DataAccess.Repository.Alerts
{
   public class AlertsRepository : IAlertsRepository
    {
        private readonly TrackerDBContext _dbContext;

        public AlertsRepository(TrackerDBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Alert> AddAsync(Alert alert)
        {
            alert.CreatedDate = DateTime.Now;
            var added = await _dbContext.Alert.AddAsync(alert);
            await _dbContext.SaveChangesAsync();
            return added.Entity;
        }

        public async Task<List<Alert>> GetTop100AlertsAsync(string userId)
        {
           return await _dbContext.Alert.Where(x => x.CustomAlert.UserIds.Contains(userId))
                .Include(x => x.CustomAlert)
                .OrderByDescending(x => x.Id)
                .Take(100)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<int> NumberOfUnViewedAlerts(string userId)
        {
            var alertIds = await _dbContext.Alert.Where(x => x.CustomAlert.UserIds.Contains(userId)).Include(x => x.CustomAlert).Select(x => x.Id)
                .ToListAsync();
            var viewedAlerts = await _dbContext.AlertByUserWatcher.Where(x => x.UserId == userId).Select(x => x.AlertId)
                .ToListAsync();

            return alertIds.Union(viewedAlerts).Except(alertIds.Intersect(viewedAlerts)).Count();
        }

        public async Task<PagedResult<Alert>> SearchAsync(string userId, long? warehouseId, long? inventoryId, long? sensorId, long? alertType, long? alertId, DateTime? fromDate, DateTime? toDate, int pageNumber, int pageSize)
        {
            var pagedList = new PagedResult<Alert>();
            var skip = (pageNumber - 1) * pageSize;

            pagedList.TotalRecords = await _dbContext.Alert
                .Where(x => x.CustomAlert.UserIds.Contains(userId) && 
                (!warehouseId.HasValue || x.WarehouseId == warehouseId) && 
                (!inventoryId.HasValue || x.InventoryId == inventoryId) &&
                (!sensorId.HasValue || x.SensorId == sensorId) &&
                (!alertId.HasValue || x.Id == alertId) &&
                (!alertType.HasValue || x.AlertTypeLookupId == alertType) &&
                (!fromDate.HasValue || x.AlertDateTime >= fromDate) &&
                (!toDate.HasValue || x.AlertDateTime <= toDate)
                ).Include(x => x.CustomAlert).CountAsync();

            pagedList.List = await _dbContext.Alert
                .Where(x => x.CustomAlert.UserIds.Contains(userId) &&
                (!warehouseId.HasValue || x.WarehouseId == warehouseId) &&
                (!inventoryId.HasValue || x.InventoryId == inventoryId) &&
                (!sensorId.HasValue || x.SensorId == sensorId) &&
                (!alertId.HasValue || x.Id == alertId) &&
                (!alertType.HasValue || x.AlertTypeLookupId == alertType) &&
                (!fromDate.HasValue || x.AlertDateTime >= fromDate) &&
                (!toDate.HasValue || x.AlertDateTime <= toDate)
                ).Include(x => x.CustomAlert)
                .OrderByDescending(x => x.AlertDateTime)
                .Skip(skip).Take(pageSize)
                .ToListAsync();
            return pagedList;
        }

        public async Task<bool> UpdateAlertsAsReadAsync(List<long> alertIds, string userId)
        {
            List<long> alertsByUsre =  await _dbContext.AlertByUserWatcher.Where(x => alertIds.Contains(x.AlertId) && x.UserId == userId).Select(x => x.AlertId).ToListAsync();
            List<long> filteredAlertIds = alertIds.Except(alertsByUsre).ToList();
            var lsAlertByUserWatcher = new List<AlertByUserWatcher>();
            foreach (var alertId in filteredAlertIds)
            {
                lsAlertByUserWatcher.Add(
                    new AlertByUserWatcher
                    {
                        UserId = userId,
                        AlertId = alertId,
                        ViewDate = DateTime.Now
                    }
                );
            }

            if (lsAlertByUserWatcher.Count > 0)
            {
                await _dbContext.AlertByUserWatcher.AddRangeAsync(lsAlertByUserWatcher);
                await _dbContext.SaveChangesAsync();
            }
            return true;
        }
    }
}
