using GPS.DataAccess.Context;
using GPS.Domain.Models;
using GPS.Domain.Views;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.DataAccess.Repository.SensorAlerts
{
    public class SensorAlertRepository : ISensorAlertRepository
    {
        private readonly TrackerDBContext _dbContext;

        public SensorAlertRepository(TrackerDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<SensorAlertTypeLookup>> GetAlertTypeLookupsAsync()
        {
            var alertTypes = await _dbContext.SensorAlertTypeLookup.OrderByDescending(x => x.RowOrder).ToListAsync();
            return alertTypes;
        }

        //public async Task<Alert> GetLastAlertBySensorId(long sensorId)
        //{
        //    return await _dbContext.Alert.Where(x => x.SensorId == sensorId).LastOrDefaultAsync();
        //}

        //public async Task<List<SensorAlert>> GetByInventorySensorIdAsync(long inventorySensorId)
        //{
        //    var sensorAlerts = await _dbContext.SensorAlert.Where(x => x.InventorySensorId == inventorySensorId).ToListAsync();
        //    return sensorAlerts;
        //}

        public async Task<DateTime?> LastSensorAlertDateAsync(long sensorId)
        {
            return await _dbContext.SensorAlertHisotry.Where(x => x.SensorId == sensorId).OrderBy(x => x.Id).Select(x => x.LastAlertDate).LastOrDefaultAsync();
        }

        //public async Task<bool> SaveListAsync(List<long> inventorySensorIds, List<SensorAlertView> sensorAlerts)
        //{
        //    foreach (var inventorySensorId in inventorySensorIds)
        //    {
        //        foreach (var inventorySensorItem in sensorAlerts)
        //        {
        //            // check if exists ? update : add
        //            var sensorAlert = await _dbContext.SensorAlert.Where(x => x.InventorySensorId == inventorySensorId && x.SensorAlertTypeLookupId == inventorySensorItem.SensorAlertTypeLookupId).FirstOrDefaultAsync();
        //            if (sensorAlert != null)
        //            {
        //                sensorAlert.IsActive = inventorySensorItem.IsActive;
        //                sensorAlert.IsSMS = inventorySensorItem.IsSMS;
        //                sensorAlert.IsEmail = inventorySensorItem.IsEmail;
        //                sensorAlert.FromValue = inventorySensorItem.FromValue;
        //                sensorAlert.ToValue = inventorySensorItem.ToValue;
        //                await _dbContext.SaveChangesAsync();
        //            }
        //            else
        //            {
        //                var newSensorAlert = new SensorAlert()
        //                {
        //                    InventorySensorId = inventorySensorId,
        //                    SensorAlertTypeLookupId = inventorySensorItem.SensorAlertTypeLookupId,
        //                    IsActive = inventorySensorItem.IsActive,
        //                    IsSMS = inventorySensorItem.IsSMS,
        //                    IsEmail = inventorySensorItem.IsEmail,
        //                    FromValue = inventorySensorItem.FromValue,
        //                    ToValue = inventorySensorItem.ToValue
        //                };

        //                _dbContext.SensorAlert.Add(newSensorAlert);
        //                await _dbContext.SaveChangesAsync();
        //            }
        //        }
        //    }

        //    return true;
        //}

        public async Task UpdateLastAlertDateBySensorIdAsync(long customAlertId, long sensorId, DateTime alertTime)
        {
            await _dbContext.SensorAlertHisotry.AddAsync(new SensorAlertHisotry
            {
                CustomerAlertId = customAlertId,
                SensorId = sensorId,
                LastAlertDate = alertTime
            });
            await _dbContext.SaveChangesAsync();
        }
    }
}
