using GPS.Domain.DTO;
using GPS.Domain.Models;
using GPS.Domain.Views;
using System.Threading.Tasks;

namespace GPS.DataAccess.Repository.Gateways
{
    public interface IGatewayRepository
    {
        Task<PagedResult<Gateway>> SearchAsync(string SearchString, int PageNumber, int pageSize);

        Task<Gateway> FindByIdAsync(long? Id);

        Task<bool> AddAsync(Gateway gateway);

        Task<bool> UpdateAsync(GatewayView model);

        Task<Gateway> DeleteAsync(long Id, string UpdatedBy);

        Task<bool> IsNameExistsAsync(string Name);

        Task<bool> IsIMEIExistsAsync(string IMEI);
    }
}
