using AutoMapper;
using DinkToPdf;
using DinkToPdf.Contracts;
using DNTCaptcha.Core;
using GPS.API.Proxy;
using GPS.DataAccess;
using GPS.DataAccess.AutoMapper;
using GPS.DataAccess.Context;
using GPS.DataAccess.Repository.UnitOfWork;
using GPS.Domain.DTO;
using GPS.Job.Models;
using GPS.Services;
using GPS.Services.Users;
using Hangfire;
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
using Refit;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using static GPS.DataAccess.AutoMapper.IAutoMapperConfig;

namespace GPS.Job
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var appSettings = new AppSettings();
            Configuration.Bind("AppSettings", appSettings);
            services.AddSingleton(appSettings);

            var cookieOptions = new CookieOptions()
            {
                Path = "/",
                HttpOnly = false,
                IsEssential = true, //<- there
                Expires = DateTime.Now.AddMonths(1),
            };

            services.AddAntiforgery(a => a.HeaderName = "XSRF-TOKEN");

            services.Configure<RequestLocalizationOptions>(opts =>
            {
                var supportedCultures = new List<CultureInfo>
                {
                    new CultureInfo("en-US"),
                    new CultureInfo("ar-SA")
                };

                opts.DefaultRequestCulture = new RequestCulture("en-US");
                opts.SupportedCultures = supportedCultures;
                opts.SupportedUICultures = supportedCultures;

                var cp = opts.RequestCultureProviders.OfType<CookieRequestCultureProvider>().First(); // Culture provider
                cp.CookieName = "Accu.Web.Admin.Lang.Cookie";
            });

            services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

            // Add DBContext services.
            services.AddDbContext<TrackerDBContext>(options => options
            .UseSqlServer(appSettings.ConnectionStrings.DefaultConnection,
            opt =>
            {
                opt.EnableRetryOnFailure();
            }));

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new AutoMapperConfig());
            });

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

            RegisterServices(services, appSettings);

            services.AddLocalization(opts => { opts.ResourcesPath = "Resources"; });

            services.AddHttpContextAccessor();

            services.AddScoped<LoggedInUserProfile>();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.Name = "GPS.Job.Cookie";
                    options.LoginPath = "/Account/Login";
                    options.AccessDeniedPath = new PathString("/");
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;

                    options.ExpireTimeSpan = TimeSpan.FromHours(8);
                    options.SlidingExpiration = true;
                });

            services.AddSingleton(new ConfigModel
            {
                ScheduledReportsCron = Configuration["Cron:ScheduledReportsCron"],
                InventoryCustomAlertsWatcherCron = Configuration["Cron:InventoryCustomAlertsWatcherCron"]
            });

            services.AddHostedService<RecurringJobService>();

            services.AddHangfire(x => x.UseSqlServerStorage(appSettings.ConnectionStrings.DefaultConnection));

            services.AddHangfireServer(options =>
            {
                options.WorkerCount = 1;
            });

            services.AddControllersWithViews()
               .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseAuthentication();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                //app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
               //Authorization = new[] { new HangFireAuthFilter() },
            });



            app.UseHangfireServer();
        }

        private void RegisterServices(IServiceCollection services, AppSettings appSettings)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUserService, UserService>();

            services.AddRefitClient<IJobApiProxy>()
                  .ConfigureHttpClient(c =>
                  {
                      c.BaseAddress = new Uri(appSettings.WebApi.BaseUrl);
                      c.DefaultRequestHeaders.TryAddWithoutValidation("api-key", appSettings.WebApi.ApiKey);
                      c.Timeout = TimeSpan.FromSeconds(appSettings.WebApi.TimeoutSeconds);
                  });
        }
    }
}
