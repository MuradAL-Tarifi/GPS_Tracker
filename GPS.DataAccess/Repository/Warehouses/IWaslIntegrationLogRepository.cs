
using GPS.Domain.Models;
using System.Threading.Tasks;

namespace GPS.DataAccess.Repository.Warehouses
{
    public interface IWaslIntegrationLogRepository
    {
        Task AddAsync(WaslIntegrationLog waslIntegrationLog);
    }
}
