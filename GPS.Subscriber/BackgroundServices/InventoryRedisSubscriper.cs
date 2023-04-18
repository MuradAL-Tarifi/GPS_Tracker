using GPS.Models;
using GPS.Redis.Enums;
using GPS.Subscriber.Integration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GPS.Subscriber.BackgroundServices
{
    public class InventoryRedisSubscriper : BackgroundService
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly IWaslProxy _waslProxy;
        private readonly ILogger<InventoryRedisSubscriper> _logger;

        public InventoryRedisSubscriper(IConnectionMultiplexer connectionMultiplexer,
            IWaslProxy waslProxy,
            ILogger<InventoryRedisSubscriper> logger)
        {
            _connectionMultiplexer = connectionMultiplexer;
            _waslProxy = waslProxy;
            _logger = logger;
        }


        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var sub = _connectionMultiplexer.GetSubscriber();

            return sub.SubscribeAsync($"ch_{PubSubChannel.WaslWarehouse}", (channel, value) => {
                var history = System.Text.Json.JsonSerializer.Deserialize<InventoryHistoryView>(value.ToString());
                _waslProxy.SFDAInventoryLocationHistoryServiceAsync(history);
            });
        }
    }
}
