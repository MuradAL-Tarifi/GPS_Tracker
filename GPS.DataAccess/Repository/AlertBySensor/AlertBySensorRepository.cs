using GPS.DataAccess.Context;
using GPS.Domain.DTO;
using GPS.Domain.Models;
using GPS.Domain.ViewModels;
using GPS.Domain.Views;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.DataAccess.Repository.AlertBySensor
{
    public class AlertBySensorRepository : IAlertBySensorRepository
    {
        private readonly TrackerDBContext _dbContext;

        public AlertBySensorRepository(TrackerDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> IsAlertBySensorExistsAsync(string sensorSN)
        {
            return await _dbContext.AlertBySensor.AnyAsync(x => x.Serial.Equals(sensorSN));
        }
        public async Task<Domain.Models.AlertBySensor> FindbyIdAsync(long? Id)
        {
            var sensor = await _dbContext.AlertBySensor.Where(x =>x.Id == Id)
               .Include(x => x.Inventory).Include(x=>x.Warehouse)
                .AsNoTracking().FirstOrDefaultAsync();

            return sensor;
        }
        public async Task<PagedResult<Domain.Models.AlertBySensor>> SearchAsync(long? warehouseId, long? InventoryId, string Serial, int pageNumber, int pageSize)
        {
            var pagedList = new PagedResult<GPS.Domain.Models.AlertBySensor>();

            var skip = (pageNumber - 1) * pageSize;

            pagedList.TotalRecords = await _dbContext.AlertBySensor.Where(x =>
            (!warehouseId.HasValue || x.WarehouseId == warehouseId) &&
            (!InventoryId.HasValue || x.InventoryId == InventoryId) &&
            ((string.IsNullOrEmpty(Serial) || (x.Serial.Equals(Serial))))).CountAsync();

            pagedList.List = await _dbContext.AlertBySensor.Where(x =>
             (!warehouseId.HasValue || x.WarehouseId == warehouseId) &&
            (!InventoryId.HasValue || x.InventoryId == InventoryId) &&
            ((string.IsNullOrEmpty(Serial) || (x.Serial.Contains(Serial)))))
                    .OrderByDescending(x => x.CreatedDate)
                    .Skip(skip).Take(pageSize)
                    .Include(x => x.Warehouse)
                    .ThenInclude(x => x.Fleet)
                    .Include(x => x.Inventory)
                    .AsNoTracking().ToListAsync();

            return pagedList;
        }

        public async Task<Domain.Models.AlertBySensor> AddAsync(Domain.Models.AlertBySensor alertSensor)
        {
            var obj = await _dbContext.AlertBySensor.AddAsync(alertSensor);
            await _dbContext.SaveChangesAsync();
            return obj.Entity;
        }

        public async Task<bool> UpdateAsync(AlertSensorView alertSensorView)
        {
            var alertSensor = await _dbContext.AlertBySensor.FindAsync(alertSensorView.Id);
            if (alertSensor == null)
            {
                return false;
            }

            alertSensor.Serial = alertSensorView.Serial;
            alertSensor.Interval = alertSensorView.Interval;
            alertSensor.UserName = alertSensorView.UserName;
            alertSensor.ToEmails = alertSensorView.ToEmails;
            alertSensor.MaxValueTemperature = alertSensorView.MaxValueTemperature;
            alertSensor.MinValueTemperature = alertSensorView.MinValueTemperature;
            alertSensor.MinValueHumidity = alertSensorView.MinValueTemperature;
            alertSensor.MaxValueHumidity = alertSensorView.MinValueTemperature;
            alertSensor.InventoryId= alertSensorView.InventoryId;
            alertSensor.WarehouseId= alertSensorView.WarehouseId;
            alertSensor.AlertTypeLookupId= alertSensorView.AlertTypeLookupId;
            
            _dbContext.AlertBySensor.Update(alertSensor);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<Domain.Models.AlertBySensor> DeleteAsync(long itemId)
        {
            var alertSensor = await _dbContext.AlertBySensor.FindAsync(Convert.ToInt32(itemId));
            if (alertSensor == null)
            {
                return null;
            }

            var deleted = _dbContext.AlertBySensor.Remove(alertSensor);
            await _dbContext.SaveChangesAsync();

            return deleted.Entity;
        }
    }
}
