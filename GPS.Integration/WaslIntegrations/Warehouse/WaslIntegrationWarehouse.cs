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

namespace GPS.Integration.WaslIntegrations.Warehouse
{
    public class WaslIntegrationWarehouse : WaslIntegrationBase, IWaslIntegrationWarehouse
    {
        public WaslIntegrationWarehouse(AppSettings appSettings, IUnitOfWork unitOfWork) : base(appSettings, unitOfWork)
        {
        }

        public async Task<WaslResponse> RegisterAsync(string companyId, WaslWarehouseModel model)
        {
            using (HttpClient client = GetClient())
            {
                var url = $"{client.BaseAddress}/operating-companies/{companyId}/warehouses";

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

                await AddIntegrationLog(WaslIntegrationLogTypeEnum.Warehouse_Register, (int)response.StatusCode, new { companyId, model }, responseString);
                return data;
            }
        }

        public async Task<WaslResponse> UpdateAsync(string warehouseId, WaslWarehouseUpdateModel model)
        {
            using (HttpClient client = GetClient())
            {
                var url = $"{client.BaseAddress}/warehouses/{warehouseId}";

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

                await AddIntegrationLog(WaslIntegrationLogTypeEnum.Warehouse_Update, (int)response.StatusCode, new { warehouseId, model }, responseString);
                return data;
            }
        }

        public async Task<List<WaslWarehouse>> GetAsync(string companyId)
        {
            //Sample URL
            //https://wasl.tga.gov.sa/api/tracking/v1/operatingcompanies{OPERATING_COMPANY_ID}/warehouses/inquiry?activity=SFDA

            using (HttpClient client = GetClient())
            {
                var url = $"{client.BaseAddress}/operating-companies/{companyId}/warehouses/inquiry?activity=SFDA";

                var response = await client.GetAsync(url);
                if (response.Content == null)
                {
                    return null;
                }

                var responseString = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<List<WaslWarehouse>>(responseString);

                await AddIntegrationLog(WaslIntegrationLogTypeEnum.Warehouse_Get, (int)response.StatusCode, new { companyId }, responseString);
                return data;
            }
        }

        public async Task<WaslInquiryModel> InquiryAsync(string companyId)
        {
            //Sample URL
            //https://wasl.tga.gov.sa/api/tracking/v1/operatingcompanies{OPERATING_COMPANY_ID}/warehouses/inquiry?activity=SFDA

            using (HttpClient client = GetClient())
            {
                var url = $"{client.BaseAddress}/operating-companies/{companyId}/warehouses/inquiry?activity=SFDA";

                var response = await client.GetAsync(url);
                if (response.Content == null)
                {
                    return null;
                }

                var responseString = await response.Content.ReadAsStringAsync();

                await AddIntegrationLog(WaslIntegrationLogTypeEnum.Warehouse_Get, (int)response.StatusCode, new { companyId }, responseString);
                return new WaslInquiryModel()
                {
                    StatusCode = response.StatusCode,
                    Request = url,
                    Response = responseString
                };
            }
        }

        public async Task<WaslResponse> DeleteAsync(string warehouseId)
        {
            using (HttpClient client = GetClient())
            {
                var url = $"{client.BaseAddress}/warehouses/{warehouseId}";

                var response = await client.DeleteAsync(url);
                if (response.Content == null)
                {
                    return null;
                }

                var responseString = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<WaslResponse>(responseString);

                await AddIntegrationLog(WaslIntegrationLogTypeEnum.Warehouse_Delete, (int)response.StatusCode, warehouseId, responseString);
                return data;
            }
        }
    }
}
