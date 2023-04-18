using AutoMapper;
using DinkToPdf;
using DinkToPdf.Contracts;
using GPS.API.History.Middlewares;
using GPS.DataAccess.Context;
using GPS.DataAccess.Repository.UnitOfWork;
using GPS.Domain.DTO;
using GPS.Services.EventLogs;
using GPS.Services.Inventorys;
using GPS.Services.Lookups;
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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using static GPS.DataAccess.AutoMapper.IAutoMapperConfig;

namespace GPS.API.History
{
    /// <summary>
    /// Startup
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Constructor 
        /// </summary>
        /// <param name="configuration"></param>
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

            var appSettings = new AppSettings();
            Configuration.Bind("AppSettings", appSettings);
            services.AddSingleton(appSettings);

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "GPS.API.History", Version = "v1" });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

            #region sql config
            // Add DBContext services.
            services.AddDbContext<TrackerDBContext>(options => options
            .UseSqlServer(appSettings.ConnectionStrings.DefaultConnection,
            opt =>
            {
                opt.EnableRetryOnFailure();
            }));

            services.AddDbContext<TrackerHistoryContext>(options => options
           .UseSqlServer(appSettings.ConnectionStrings.HistoryConnection,
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

            #region services config
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ILookupsService, LookupsService>();
            services.AddScoped<IEventLogService, EventLogService>();
            services.AddScoped<IInventoryHistoryService, InventoryHistoryService>();
            #endregion

            services.AddLocalization(opts => { opts.ResourcesPath = "Resources"; });
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
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "GPS.API.History v1"));
            }
            else
            {
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/GPSTrackerHistoyApi/swagger/v1/swagger.json", "GPS.API.History v1"));
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
