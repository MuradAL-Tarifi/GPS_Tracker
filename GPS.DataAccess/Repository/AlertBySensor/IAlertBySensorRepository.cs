using GPS.Domain.DTO;
using GPS.Domain.Models;
using GPS.Domain.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.DataAccess.Repository.AlertBySensor
{
    public interface IAlertBySensorRepository
    {
        Task<Domain.Models.AlertBySensor> AddAsync(Domain.Models.AlertBySensor alertSensor);
        Task<Domain.Models.AlertBySensor> DeleteAsync(long itemId);
        Task<Domain.Models.AlertBySensor> FindbyIdAsync(long? Id);
        Task<bool> IsAlertBySensorExistsAsync(string sensorSN);
        Task<PagedResult<GPS.Domain.Models.AlertBySensor>> SearchAsync(long? warehouseId, long? InventoryId, string Serial, string search, int pageNumber, int pageSize);
        Task<bool> UpdateAsync(AlertSensorView alertSensor);
    }
}
