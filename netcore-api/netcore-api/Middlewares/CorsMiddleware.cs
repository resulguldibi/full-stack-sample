using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace netcore_api.Middlewares
{
    public class CorsMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public CorsMiddleware(RequestDelegate next, ILoggerFactory logFactory)
        {
            _next = next;
            _logger = logFactory.CreateLogger("MyMiddleware");
        }

        public async Task Invoke(HttpContext httpContext)
        {
            await _next(httpContext);

            //httpContext.Response.Headers.Add("Access-Control-Allow-Origin", "http://localhost:8081");
        }
    }

    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseCorsMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CorsMiddleware>();
        }
    }
}
