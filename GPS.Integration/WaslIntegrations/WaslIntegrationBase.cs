using GPS.DataAccess.Repository.UnitOfWork;
using GPS.Domain.DTO;
using GPS.Domain.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Integration.WaslIntegrations
{
    public class WaslIntegrationBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly AppSettings _appSettings;
        public WaslIntegrationBase(AppSettings appSettings, IUnitOfWork unitOfWork)
        {
            _appSettings = appSettings;
            _unitOfWork = unitOfWork;
        }

        public HttpClient GetClient()
        {
            string baseUrl = _appSettings.WaslIntegration.BaseUrl;
            string apiKey = _appSettings.WaslIntegration.ApiKey;

            var client = new HttpClient
            {
                BaseAddress = new Uri(baseUrl),
                Timeout = TimeSpan.FromSeconds(120)
            };

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("x-api-key", apiKey);
            client.DefaultRequestHeaders.Add("x-api-key", apiKey);
            //client.DefaultRequestHeaders.Add("Content-Type", "application/json");

            return client;
        }

        public static StringContent ParseJsonStringContent(object model)
        {
            var jsonData = JsonConvert.SerializeObject(model, new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                },
                Formatting = Formatting.Indented
            });

            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            return content;
        }

        public async Task AddIntegrationLog(WaslIntegrationLogTypeEnum typeEnum, int httpCode, object request, object response)
        {
            string responseString;
            if (response is string)
            {
                responseString = response.ToString();
            }
            else
            {
                responseString = JsonConvert.SerializeObject(response);
            }

            try
            {
                var log = new WaslIntegrationLog()
                {
                    WaslLogTypeLookupId = typeEnum,
                    HttpCode = httpCode,
                    Request = JsonConvert.SerializeObject(request, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }),
                    Response = responseString,
                };

                await _unitOfWork.WaslIntegrationLogRepository.AddAsync(log);
            }
            catch (Exception ex)
            {
            }
        }
    }
}
