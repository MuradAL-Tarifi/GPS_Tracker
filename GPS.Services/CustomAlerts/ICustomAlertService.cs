using GPS.Domain.DTO;
using GPS.Domain.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Services.CustomAlerts
{
    public interface ICustomAlertService
    {
        Task<ReturnResult<PagedResult<CustomAlertView>>> SearchAsync(long FleetId,long? WarehouseId = null, long? InventoryId = null, int? IsActive = null, string SearchString = "", int PageNumber = 1, int PageSize = 100);
        Task<ReturnResult<bool>> AddAsync(CustomAlertView CustomAlertView);

        Task<ReturnResult<bool>> UpdateAsync(CustomAlertView CustomAlertView);

        Task<ReturnResult<bool>> DeleteAsync(long Id, string UpdatedBy);

        Task<ReturnResult<CustomAlertView>> CustomAlertByIdAsync(long Id);
    }
}
