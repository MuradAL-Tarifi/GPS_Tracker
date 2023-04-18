using GPS.Proxy;
using GPS.Proxy.Abstractions;
using GPS.Redis;
using GPS.Server.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Refit;
using Serilog;
using StackExchange.Redis;
using System;
using System.IO;
using System.Threading.Tasks;

namespace GPS.Server
{
    class Program
    {
        private static Bootstrap _bootstrap;
        public async static Task Main(string[] args)
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.DarkBlue;
                Console.WriteLine("Server Socket Started Listening..........");

                #region config
                var builder = new ConfigurationBuilder();
                builder.SetBasePath(Directory.GetCurrentDirectory())
                     .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                var _configration = builder.Build();
                #endregion

                #region Logger
                Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(_configration)
                .Enrich.FromLogContext()
                .CreateLogger();
                #endregion

                var _inventoryEndpointBaseAddress = _configration["InventoryEndpoint:BaseUrl"];
                var _inventoryEndpointApiKey = _configration["InventoryEndpoint:ApiKey"];


                var host = Host.CreateDefaultBuilder()
                    .ConfigureServices((context, services) => {

                        services.AddSingleton<IConnectionMultiplexer>(x => {
                            var options = ConfigurationOptions.Parse(_configration.GetValue<string>("RedisConnection:server"));
                        //options.Password = _configration.GetValue<string>("RedisConnection:password");
                        return ConnectionMultiplexer.Connect(options);
                        });
                        services.AddScoped<IRD07WifigatewayListener, RD07WifigatewayListener>();
                        services.AddScoped<IRD07gatewayListener, RD07gatewayListener>();
                        services.AddScoped<IWF501Listener, WF501Listener>();
                        services.AddSingleton<ICacheService, RedisCachService>();
                        services.AddScoped<IInventoryProxyAccessor, InventoryProxyAccessor>();
                        services.AddScoped<IBootstrap, Bootstrap>();
                        services.AddScoped<IQc_Moko107Listener, Qc_Moko107Listener>(); 
                        services.AddRefitClient<IInventoryProxy>()
                                .ConfigureHttpClient(c =>
                                {
                                    c.BaseAddress = new Uri(_inventoryEndpointBaseAddress);
                                    c.DefaultRequestHeaders.TryAddWithoutValidation("api-key", _inventoryEndpointApiKey);
                                });

                    })
                    .UseSerilog()
                    .Build();


                _bootstrap = ActivatorUtilities.CreateInstance<Bootstrap>(host.Services);
                await _bootstrap.Run();
                while (true)
                {
                    Console.ReadLine();
                }

            }
            catch(Exception ex)
            {

            }
        }
    }
}
