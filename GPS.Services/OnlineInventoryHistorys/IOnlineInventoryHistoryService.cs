using GPS.Domain.DTO;
using GPS.Domain.ViewModels;
using GPS.Domain.Views;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GPS.Services.OnlineInventoryHistorys
{
    public interface IOnlineInventoryHistoryService
    {
        Task<ReturnResult<List<OnlineInventoryHistoryView>>> SearchAsync(long fleetId, long? warehouseId = null, long? inventoryId = null);

        Task<ReturnResult<List<WarehouseView>>> GetByUserIdAsync(string userId);

        Task<ReturnResult<List<InventorySensorTemperatureModel>>> GetOnlineSensorTempertureAsync(long inventoryId);

        Task<ReturnResult<List<OnlineInventorySensors>>> GetOnlineInventoriesSensorData(List<long> inventoryIds);

        //Task<ReturnResult<OnlineInventoryHistoryDto[]>> AllInvetoryHistoy();
    }
}
