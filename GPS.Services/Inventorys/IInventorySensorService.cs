using GPS.Domain.DTO;
using GPS.Domain.Views;
using System.Threading.Tasks;

namespace GPS.Services.Inventorys
{
    public interface IInventorySensorService
    {
        Task<ReturnResult<InventorySensorView>> FindBySensorSerialAsync(string Serial);
    }
}
