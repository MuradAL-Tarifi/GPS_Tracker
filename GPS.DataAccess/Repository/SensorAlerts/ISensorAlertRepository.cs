using GPS.Domain.Models;
using GPS.Domain.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.DataAccess.Repository.SensorAlerts
{
    public interface ISensorAlertRepository
    {
        Task<List<SensorAlertTypeLookup>> GetAlertTypeLookupsAsync();

        //Task<List<SensorAlert>> GetByInventorySensorIdAsync(long inventorySensorId);

        //Task<bool> SaveListAsync(List<long> inventorySensorIds, List<SensorAlertView> sensorAlerts);

        Task<DateTime?> LastSensorAlertDateAsync(long sensorId);

        Task UpdateLastAlertDateBySensorIdAsync(long customAlertId, long sensorId, DateTime alertTime);

        //Task<Alert> GetLastAlertBySensorId(long sensorId);
    }
}
