using GPS.Domain.DTO;
using GPS.Integration.WaslModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Integration.WaslServices.Inventories
{
    public interface IWaslInventoryService
    {
        Task<ReturnResult<WaslResult>> RegisterAsync(string companyId, string warehouseId, WaslInventoryModel model);

        Task<ReturnResult<WaslResult>> UpdateAsync(string inventoryId, WaslInventoryUpdateModel model);

        Task<ReturnResult<WaslResult>> DeleteAsync(string inventoryId);
    }
}
