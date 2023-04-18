using GPS.DataAccess.Repository.UnitOfWork;
using GPS.Domain.DTO;
using GPS.Integration.WaslModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Integration.WaslIntegrations.Inventories
{
    public class WaslIntegrationInventory : WaslIntegrationBase, IWaslIntegrationInventory
    {
        public WaslIntegrationInventory(AppSettings appSettings, IUnitOfWork unitOfWork) : base(appSettings, unitOfWork)
        {
        }

        public async Task<WaslResponse> RegisterAsync(string companyId, string warehouseId, WaslInventoryModel model)
        {
            using (HttpClient client = GetClient())
            {
                var url = $"{client.BaseAddress}/operating-companies/{companyId}/warehouses/{warehouseId}/inventories";

                var jsonData = JsonConvert.SerializeObject(model, new JsonSerializerSettings
                {
                    ContractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new CamelCaseNamingStrategy()
                    },
                    Formatting = Formatting.Indented
                });
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(url, content);
                if (response.Content == null)
                {
                    return null;
                }

                var responseString = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<WaslResponse>(responseString);

                await AddIntegrationLog(WaslIntegrationLogTypeEnum.Inventory_Register, (int)response.StatusCode, new { companyId, warehouseId, model }, responseString);
                return data;
            }
        }

        public async Task<WaslResponse> UpdateAsync(string inventoryId, WaslInventoryUpdateModel model)
        {
            using (HttpClient client = GetClient())
            {
                var url = $"{client.BaseAddress}/inventories/{inventoryId}";

                var jsonData = JsonConvert.SerializeObject(model, new JsonSerializerSettings
                {
                    ContractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new CamelCaseNamingStrategy()
                    },
                    Formatting = Formatting.Indented
                });
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                var response = await client.PatchAsync(url, content);
                if (response.Content == null)
                {
                    return null;
                }

                var responseString = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<WaslResponse>(responseString);

                await AddIntegrationLog(WaslIntegrationLogTypeEnum.Inventory_Update, (int)response.StatusCode, new { inventoryId, model }, responseString);
                return data;
            }
        }

        public async Task<WaslResponse> DeleteAsync(string inventoryId)
        {
            using (HttpClient client = GetClient())
            {
                var url = $"{client.BaseAddress}/inventories/{inventoryId}";

                var response = await client.DeleteAsync(url);
                if (response.Content == null)
                {
                    return null;
                }

                var responseString = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<WaslResponse>(responseString);

                await AddIntegrationLog(WaslIntegrationLogTypeEnum.Inventory_Delete, (int)response.StatusCode, inventoryId, responseString);
                return data;
            }
        }
    }
}
