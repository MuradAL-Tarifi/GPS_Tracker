using GPS.Integration.WaslModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Integration.WaslIntegrations.Inventories
{
    public interface IWaslIntegrationInventory
    {
        Task<WaslResponse> RegisterAsync(string companyId, string warehouseId, WaslInventoryModel model);

        Task<WaslResponse> UpdateAsync(string inventoryId, WaslInventoryUpdateModel model);

        Task<WaslResponse> DeleteAsync(string inventoryId);
    }
}
