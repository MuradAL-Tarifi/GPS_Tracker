using AutoMapper;
using GPS.API.Server.Services;
using GPS.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;
using GPS.Redis;
using GPS.API.Server.Middleware;

namespace GPS.API.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var appSettings = new AppSettings();
            Configuration.Bind("AppSettings", appSettings);
            services.AddSingleton(appSettings);

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new AutoMapperConfig());
            });

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

            services.AddSingleton<IConnectionMultiplexer>(x => {

                var options = ConfigurationOptions.Parse(Configuration.GetValue<string>("RedisConnection:server"));
                //options.Password = Configuration.GetValue<string>("RedisConnection:password");
                //options.AbortOnConnectFail = false;

                return ConnectionMultiplexer.Connect(options);
            });
            services.AddSingleton<ICacheService, RedisCachService>();
            services.AddScoped<IDapperRepository, DapperRepository>();
            services.AddScoped<IInventoryHistoryService, InventoryHistoryService>();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "GPS.API.Server", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "GPS.API.Server v1"));
            }
            else
            {
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/GPSServer/swagger/v1/swagger.json", "GPS.API.Server v1"));
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
