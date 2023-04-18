using GPS.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GPS.API.Server.Middleware
{
    public class ApiSecurityMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly AppSettings _appSettings;

        public ApiSecurityMiddleware(RequestDelegate next, AppSettings appSettings)
        {
            _next = next;
            _appSettings = appSettings;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!_appSettings.Security.Enable)
            {
                await _next(context);
                return;
            }

            var key = context.Request.Headers["api-key"];

            if (string.IsNullOrWhiteSpace(key) || !key.Equals(_appSettings.Security.ApiKey))
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
