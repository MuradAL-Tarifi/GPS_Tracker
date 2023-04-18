using GPS.Domain.DTO;
using GPS.Domain.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Services.Gateways
{
    public interface IGatewayService
    {
        Task<ReturnResult<PagedResult<GatewayView>>> SearchAsync(string SearchString = "", int PageNumber = 1, int pageSize = 100);
        Task<ReturnResult<GatewayView>> FindByIdAsync(long? Id);
        Task<ReturnResult<bool>> AddAsync(GatewayView gateway);
        Task<ReturnResult<bool>> UpdateAsync(GatewayView gateway);
        Task<ReturnResult<bool>> DeleteAsync(long Id, string UpdatedBy);
        Task<bool> IsNameExistsAsync(string Name);
        Task<bool> IsIMEIExistsAsync(string IMEI);
        Task<bool> IsGatewayLinkedToInventoryAsync(long IMEI);
    }
}
