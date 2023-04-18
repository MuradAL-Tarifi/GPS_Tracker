using GPS.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Subscriber.Integration
{
    public interface IWaslProxy
    {

        /// <summary>
        /// ارسال بيانات المستودعات لمنصة وصل - تتبع المستودعات
        /// </summary>
        /// <param name="inventoryHistory"></param>
        /// <returns></returns>
        Task SFDAInventoryLocationHistoryServiceAsync(InventoryHistoryView inventoryHistory);

    }

    public class WaslProxy : IWaslProxy
    {
        //private readonly string WaseBaselUrl = "https://wasl.tga.gov.sa/api/tracking/v1";
        //private readonly string baseUrl = "https://wasl.tga.gov.sa/";
       
        //private readonly string WaselAPIKey = "";
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<WaslProxy> _logger;
        private readonly IConfiguration _configuration;

        public WaslProxy(IHttpClientFactory httpClientFactory, ILogger<WaslProxy> logger, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _configuration = configuration;
        }
        public async Task SFDAInventoryLocationHistoryServiceAsync(InventoryHistoryView history)
        {
            try
            {
                string WaselAPIKey = _configuration["Wasl:APIKey"];
                HttpClient client = _httpClientFactory.CreateClient("WaslClient");
                //client.BaseAddress = new Uri(WaseBaselUrl);

                client.Timeout = TimeSpan.FromSeconds(10);

                var url = $"https://wasl.tga.gov.sa/api/tracking/v1/inventories/{history.InventoryReferanceKey}/stats";


                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("x-api-key", WaselAPIKey);
                client.DefaultRequestHeaders.Add("x-api-key", WaselAPIKey);


                var contractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver
                {
                    NamingStrategy = new Newtonsoft.Json.Serialization.CamelCaseNamingStrategy()
                };

                string json = JsonConvert.SerializeObject(history, new JsonSerializerSettings
                {
                    ContractResolver = contractResolver,
                    Formatting = Formatting.None
                });

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                using (HttpResponseMessage response = await client.PostAsync(url, content))
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {

                        _logger.LogInformation($"SFDA Inventory sensor {history.Serial} Success :  {responseString}");
                    }
                    else
                    {
                        _logger.LogError($"SFDA Inventory sensor {history.Serial} Error : {JsonConvert.SerializeObject(history)}");
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                // TODO : log error
            }
        }

    }
}
