using AutoMapper;
using GPS.API.Proxy;
using GPS.DataAccess.Context;
using GPS.DataAccess.Repository.UnitOfWork;
using GPS.Domain.DTO;
using GPS.Services.Agents;
using GPS.Services.EventLogs;
using GPS.Services.Fleets;
using GPS.Services.Lookups;
using GPS.Services.Gateways;
using GPS.Services.Inventorys;
using GPS.Services.Users;
using GPS.Web.Admin.AppCode.Helpers;
using GPS.Web.Admin.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Refit;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using static GPS.DataAccess.AutoMapper.IAutoMapperConfig;
using GPS.Services.WareHouses;
using GPS.Services.Sensors;
using GPS.Services.Brands;
using GPS.Integration.WaslServices.OperatingCompanies;
using GPS.Integration.WaslIntegrations.OperatingCompanies;
using GPS.Integration.WaslIntegrations.Warehouse;
using GPS.Integration.WaslIntegrations.Inventories;
using GPS.Integration.WaslServices.Warehouse;
using GPS.Integration.WaslServices.Inventories;
using GPS.Services.SystemSettings;

namespace GPS.Web.Admin
{
    public class Startup
    {
        public IWebHostEnvironment _hostEnvironment { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment hostEnvironment)
        {
            Configuration = configuration;
            _hostEnvironment = hostEnvironment;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region appsettings config
            var appSettings = new AppSettings();
            Configuration.Bind("AppSettings", appSettings);
            services.AddSingleton(appSettings);
            #endregion

            #region cookie config
            var cookieOptions = new CookieOptions()
            {
                Path = "/",
                HttpOnly = false,
                IsEssential = true, //<- there
                Expires = DateTime.Now.AddMonths(1),
            };
            #endregion

            #region security antiforgery config
            services.AddAntiforgery(a => a.HeaderName = "XSRF-TOKEN");
            #endregion

            #region mvc config
            services.AddControllersWithViews()
                .AddViewLocalization(opts =>
                {
                    opts.ResourcesPath = "Resources";
                }).AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix).
                AddDataAnnotationsLocalization();
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

            services.Configure<RequestLocalizationOptions>(opts =>
            {
                var supportedCultures = new List<CultureInfo>
                {
                    new CultureInfo("ar-SA"),
                    new CultureInfo("en-US"),
                };

                opts.DefaultRequestCulture = new RequestCulture("ar-SA");
                opts.SupportedCultures = supportedCultures;
                opts.SupportedUICultures = supportedCultures;
                opts.RequestCultureProviders = new List<IRequestCultureProvider>
                {
                    new QueryStringRequestCultureProvider(),
                    new CookieRequestCultureProvider()
                };
                var cp = opts.RequestCultureProviders.OfType<CookieRequestCultureProvider>().First(); // Culture provider
                cp.CookieName = "GPS.Lang.Web.Admin.Cookie";
            });
            services.AddLocalization(opts => { opts.ResourcesPath = "Resources"; });
            #endregion

            #region authentication config
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.Name = "GPS.Web.Admin.Cookie";
                    options.LoginPath = "/Account/Login";
                    options.AccessDeniedPath = new PathString("/");
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;

                    options.ExpireTimeSpan = TimeSpan.FromHours(8);
                    options.SlidingExpiration = true;
                });
            #endregion

            #region http context accessor configs
            services.AddHttpContextAccessor();
            #endregion

            #region services config
            services.AddScoped<LoggedInUserProfile>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IViewHelper, ViewHelper>();
            services.AddScoped<ICultureHelper, CultureHelper>();
            services.AddScoped<IFleetService, FleetService>();
            services.AddScoped<IBrandService, BrandService>();
            services.AddScoped<ISensorService, SensorService>();
            services.AddScoped<IAgentService, AgentService>();
            services.AddScoped<ILookupsService, LookupsService>();
            services.AddScoped<IEventLogService, EventLogService>();
            services.AddScoped<IGatewayService, GatewayService>();
            services.AddScoped<IInventoryService, InventoryService>();
            services.AddScoped<IWarehouseService, WarehouseService>();
            services.AddScoped<IInventorySensorService, InventorySensorService>();
            services.AddScoped<ISystemSettingService, SystemSettingService>(); 
            #region Wasl
            services.AddScoped<IWaslOperatingCompaniesService, WaslOperatingCompaniesService>();
            services.AddScoped<IWaslWarehouseService, WaslWarehouseService>();
            services.AddScoped<IWaslInventoryService, WaslInventoryService>();
            if (_hostEnvironment.IsDevelopment())
            {
                services.AddScoped<IWaslIntegrationOperatingCompanies, WaslIntegrationOperatingCompaniesMock>();
                services.AddScoped<IWaslIntegrationWarehouse, WaslIntegrationWarehouseMock>();
                services.AddScoped<IWaslIntegrationInventory, WaslIntegrationInventoryMock>();
            }
            else
            {
                services.AddScoped<IWaslIntegrationOperatingCompanies, WaslOperatingCompanies>();
                services.AddScoped<IWaslIntegrationWarehouse, WaslIntegrationWarehouse>();
                services.AddScoped<IWaslIntegrationInventory, WaslIntegrationInventory>();
            }
            #endregion
            services.AddRefitClient<IUserProxy>()
                     .ConfigureHttpClient(c =>
                     {
                         c.BaseAddress = new Uri(appSettings.WebApi.BaseUrl);
                         c.DefaultRequestHeaders.TryAddWithoutValidation("api-key", appSettings.WebApi.ApiKey);
                         c.Timeout = TimeSpan.FromSeconds(appSettings.WebApi.TimeoutSeconds);
                     });
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            //Request Localization
            var options = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(options.Value);

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
