using GPS.DataAccess.Context;
using GPS.Domain.Models;
using GPS.Redis.Enums;
using GPS.Services.Job;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GPS.API.Web
{
    /// <summary>
    /// 
    /// </summary>
    public class CustomAlertRedisSubscriber : BackgroundService
    {
        /// <summary>
        /// 
        /// </summary>
        public IServiceScopeFactory _serviceScopeFactory;
        /// <summary>
        /// 
        /// </summary>
        private readonly IConnectionMultiplexer _connectionMultiplexer;
       /// <summary>
       /// 
       /// </summary>
       /// <param name="serviceScopeFactory"></param>
       /// <param name="connectionMultiplexer"></param>
        public CustomAlertRedisSubscriber(IServiceScopeFactory serviceScopeFactory, IConnectionMultiplexer connectionMultiplexer)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _connectionMultiplexer = connectionMultiplexer;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var sub = _connectionMultiplexer.GetSubscriber();
            await sub.SubscribeAsync($"ch_{PubSubChannel.CustomAlert}", async (channel, value) =>
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scoped = scope.ServiceProvider.GetRequiredService<IJobService>();
                    var watcher = System.Text.Json.JsonSerializer.Deserialize<CustomAlertWatcher>(value.ToString());
                    await scoped.BackgroundCustomAlertsJobAsync(watcher);
                }
            });
        }
    }
}
