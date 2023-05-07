using GPS.DataAccess.Context;
using GPS.Domain.DTO;
using GPS.Domain.Models;
using GPS.Domain.Views;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.DataAccess.Repository.Sensors
{
    public class SensorRepository : ISensorRepository
    {
        private readonly TrackerDBContext _dbContext;

        public SensorRepository(TrackerDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PagedResult<Sensor>> SearchAsync(List<string> sensorSNs, int? SensorStatus, int? BrandId, string SearchString, int PageNumber, int pageSize)
        {
            var pagedList = new PagedResult<Sensor>();
            var skip = (PageNumber - 1) * pageSize;
            var listWorkingSensorSN = new List<string>();
            var listNotWorkingSensorSN = new List<string>();
            // 0 not working sensors
            if (SensorStatus == 0)
            {
                var _tempWorkingSensors = await _dbContext.OnlineInventoryHistory
                   .Where(x => x.GpsDate > DateTime.Now.AddDays(-1) && (sensorSNs.Count == 0 || sensorSNs.Contains(x.Serial))).Select(x => x.Serial).ToListAsync();
                listNotWorkingSensorSN = sensorSNs.Except(_tempWorkingSensors).ToList();
            }
            // 1 working sensors
            if (SensorStatus == 1)
            {
                listWorkingSensorSN = await _dbContext.OnlineInventoryHistory
                    .Where(x => x.GpsDate > DateTime.Now.AddDays(-1) && (sensorSNs.Count == 0 || sensorSNs.Contains(x.Serial))).Select(x => x.Serial).ToListAsync();
            }
            pagedList.TotalRecords = await _dbContext.Sensor.Where(x => !x.IsDeleted && (!BrandId.HasValue || x.BrandId == BrandId)
            && (!SensorStatus.HasValue || listNotWorkingSensorSN.Contains(x.Serial)|| listWorkingSensorSN.Contains(x.Serial)) &&
            (sensorSNs.Count == 0 || sensorSNs.Contains(x.Serial)) &&
            (string.IsNullOrEmpty(SearchString) || (x.Name.Contains(SearchString) || x.Name.Contains(SearchString)
            || x.Serial.Contains(SearchString))
            )).GroupBy(x => x.Serial).CountAsync();

            var _tempList= await _dbContext.Sensor.Where(x => !x.IsDeleted && (!BrandId.HasValue || x.BrandId == BrandId)
            && (!SensorStatus.HasValue || listNotWorkingSensorSN.Contains(x.Serial) || listWorkingSensorSN.Contains(x.Serial)) &&
            (sensorSNs.Count == 0 || sensorSNs.Contains(x.Serial)) &&
                 (string.IsNullOrEmpty(SearchString) || (x.Name.Contains(SearchString) || x.Name.Contains(SearchString)
                 || x.Serial.Contains(SearchString))))
                    .OrderByDescending(x => x.CreatedDate)
                    .Skip(skip).Take(pageSize)
                    .Include(x => x.Brand)
                    .AsNoTracking().ToListAsync();
            pagedList.List = _tempList.GroupBy(x => x.Serial).Select(x => x.First()).ToList();

            return pagedList;
        }

        public async Task<Sensor> FindbyIdAsync(long? Id)
        {
            var sensor = await _dbContext.Sensor.Where(x => !x.IsDeleted && x.Id == Id)
               .Include(x => x.Brand)
                .AsNoTracking().FirstOrDefaultAsync();

            return sensor;
        }

        public async Task<Sensor> AddAsync(Sensor sensor)
        {
           var obj = await _dbContext.Sensor.AddAsync(sensor);
            await _dbContext.SaveChangesAsync();
            return obj.Entity;
        }
        public async Task<bool> AddRangeAsync(List<Sensor> sensorsList)
        {
            await _dbContext.Sensor.AddRangeAsync(sensorsList);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAsync(SensorView sensorView)
        {
            var sensor = await _dbContext.Sensor.FindAsync(sensorView.Id);
            if (sensor == null)
            {
                return false;
            }

            sensor.BrandId = sensorView.BrandId;
            sensor.Name = sensorView.Name;
            sensor.Serial = sensorView.Serial;
            sensor.UpdatedBy = sensorView.UpdatedBy;
            sensor.UpdatedDate = DateTime.Now;
            sensor.CalibrationDate = sensorView.CalibrationDate;
            sensor.DueDate= sensorView.DueDate;
            _dbContext.Sensor.Update(sensor);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<Sensor> DeleteAsync(long Id, string UpdatedBy)
        {
            var sensor = await _dbContext.Sensor.FindAsync(Id);
            if (sensor == null)
            {
                return null;
            }

            sensor.IsDeleted = true;
            sensor.UpdatedBy = UpdatedBy;
            sensor.UpdatedDate = DateTime.Now;

            var deleted = _dbContext.Sensor.Update(sensor);
            await _dbContext.SaveChangesAsync();

            return deleted.Entity;
        }

        public async Task<int> CountAsync(int? BrandId)
        {
            int Count = await _dbContext.Sensor.Where(x => !x.IsDeleted && (!BrandId.HasValue || x.BrandId == BrandId)).CountAsync();
            return Count;
        }

        public async Task<List<Sensor>> GetAllAsync(int? BrandID)
        {
            var sensor = await _dbContext.Sensor.Where(x => !x.IsDeleted &&
            (!BrandID.HasValue || x.BrandId == BrandID))
               .AsNoTracking().ToListAsync();

            return sensor;
        }

        public async Task<bool> IsSensorExistsAsync(string SensorSN)
        {
            return await _dbContext.Sensor.AnyAsync(x => x.Serial == SensorSN);
        }
        public async Task<List<Sensor>> FindSensorsAsync(List<long> sensorIds)
        {
            return await _dbContext.Sensor.Where(x => sensorIds.Contains(x.Id)).ToListAsync();
        }

        public async Task<Sensor> FindSensorBySerialNumberAsync(string serialNumber)
        {
            return await _dbContext.Sensor.Where(x => x.Serial == serialNumber).FirstOrDefaultAsync();
        }

        public async Task<List<string>> AllSensorsSerialNumber()
        {
            return await _dbContext.Sensor.Select(x => x.Serial).ToListAsync();
        }

        public async Task<List<Sensor>> AllSensorsBySerialNumberAsync(List<string> lsSerial)
        {
            return await _dbContext.Sensor.Where(x => lsSerial.Any(sn => sn == x.Serial)).AsNoTracking().ToListAsync();
        }
    }
}
