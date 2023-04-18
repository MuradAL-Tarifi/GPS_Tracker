using AutoMapper;
using DinkToPdf;
using DinkToPdf.Contracts;
using GPS.API.Proxy;
using GPS.API.Web.Middlewares;
using GPS.DataAccess.Context;
using GPS.DataAccess.Repository.UnitOfWork;
using GPS.Domain.DTO;
using GPS.Integration.EmailIntegrations;
using GPS.Redis;
using GPS.Services.Alerts;
using GPS.Services.EmailHistorys;
using GPS.Services.EventLogs;
using GPS.Services.Inventorys;
using GPS.Services.Job;
using GPS.Services.Lookups;
using GPS.Services.OnlineInventoryHistorys;
using GPS.Services.Users;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Refit;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using static GPS.DataAccess.AutoMapper.IAutoMapperConfig;

namespace GPS.API.Web
{    /// <summary>
     /// Startup
     /// </summary>
    public class Startup
    {
        /// <summary>
        /// Configuration
        /// </summary>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        /// <summary>
        /// Configuration
        /// </summary>
        public IConfiguration Configuration { get; }


        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            #region appsettings config
            var appSettings = new AppSettings();
            Configuration.Bind("AppSettings", appSettings);
            services.AddSingleton(appSettings);
            #endregion

            #region mvc config
            services.AddControllers();
            services.AddControllersWithViews();
            #endregion

            #region swagger config
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "GPS.API.Web", Version = "v1" });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
            #endregion

            #region pdf config
            services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
            #endregion

            #region sql config
            // Add DBContext services.
            services.AddDbContext<TrackerDBContext>(options => options
            .UseSqlServer(appSettings.ConnectionStrings.DefaultConnection,
            opt =>
            {
                opt.EnableRetryOnFailure();
            }));
            #endregion

            #region mapper config
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new AutoMapperConfig());
            });

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);
            #endregion

            #region localization config
            services.AddLocalization(opts => { opts.ResourcesPath = "Resources"; });
            #endregion

            #region services config
            services.AddSingleton<IConnectionMultiplexer>(x =>
            {
                var options = ConfigurationOptions.Parse("127.0.01:6379");
                options.AbortOnConnectFail = false;
                //options.Password = _configration.GetValue<string>("RedisConnection:password");

                return ConnectionMultiplexer.Connect(options);
            });
            services.AddSingleton<ICacheService, RedisCachService>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IInventoryHistoryService, InventoryHistoryService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ILookupsService, LookupsService>();
            services.AddScoped<IInventorySensorService, InventorySensorService>();
            services.AddScoped<IInventoryHistoryReportService, InventoryHistoryReportService>();
            services.AddScoped<IOnlineInventoryHistoryService, OnlineInventoryHistoryService>();

            services.AddScoped<IEventLogService, EventLogService>();
            services.AddScoped<IJobService, JobService>();
            services.AddScoped<IEmailHistoryService, EmailHistoryService>();
            services.AddScoped<IAlertService, AlertService>();
            services.AddScoped<IEmailIntegration, EmailIntegration>();  
            #endregion

            services.AddRefitClient<IInventoryHistoryApiProxy>()
             .ConfigureHttpClient(c =>
             {
                 c.BaseAddress = new Uri(appSettings.HistoryApi.BaseUrl);
                 c.DefaultRequestHeaders.TryAddWithoutValidation("api-key", appSettings.HistoryApi.ApiKey);
                 c.Timeout = TimeSpan.FromSeconds(appSettings.HistoryApi.TimeoutSeconds);
             });
            services.AddHostedService<CustomAlertRedisSubscriber>();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "GPS.API.Web v1"));
            }
            else
            {
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/GPSTrackerWebApi/swagger/v1/swagger.json", "GPS.API.Web v1"));
            }
            app.UseMiddleware<ApiSecurityMiddleware>();
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
