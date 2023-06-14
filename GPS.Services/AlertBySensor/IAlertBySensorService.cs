using GPS.Domain.DTO;
using GPS.Domain.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Services.AlertBySensor
{
    public interface IAlertBySensorService
    {
        
        Task<bool> IsAlertBySensorExistsAsync(string sensorSN);
        Task<ReturnResult<PagedResult<AlertSensorView>>> SearchAsync(long? warehouseId, long? InventoryId, string Serial, string search, int pageNumber, int pageSize);
        Task<ReturnResult<AlertSensorView>> FindbyId(long? id);
        Task<ReturnResult<bool>> SaveAsync(AlertSensorView model, string userId);
        Task<ReturnResult<bool>> Delete(long itemId, string userId);
    }
}
