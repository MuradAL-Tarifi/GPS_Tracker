using GPS.Domain.Models;
using GPS.Domain.Views;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GPS.DataAccess.Repository.Inventorys
{
    public interface IInventoryHistoryRepository
    {
        Task<int> InsertAsync(InventoryHistoryView inventoryHistoryView);

        Task<List<InventoryHistory>> GetByInventoryIdAsync(long sensorSerial, DateTime fromDate, DateTime toDate);

        Task<List<InventoryHistory>> GetByInventoryIdAndSensorSerialAsync(long InventoryId, string sensorSerial, DateTime fromDate, DateTime toDate);
        Task<List<InventoryHistory>> GetBySensorsSerialsAsync(List<string> sensorSerial, DateTime fromDate, DateTime toDate);
    }
}
