using GPS.DataAccess.Context;
using GPS.Domain.Models;
using GPS.Domain.Views;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GPS.DataAccess.Repository.Inventorys
{
    public class InventorySensorRepository : IInventorySensorRepository
    {
        private readonly TrackerDBContext _dbContext;

        public InventorySensorRepository(TrackerDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<InventorySensor>> GetInventorySensorList(long inventoryId)
        {
            var sensorList = await _dbContext.InventorySensor
                .Include(x => x.Inventory).ThenInclude(x => x.Warehouse).ThenInclude(x => x.Fleet)
                .Include(x => x.Inventory).ThenInclude(x => x.Gateway)
                .Where(x => !x.IsDeleted && x.InventoryId == inventoryId
                //&& (!sensorSerial.HasValue || sensorSerial == x.Serial)
                )
                .AsNoTracking()
                .ToListAsync();

            return sensorList;
        }

        public async Task<InventorySensor> GetInventorySensor(long inventoryId, string sensorSerial)
        {
            return await _dbContext.InventorySensor
                  .Include(x => x.Sensor)
                  .Include(x => x.Inventory).ThenInclude(x => x.Warehouse).ThenInclude(x => x.Fleet)
                  .Include(x => x.Inventory).ThenInclude(x => x.Gateway)

                  .FirstOrDefaultAsync(x => !x.IsDeleted && x.InventoryId == inventoryId && sensorSerial == x.Sensor.Serial);
        }

        public async Task<InventorySensor> GetBasicBySerial(string serial)
        {
            var sensor = await _dbContext.InventorySensor.Include(x =>x.Sensor).FirstOrDefaultAsync(x => !x.IsDeleted && x.Sensor.Serial == serial);
            return sensor;
        }

        public async Task<List<InventorySensor>> GetByInventoryId(long inventoryId)
        {
            var sensorList = await _dbContext.InventorySensor
                .Where(x => !x.IsDeleted && x.InventoryId == inventoryId)
                .Include(x => x.Inventory)
                .Include(x => x.Sensor)
                .AsNoTracking()
                .ToListAsync();

            return sensorList;
        }

        public async Task<List<InventorySensor>> GetBasicByInventoryId(long inventoryId)
        {
            var sensorList = await _dbContext.InventorySensor
                .Where(x => !x.IsDeleted && x.InventoryId == inventoryId)
                .Include(x=>x.Sensor)
                .AsNoTracking()
                .ToListAsync();

            return sensorList;
        }

        public async Task<bool> IsInventorySensorExistsAsync(long sensorId)
        {
            return await _dbContext.InventorySensor.AnyAsync(x => x.SensorId == sensorId);
        }

        public async Task<InventorySensor> DeleteSensorFromInventoryAsync(long sensorId, string updatedBy)
        {
            var inventorySensor = await _dbContext.InventorySensor.Where(x => x.SensorId == sensorId).FirstOrDefaultAsync();
            if (inventorySensor == null)
            {
                return null;
            }
            inventorySensor.IsDeleted = true;
            inventorySensor.UpdatedBy = updatedBy;
            inventorySensor.UpdatedDate = DateTime.Now;

            _dbContext.InventorySensor.Attach(inventorySensor);
            _dbContext.Entry(inventorySensor).State = EntityState.Modified;
            _dbContext.Entry(inventorySensor).Property(x => x.IsDeleted).IsModified = true;
            _dbContext.Entry(inventorySensor).Property(x => x.UpdatedBy).IsModified = true;
            _dbContext.Entry(inventorySensor).Property(x => x.UpdatedDate).IsModified = true;
           
            await _dbContext.SaveChangesAsync();

            return inventorySensor;
        }
        public async Task<List<InventorySensor>> GetListInventorySensor(List<long> sensorIds)
        {
            return await _dbContext.InventorySensor.Where(x => sensorIds.Contains(x.SensorId)).Include(x => x.Inventory).ToListAsync();
        }

        public async Task<List<InventorySensor>> GetListInventorySensorByInventoryIdsAsync(List<long> inventoryIds)
        {
            return await _dbContext.InventorySensor.Where(x => inventoryIds.Contains(x.InventoryId)).Include(x => x.Sensor).ToListAsync();
        }
    }
}
