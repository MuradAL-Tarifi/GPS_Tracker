using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Domain.DTO
{
    public class AppSettings
    {
        public ConnectionStrings ConnectionStrings { get; set; }

        public ApiSecurity ApiSecurity { get; set; }

        public WaslIntegration WaslIntegration { get; set; }

        public CustomAlerts CustomAlerts { get; set; }

        public Email Email { get; set; }

        public SMS SMS { get; set; }

        public HistoryApi HistoryApi { get; set; }

        public WebApi WebApi { get; set; }

        public string WebAgentUrl { get; set; }

        //public string RedisConfiguration { get; set; }
    }

    public class ConnectionStrings
    {
        public string DefaultConnection { get; set; }
        public string HistoryConnection { get; set; }
    }

    public class ApiSecurity
    {
        public bool Enable { get; set; }
        public string ApiKey { get; set; }
    }

    public class WaslIntegration
    {
        public string BaseUrl { get; set; }
        public string ApiKey { get; set; }
    }

    public class HistoryApi
    {
        public string BaseUrl { get; set; }
        public string ApiKey { get; set; }
        public int TimeoutSeconds { get; set; }
    }

    public class WebApi
    {
        public string BaseUrl { get; set; }
        public string ApiKey { get; set; }
        public int TimeoutSeconds { get; set; }
    }

    public class CustomAlerts
    {
        public bool IsEnabled { get; set; }
        public int MinIntervalMinutes { get; set; }
    }

    public class Email
    {
        public bool IsEnabled { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public bool EnableSsl { get; set; }
        public string Address { get; set; }
        public string DisplayName { get; set; }
        public string Password { get; set; }
    }

    public class SMS
    {
        public bool IsEnabled { get; set; }
        public string ServiceUrl { get; set; }
    }
}
