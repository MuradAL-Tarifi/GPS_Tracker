using GPS.Domain.DTO;
using GPS.Domain.Models;
using GPS.Domain.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.DataAccess.Repository.Sensors
{
   public interface ISensorRepository
    {
        Task<PagedResult<Sensor>> SearchAsync(List<string> sensorSNs, int? SensorStatus, int? BrandId, string SearchString, int PageNumber, int pageSize);

        Task<Sensor> FindbyIdAsync(long? Id);

        Task<Sensor> AddAsync(Sensor sensor);

        Task<bool> AddRangeAsync(List<Sensor> sensorsList);

        Task<bool> UpdateAsync(SensorView groupView);

        Task<Sensor> DeleteAsync(long Id, string UpdatedBy);

        Task<int> CountAsync(int? BrandId);

        Task<List<Sensor>> GetAllAsync(int? BrandId);

        Task<bool> IsSensorExistsAsync(string SensorSN);
        Task<List<Sensor>> FindSensorsAsync(List<long> sensorIds);
        Task<Sensor> FindSensorBySerialNumberAsync(string serialNumber);
        Task<List<string>> AllSensorsSerialNumber();
        Task<List<Sensor>> AllSensorsBySerialNumberAsync(List<string> lsSerial);
    }
}
