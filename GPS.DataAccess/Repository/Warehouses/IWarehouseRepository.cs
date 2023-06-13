using GPS.Domain.DTO;
using GPS.Domain.Models;
using GPS.Domain.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.DataAccess.Repository.Warehouses
{
    public interface IWarehouseRepository
    {
        Task<PagedResult<Warehouse>> SearchAsync(long? FleetId, int? waslLinkStatus, int? IsActive, string SearchString, int PageNumber, int pageSize);

        Task<Warehouse> FindByIdAsync(long? Id);

        Task<Warehouse> AddAsync(Warehouse warehouse);

        Task<Warehouse> UpdateAsync(WarehouseView model);

        Task<Warehouse> DeleteAsync(long Id, string UpdatedBy);

        Task<List<Warehouse>> GetByUserIdAsync(string userId);

        Task<bool> UpdateLinkedWithWaslInfoAsync(Warehouse warehouse);

        /// <summary>
        /// Update Wasl Info
        /// </summary>
        /// <param name="warehouse"></param>
        /// <returns></returns>
        Task<bool> UpdateWaslInfoAsync(Warehouse warehouse);

        /// <summary>
        /// Get Fleet Linked With Wasl Warehouses
        /// </summary>
        /// <param name="fleetId"></param>
        /// <returns></returns>
        Task<List<Warehouse>> GetFleetLinkedWithWaslWarehousesAsync(long fleetId);
        Task<List<Warehouse>> FindWarehousesAsync(List<long> warehouseIds);

        Task<List<Warehouse>> GetByFleetIdAsync(long fleetId);
        Task<List<Warehouse>> GetAllAsync();
    }
}
