using GPS.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GPS.API.Web.Middlewares
{
    /// <summary>
    /// Api Security Middleware
    /// </summary>
    public class ApiSecurityMiddleware
    {
        private readonly Microsoft.AspNetCore.Http.RequestDelegate _next;
        private readonly AppSettings _appSettings;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="next"></param>
        /// <param name="appSettings"></param>
        public ApiSecurityMiddleware(Microsoft.AspNetCore.Http.RequestDelegate next, AppSettings appSettings)
        {
            _next = next;
            _appSettings = appSettings;
        }


        /// <summary>
        /// Invoke
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task InvokeAsync(Microsoft.AspNetCore.Http.HttpContext context)
        {
            if (!_appSettings.ApiSecurity.Enable)
            {
                await _next(context);
                return;
            }

            var key = context.Request.Headers["api-key"];
            if (string.IsNullOrWhiteSpace(key) || !key.Equals(_appSettings.ApiSecurity.ApiKey))
            {
                context.Response.StatusCode = 401;
            }
            else
            {
                // Call the next delegate/middleware in the pipeline
                await _next(context);
            }

        }
    }
}
