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

namespace GPS.Integration.WaslIntegrations.OperatingCompanies
{
    public class WaslOperatingCompanies : WaslIntegrationBase, IWaslIntegrationOperatingCompanies
    {
        public WaslOperatingCompanies(AppSettings appSettings, IUnitOfWork unitOfWork) : base(appSettings, unitOfWork)
        {
        }

        public async Task<WaslResponse> RegisterCompanyAsync(WaslRegisterCompanyModel model)
        {
            using (HttpClient client = GetClient())
            {
                var url = $"{client.BaseAddress}/operating-companies";

                var jsonObject = new Dictionary<string, object>
                {
                    { "identityNumber", model.IdentityNumber },
                    { "commercialRecordNumber", model.CommercialRecordNumber },
                    { "commercialRecordIssueDateHijri", model.CommercialRecordIssueDateHijri },
                    { "phoneNumber", model.PhoneNumber },
                    { "extensionNumber", model.ExtensionNumber },
                    { "emailAddress", model.EmailAddress },
                    { "managerName", model.ManagerName },
                    { "managerPhoneNumber", model.ManagerPhoneNumber },
                    { "managerMobileNumber", model.ManagerMobileNumber }
                };

                if (!string.IsNullOrEmpty(model.Activity))
                {
                    jsonObject.Add("activity", model.Activity);
                }

                if (!string.IsNullOrEmpty(model.SFDACompanyActivity))
                {
                    jsonObject.Add("sfdaCompanyActivity", model.SFDACompanyActivity);
                }

                var content = ParseJsonStringContent(jsonObject);
                var response = await client.PostAsync(url, content);
                if (response.Content == null)
                {
                    return null;
                }

                var responseString = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<WaslResponse>(responseString);
                //JsonConvert.SerializeObject(client.DefaultRequestHeaders.Authorization)
                await AddIntegrationLog(WaslIntegrationLogTypeEnum.OperatingCompany_RegisterCompany, (int)response.StatusCode, model, responseString);
                return data;
            }
        }

        public async Task<WaslResponse> RegisterIndividualAsync(WaslIndividualModel model)
        {
            using (HttpClient client = GetClient())
            {
                var url = $"{client.BaseAddress}/operating-companies";

                var jsonObject = new Dictionary<string, object>
                {
                    { "identityNumber", model.IdentityNumber },
                    { "dateOfBirthHijri", model.DateOfBirthHijri },
                    { "phoneNumber", model.PhoneNumber },
                    { "extensionNumber", model.ExtensionNumber },
                    { "emailAddress", model.EmailAddress }
                };

                if (!string.IsNullOrEmpty(model.Activity))
                {
                    jsonObject.Add("activity", model.Activity);
                }

                if (!string.IsNullOrEmpty(model.SFDACompanyActivity))
                {
                    jsonObject.Add("sfdaCompanyActivity", model.SFDACompanyActivity);
                }

                var content = ParseJsonStringContent(jsonObject);
                var response = await client.PostAsync(url, content);
                if (response.Content == null)
                {
                    return null;
                }

                var responseString = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<WaslResponse>(responseString);

                await AddIntegrationLog(WaslIntegrationLogTypeEnum.OperatingCompany_RegisterIndividual, (int)response.StatusCode, model, responseString);
                return data;
            }
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

                await AddIntegrationLog(WaslIntegrationLogTypeEnum.OperatingCompany_Get, (int)response.StatusCode, new { IdentityNumber, CommercialRecordNumber, activity }, responseString);
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

                await AddIntegrationLog(WaslIntegrationLogTypeEnum.OperatingCompany_Get, (int)response.StatusCode, new { IdentityNumber, activity }, responseString);
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
                await AddIntegrationLog(WaslIntegrationLogTypeEnum.OperatingCompany_Get, 200, new { IdentityNumber, CommercialRecordNumber, activity }, responseString);

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
            using (HttpClient client = GetClient())
            {
                var url = $"{client.BaseAddress}/operating-companies/contact-info";

                var jsonData = JsonConvert.SerializeObject(model, new JsonSerializerSettings
                {
                    ContractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new CamelCaseNamingStrategy()
                    },
                    Formatting = Formatting.Indented
                });
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                var request = new HttpRequestMessage(new HttpMethod("PATCH"), url) { Content = content };
                var response = await client.SendAsync(request);
                if (response.Content == null)
                {
                    return null;
                }

                var responseString = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<WaslResponse>(responseString);

                await AddIntegrationLog(WaslIntegrationLogTypeEnum.OperatingCompany_UpdateContactInfo, (int)response.StatusCode, model, responseString);
                return data;
            }
        }

        public async Task<WaslResponse> DeleteAsync(string referenceNumber, string activity)
        {
            using (HttpClient client = GetClient())
            {
                var url = $"{client.BaseAddress}/operating-companies/{referenceNumber}";
                if (!string.IsNullOrEmpty(activity))
                {
                    url += $"?activity={activity}";
                }

                var response = await client.DeleteAsync(url);
                if (response.Content == null)
                {
                    return null;
                }

                var responseString = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<WaslResponse>(responseString);

                await AddIntegrationLog(WaslIntegrationLogTypeEnum.OperatingCompany_Delete, (int)response.StatusCode, referenceNumber, responseString);
                return data;
            }
        }
    }
}
