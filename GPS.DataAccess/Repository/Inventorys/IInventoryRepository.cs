using GPS.Domain.DTO;
using GPS.Domain.Models;
using GPS.Domain.Views;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GPS.DataAccess.Repository.Inventorys
{
    public interface IInventoryRepository
    {
        Task<PagedResult<Inventory>> SearchAsync(long? FleetId, long? WarehouseId, int? waslLinkStatus, int? IsActive, string SearchString, int PageNumber, int pageSize);

        Task<Inventory> FindByIdAsync(long? Id);

        Task<Inventory> AddAsync(Inventory inventory, List<InventorySensor> list);

        Task<Inventory> UpdateAsync(InventoryView model);

        Task<Inventory> DeleteAsync(long Id, string UpdatedBy);

        Task<bool> UpdateLinkedWithWaslInfoAsync(Inventory inventory);

        Task<List<Inventory>> GetByWarehouseIdAsync(long warehouseId);

        Task<bool> IsAnyLinkedWithWaslAsync(long warehouseId);

        Task<bool> IsInventoryNumberExists(long fleetId, string inventoryNumber);

        /// <summary>
        /// Update Wasl Info
        /// </summary>
        /// <param name="inventory"></param>
        /// <returns></returns>
        Task<bool> UpdateWaslInfoAsync(Inventory inventory);

        /// <summary>
        /// Get Warehouse Linked With Wasl Inventories
        /// </summary>
        /// <param name="warehouseId"></param>
        /// <returns></returns>
        Task<List<Inventory>> GetWarehouseLinkedWithWaslInventoriesAsync(long warehouseId);
        /// <summary>
        /// Get Inventories By WarehouseId
        /// </summary>
        /// <param name="warehouseId"></param>
        /// <returns></returns>
        Task<List<Inventory>> GetInventoriesByWarehouseId(long warehouseId);
        /// <summary>
        /// Add Inventory Sensors 
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        Task<bool> AddInventorySensorsAsync(List<InventorySensor> list);

        Task<bool> IsGatewayRegisteredWithInventoryAsync(long GatewayId);

        Task<List<Inventory>> FindInventoriesAsync(List<long> inventoryIds);

        Task<List<Inventory>> GetInventoryByFleetIdAsync(long fleetId);
        Task<List<Inventory>> FindInventoriesIncludingWarehousesAsync(List<long> inventoryIds);
    }
}
