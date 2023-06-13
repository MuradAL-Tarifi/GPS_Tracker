using GPS.Domain.DTO;
using GPS.Domain.Models;
using GPS.Domain.ViewModels;
using GPS.Domain.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Services.WareHouses
{
    public interface IWarehouseService
    {
        Task<ReturnResult<PagedResult<WarehouseView>>> SearchAsync(long? FleetId = null, int? waslLinkStatus = null, int? isActive = null, string SearchString = "", int PageNumber = 1, int pageSize = 100);
        Task<ReturnResult<WarehouseView>> FindByIdAsync(long? Id);
        Task<ReturnResult<WarehouseView>> SaveAsync(WarehouseView warehouse);
        Task<ReturnResult<bool>> DeleteAsync(long Id, string UpdatedBy);
        Task<ReturnResult<List<WarehouseView>>> GetByUserIdAsync(string userId);
        Task<ReturnResult<bool>> LinkWithWasl(long id, string updatedBy);
        Task<ReturnResult<bool>> UnlinkWithWasl(long id, string updatedBy);
        Task<ReturnResult<List<WarehouseView>>> GetFleetLinkedWithWaslWarehousesAsync(long fleetId);
        Task<ReturnResult<WarehouDetailsViewModel>> FindDetailedWarehouseByIdAsync(long WarehouseId);
        Task<ReturnResult<List<WarehouseView>>> GetAll();
    }
}
