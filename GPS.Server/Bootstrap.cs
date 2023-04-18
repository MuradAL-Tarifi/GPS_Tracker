using GPS.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GPS.Server
{
    public interface IBootstrap
    {
        Task Run();
    }
    public class Bootstrap : IBootstrap
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _config;
        private readonly int RD07Gateway_Port;
        private readonly int RD07WifiGateway_Port;
        private readonly int WF501_Port;
        private readonly int QcMoko107_Port;
        IServiceProvider _serviceProvider;
        //servers
        private readonly TcpServers.RD07GatewayServer _rd07GatewayServer;
        private readonly TcpServers.RD07WifiGatewayServer _rd07WifiGatewayServer;
        private readonly TcpServers.WF501Server _wF501Server;
        private readonly TcpServers.MQTT_Qc_Moko107_Server _mqtt_Qc_Moko107_Server;
        // Tasks
        private Task tRd07;
        private Task tRd07Wifi;
        private Task tWF501;
        private Task tQcMoko107;

        public Bootstrap(ILogger<Bootstrap> logger,
            IConfiguration config,
            IServiceProvider serviceProvider
            )
        {
            _logger = logger;
            _config = config;
            _serviceProvider = serviceProvider;

            RD07Gateway_Port = int.Parse(_config.GetValue<string>("TcpServer:RD07Gateway:Port"));
            RD07WifiGateway_Port = int.Parse(_config.GetValue<string>("TcpServer:RD07WifiGateway:Port"));
            WF501_Port = int.Parse(_config.GetValue<string>("TcpServer:WF501:Port"));
            QcMoko107_Port = int.Parse(_config.GetValue<string>("TcpServer:QcMoko107:Port"));

            if (RD07Gateway_Port > 0)
                _rd07GatewayServer = new TcpServers.RD07GatewayServer(IPAddress.Any, RD07Gateway_Port, _serviceProvider);

            if (WF501_Port > 0)
                _wF501Server = new TcpServers.WF501Server(IPAddress.Any, WF501_Port, _serviceProvider);

            if (RD07WifiGateway_Port > 0)
                _rd07WifiGatewayServer = new TcpServers.RD07WifiGatewayServer(IPAddress.Any, RD07WifiGateway_Port, _serviceProvider);

            if (QcMoko107_Port > 0)
            {
                var mqttClientOptions = new MqttClientOptionsBuilder()
                    .WithTcpServer(_config.GetValue<string>("TcpServer:QcMoko107:IPAddress"), QcMoko107_Port)
                    .WithClientId(_config.GetValue<string>("TcpServer:QcMoko107:ClientId"))
                    .WithCredentials(_config.GetValue<string>("TcpServer:QcMoko107:Username"), _config.GetValue<string>("TcpServer:QcMoko107:Password"))
                    .Build();
                var mqttFactory = new MqttFactory();
                var mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
                        .WithTopicFilter(_config.GetValue<string>("TcpServer:QcMoko107:Topic"), MqttQualityOfServiceLevel.AtMostOnce)
                        .Build();
                _mqtt_Qc_Moko107_Server = new TcpServers.MQTT_Qc_Moko107_Server(_serviceProvider, mqttClientOptions, mqttFactory.CreateMqttClient(), mqttSubscribeOptions);
            }
        }

        public async Task Run()
        {
            tRd07 = Task.Factory.StartNew(() => _rd07GatewayServer.Start(), TaskCreationOptions.LongRunning);

            tWF501 = Task.Factory.StartNew(() => _wF501Server.Start(), TaskCreationOptions.LongRunning);

            tRd07Wifi = Task.Factory.StartNew(() => _rd07WifiGatewayServer.Start(), TaskCreationOptions.LongRunning);

            tQcMoko107 = Task.Factory.StartNew(() =>  _mqtt_Qc_Moko107_Server.Start(), TaskCreationOptions.LongRunning);
           

            if (RD07Gateway_Port > 0)
                _logger.LogInformation($"RD07 gateway Server Started on port {RD07Gateway_Port} ....");

            if (WF501_Port > 0)
                _logger.LogInformation($"WF501 Server Started on port {WF501_Port} ....");

            if (RD07WifiGateway_Port > 0)
                _logger.LogInformation($"RD07 Wifi gateway Server Started on port {RD07WifiGateway_Port} ....");

            if (QcMoko107_Port > 0)
                _logger.LogInformation($"Qc-Moko107 Server Started on port {QcMoko107_Port} ....");

            _logger.LogInformation($"V15");
            await Task.FromResult(0);
        }

       
    }
}
