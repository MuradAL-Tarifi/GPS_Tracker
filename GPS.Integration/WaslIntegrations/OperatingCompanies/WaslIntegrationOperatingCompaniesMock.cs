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

namespace GPS.Integration.WaslIntegrations.OperatingCompanies
{
    public class WaslIntegrationOperatingCompaniesMock : WaslIntegrationBase, IWaslIntegrationOperatingCompanies
    {
        public WaslIntegrationOperatingCompaniesMock(AppSettings appSettings, IUnitOfWork unitOfWork) : base(appSettings, unitOfWork)
        {
        }

        public async Task<WaslResponse> RegisterCompanyAsync(WaslRegisterCompanyModel model)
        {
            var response = await Task.FromResult(new WaslResponse()
            {
                Success = true,
                Result = new WaslResult()
                {
                    ReferenceKey = Guid.NewGuid().ToString(),
                    IsDuplicate = false
                }
            });

            await AddIntegrationLog(WaslIntegrationLogTypeEnum.OperatingCompany_RegisterCompany, 200, model, response);
            return response;
        }

        public async Task<WaslResponse> RegisterIndividualAsync(WaslIndividualModel model)
        {
            var response = await Task.FromResult(new WaslResponse()
            {
                Success = true,
                Result = new WaslResult()
                {
                    ReferenceKey = Guid.NewGuid().ToString(),
                    IsDuplicate = false
                }
            });

            await AddIntegrationLog(WaslIntegrationLogTypeEnum.OperatingCompany_RegisterIndividual, 200, model, response);
            return response;
        }

        public async Task<WaslCompany> GetCompanyAsync(string IdentityNumber, string CommercialRecordNumber, string activity)
        {
            //Sample URL
            //https://wasl.elm.sa/api/tracking/v1/operating-companies?identityNumber=7000000000&commercialRecordNumber=1000000000

            using (HttpClient client = GetClient())
            {
                var url = $"{client.BaseAddress}/operating-companies?identityNumber={IdentityNumber}&commercialRecordNumber={CommercialRecordNumber}";
                if (!string.IsNullOrEmpty(activity))
                {
                    url += $"&activity={activity}";
                }

                var response = await client.GetAsync(url);
                if (response.Content == null)
                {
                    return null;
                }

                var responseString = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<WaslCompany>(responseString);

                await AddIntegrationLog(WaslIntegrationLogTypeEnum.OperatingCompany_Get, 200, new { IdentityNumber, CommercialRecordNumber, activity }, response);
                return data;
            }
        }

        public async Task<WaslCompany> GetIndividualAsync(string IdentityNumber, string activity)
        {
            //Sample URL
            //https://wasl.elm.sa/api/tracking/v1/operating-companies?identityNumber=7000000000

            using (HttpClient client = GetClient())
            {
                var url = $"{client.BaseAddress}/operating-companies?identityNumber={IdentityNumber}";
                if (!string.IsNullOrEmpty(activity))
                {
                    url += $"&activity={activity}";
                }

                var response = await client.GetAsync(url);
                if (response.Content == null)
                {
                    return null;
                }

                var responseString = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<WaslCompany>(responseString);

                await AddIntegrationLog(WaslIntegrationLogTypeEnum.OperatingCompany_Get, 200, IdentityNumber, response);
                return data;
            }
        }

        public async Task<WaslInquiryModel> CompanyInquiryAsync(string IdentityNumber, string CommercialRecordNumber, string activity)
        {
            //Sample URL
            //https://wasl.elm.sa/api/tracking/v1/operating-companies?identityNumber=7000000000&commercialRecordNumber=1000000000

            using (HttpClient client = GetClient())
            {
                var url = $"{client.BaseAddress}/operating-companies?identityNumber={IdentityNumber}&commercialRecordNumber={CommercialRecordNumber}";
                if (!string.IsNullOrEmpty(activity))
                {
                    url += $"&activity={activity}";
                }

                var response = await client.GetAsync(url);
                if (response.Content == null)
                {
                    return null;
                }

                var responseString = await response.Content.ReadAsStringAsync();
                await AddIntegrationLog(WaslIntegrationLogTypeEnum.OperatingCompany_Get, 200, new { IdentityNumber, CommercialRecordNumber, activity }, response);

                return new WaslInquiryModel()
                {
                    StatusCode = response.StatusCode,
                    Request = url,
                    Response = responseString
                };
            }
        }

        public async Task<WaslInquiryModel> IndividualInquiryAsync(string IdentityNumber, string activity)
        {
            //Sample URL
            //https://wasl.elm.sa/api/tracking/v1/operating-companies?identityNumber=7000000000

            using (HttpClient client = GetClient())
            {
                var url = $"{client.BaseAddress}/operating-companies?identityNumber={IdentityNumber}";
                if (!string.IsNullOrEmpty(activity))
                {
                    url += $"&activity={activity}";
                }

                var response = await client.GetAsync(url);
                if (response.Content == null)
                {
                    return null;
                }

                var responseString = await response.Content.ReadAsStringAsync();
                await AddIntegrationLog(WaslIntegrationLogTypeEnum.OperatingCompany_Get, 200, new { IdentityNumber, activity }, response);

                return new WaslInquiryModel()
                {
                    StatusCode = response.StatusCode,
                    Request = url,
                    Response = responseString
                };
            }
        }

        public async Task<WaslResponse> UpdateContactInfoAsync(WaslUpdateCompanyModel model)
        {
            var response = await Task.FromResult(new WaslResponse()
            {
                Success = true
            });

            await AddIntegrationLog(WaslIntegrationLogTypeEnum.OperatingCompany_UpdateContactInfo, 200, model, response);
            return response;
        }

        public async Task<WaslResponse> DeleteAsync(string referenceNumber, string activity)
        {
            var response = await Task.FromResult(new WaslResponse()
            {
                Success = true
            });

            await AddIntegrationLog(WaslIntegrationLogTypeEnum.OperatingCompany_Delete, 200, referenceNumber, response);
            return response;
        }
    }
}
