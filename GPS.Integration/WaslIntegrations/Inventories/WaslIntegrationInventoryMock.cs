using GPS.DataAccess.Repository.UnitOfWork;
using GPS.Domain.DTO;
using GPS.Integration.WaslModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Integration.WaslIntegrations.Inventories
{
    public class WaslIntegrationInventoryMock : WaslIntegrationBase, IWaslIntegrationInventory
    {
        public WaslIntegrationInventoryMock(AppSettings appSettings, IUnitOfWork unitOfWork) : base(appSettings, unitOfWork)
        {
        }

        public async Task<WaslResponse> RegisterAsync(string companyId, string warehouseId, WaslInventoryModel model)
        {
            var response = await Task.FromResult(new WaslResponse()
            {
                Success = true,
                Result = new WaslResult()
                {
                    ReferenceKey = Guid.NewGuid().ToString(),
                },
                ResultCode = "success"
            });

            await AddIntegrationLog(WaslIntegrationLogTypeEnum.Inventory_Register, 200, new { companyId, warehouseId, model }, response);
            return response;
        }

        public async Task<WaslResponse> UpdateAsync(string inventoryId, WaslInventoryUpdateModel model)
        {
            var response = await Task.FromResult(new WaslResponse()
            {
                Success = true,
                ResultCode = "success"
            });

            await AddIntegrationLog(WaslIntegrationLogTypeEnum.Inventory_Update, 200, new { inventoryId, model }, response);
            return response;
        }

        public async Task<WaslResponse> DeleteAsync(string inventoryId)
        {
            var response = await Task.FromResult(new WaslResponse()
            {
                Success = true,
                ResultCode = "success"
            });

            await AddIntegrationLog(WaslIntegrationLogTypeEnum.Inventory_Delete, 200, inventoryId, response);
            return response;
        }
    }
}
