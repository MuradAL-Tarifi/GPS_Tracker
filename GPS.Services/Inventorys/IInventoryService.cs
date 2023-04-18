using GPS.Domain.DTO;
using GPS.Domain.Views;
using System.Threading.Tasks;

namespace GPS.Services.Inventorys
{
    public interface IInventoryService
    {
        Task<ReturnResult<PagedResult<InventoryView>>> SearchAsync(long? FleetId = null, long? WarehouseId = null, int? waslLinkStatus = null, int? isActive = null, string SearchString = "", int PageNumber = 1, int pageSize = 100);
        Task<ReturnResult<InventoryView>> FindByIdAsync(long? Id);
        Task<ReturnResult<InventoryView>> SaveAsync(InventoryView model);
        Task<ReturnResult<bool>> DeleteAsync(long Id, string UpdatedBy);
        Task<ReturnResult<bool>> IsInventoryNumberExists(long fleetId, string inventoryNumber);
        Task<ReturnResult<bool>> LinkWithWasl(long id, string updatedBy);
        Task<ReturnResult<bool>> UnlinkWithWasl(long id, string updatedBy);
        Task<bool> IsInventorySensorExistsAsync(long sensorId);
    }
}
