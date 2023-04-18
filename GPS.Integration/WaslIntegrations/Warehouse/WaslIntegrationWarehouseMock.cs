using GPS.DataAccess.Repository.UnitOfWork;
using GPS.Domain.DTO;
using GPS.Integration.WaslModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Integration.WaslIntegrations.Warehouse
{
    public class WaslIntegrationWarehouseMock : WaslIntegrationBase, IWaslIntegrationWarehouse
    {
        public WaslIntegrationWarehouseMock(AppSettings appSettings, IUnitOfWork unitOfWork) : base(appSettings, unitOfWork)
        {
        }

        public async Task<WaslResponse> RegisterAsync(string companyId, WaslWarehouseModel model)
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

            await AddIntegrationLog(WaslIntegrationLogTypeEnum.Warehouse_Register, 200, new { companyId, model }, response);
            return response;
        }

        public async Task<WaslResponse> UpdateAsync(string warehouseId, WaslWarehouseUpdateModel model)
        {
            var response = await Task.FromResult(new WaslResponse()
            {
                Success = true,
                ResultCode = "success"
            });

            await AddIntegrationLog(WaslIntegrationLogTypeEnum.Warehouse_Update, 200, new { warehouseId, model }, response);
            return response;
        }

        public async Task<WaslResponse> DeleteAsync(string warehouseId)
        {
            var response = await Task.FromResult(new WaslResponse()
            {
                Success = true,
                ResultCode = "success"
            });

            await AddIntegrationLog(WaslIntegrationLogTypeEnum.Warehouse_Delete, 200, warehouseId, response);
            return response;
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
    }
}
