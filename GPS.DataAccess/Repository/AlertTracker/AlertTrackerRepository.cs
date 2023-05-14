using GPS.DataAccess.Context;
using GPS.Domain.DTO;
using GPS.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.DataAccess.Repository.AlertTracker
{
    public class AlertTrackerRepository : IAlertTrackerRepository
    {
        private readonly TrackerDBContext _dbContext;

        public AlertTrackerRepository(TrackerDBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<PagedResult<GPS.Domain.Models.AlertTracker>> SearchAsync(string warehouseName, string fleetName, string sensorNumber, DateTime? fromDate, DateTime? toDate, int pageNumber, int pageSize)
        {
            var pagedList = new PagedResult<GPS.Domain.Models.AlertTracker>();
            var skip = (pageNumber - 1) * pageSize;

            pagedList.TotalRecords = await _dbContext.AlertTracker
                .Where(x =>
                (warehouseName == null || x.WarehouseName.Contains(warehouseName)) &&
                (fleetName == null || x.Zone.Contains(fleetName)) &&
                (sensorNumber == null || x.Serial.Contains(sensorNumber)) &&
                (!fromDate.HasValue || x.AlertDateTime >= fromDate) &&
                (!toDate.HasValue || x.AlertDateTime <= toDate)
                )
                .CountAsync();
            pagedList.List = await _dbContext.AlertTracker
                .Where(x =>
                (warehouseName == null || x.WarehouseName.Equals(warehouseName)) &&
                (fleetName == null || x.Zone.Equals(fleetName)) &&
                (sensorNumber == null || x.Serial.Equals(sensorNumber)) &&
                (!fromDate.HasValue || x.AlertDateTime >= fromDate) &&
                (!toDate.HasValue || x.AlertDateTime <= toDate)
                )
                .OrderByDescending(x => x.AlertDateTime)
                .Skip(skip).Take(pageSize)
                .ToListAsync();
            return pagedList;
        }

    }
}
