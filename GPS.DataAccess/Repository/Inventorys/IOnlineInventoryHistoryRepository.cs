using GPS.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.DataAccess.Repository.Inventorys
{
    public interface IOnlineInventoryHistoryRepository
    {
        Task<int> InsertAsync(OnlineInventoryHistory onlineInventoryHistory);

        Task<int> UpdateAsync(OnlineInventoryHistory onlineInventoryHistory);

        Task<OnlineInventoryHistory> GetBySensorSerialAsync(string sensorSerial);

        Task<(List<InventorySensor>, List<OnlineInventoryHistory>)> SearchAsync(long fleetId, long? warehouseId, long? inventoryId);
    }
}
