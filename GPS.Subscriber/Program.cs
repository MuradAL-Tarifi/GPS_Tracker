using GPS.Redis;
using GPS.Subscriber.Integration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using StackExchange.Redis;
using System;
using System.IO;
using System.Threading.Tasks;

namespace GPS.Subscriber
{
    class Program
    {

        public async static Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder();
            BuildConfig(builder);

            var _configration = builder.Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(_configration)
                .Enrich.FromLogContext()
                .CreateLogger();

            //var _webEndpointBaseAddress = _configration["WebEndpoint:BaseUrl"];

            var host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton<IConnectionMultiplexer>(x =>
                    {
                        var options = ConfigurationOptions.Parse(_configration.GetValue<string>("RedisConnection:server"));
                        //options.Password = _configration.GetValue<string>("RedisConnection:password");
                        options.AbortOnConnectFail = false;

                        return ConnectionMultiplexer.Connect(options);
                    });
                    services.AddScoped<IWaslProxy, WaslProxy>();

                    services.AddSingleton<ICacheService,RedisCachService>();

                    services.AddHttpClient("WaslClient", c =>
                    {
                        //c.BaseAddress = new Uri("https://wasl.tga.gov.sa");
                        c.DefaultRequestHeaders.Add("Accept", "application/json");
                    });
                    services.AddHostedService<BackgroundServices.InventoryRedisSubscriper>();
                })
                .UseSerilog()
                .Build();

            await host.RunAsync();

            Console.Write("press any key to close the server.....");
            Console.Read();
        }

        static void BuildConfig(IConfigurationBuilder builder)
        {
            builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        }
    }
}
