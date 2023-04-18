using GPS.Server.Services;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Adapter;
using MQTTnet.Client;
using MQTTnet.Diagnostics;
using MQTTnet.Protocol;
using MQTTnet.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GPS.Server.TcpServers
{
   public class MQTT_Qc_Moko107_Server
    {

        private readonly IQc_Moko107Listener _qc_Moko107Listener;
        private readonly ILogger<MQTT_Qc_Moko107_Server> _logger;
        private readonly MqttClientOptions _mqttClientOptions;
   
        private readonly IMqttClient _iMqttClient;
        private readonly MqttClientSubscribeOptions _mqttClientSubscribeOptions;

        public MQTT_Qc_Moko107_Server(IServiceProvider serviceProvider, MqttClientOptions mqttClientOptions, IMqttClient iMqttClient, MqttClientSubscribeOptions mqttClientSubscribeOptions)
        {
            _qc_Moko107Listener = (IQc_Moko107Listener)serviceProvider.GetService(typeof(IQc_Moko107Listener));
            _logger = (ILogger<MQTT_Qc_Moko107_Server>)serviceProvider.GetService(typeof(ILogger<MQTT_Qc_Moko107_Server>));
            _mqttClientOptions = mqttClientOptions;
            _iMqttClient = iMqttClient;
            _mqttClientSubscribeOptions = mqttClientSubscribeOptions;
        }

        public async Task Start()
        {
            _iMqttClient.ApplicationMessageReceivedAsync += _qc_Moko107Listener.MqttMsgPublishReceived;
            await _iMqttClient.ConnectAsync(_mqttClientOptions, CancellationToken.None);
            await _iMqttClient.SubscribeAsync(_mqttClientSubscribeOptions, CancellationToken.None);
        }


        //public async Task Start()
        //{
        //    var mqttFactory = new MqttFactory();

        //    using (var mqttClient = mqttFactory.CreateMqttClient())
        //    {

        //        mqttClient.ApplicationMessageReceivedAsync += _qc_Moko107Listener.MqttMsgPublishReceived;

        //        await mqttClient.ConnectAsync(_mqttClientOptions, CancellationToken.None);


        //        var mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
        //        .WithTopicFilter(_mqqtTopic, MqttQualityOfServiceLevel.AtMostOnce)
        //        .Build();
        //        await mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);
        //        //Console.ReadLine();
        //    }
        //}

    }
}
