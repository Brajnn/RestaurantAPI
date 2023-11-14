using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace RestaurantAPI.Middleware
{
    public class ErrorHandlingMiddleware:IMiddleware
    {
        public readonly ILogger<ErrorHandlingMiddleware> _logger;

        
        public ErrorHandlingMiddleware(ILogger<ErrorHandlingMiddleware> logger)
        {
            _logger= logger;
        }
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next.Invoke(context);
            }
            catch(Exception e)
            {
                _logger.LogError(e, e.Message);

                context.Response.StatusCode = 500;
                context.Response.WriteAsync("Something went wrong");
            }
        }
    }
}
