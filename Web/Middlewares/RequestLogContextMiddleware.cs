using Serilog.Context;
using Utils.Services;

namespace Web.Middlewares
{
    /// <summary>
    ///     Includes Properties in Logs
    /// </summary>
    public class RequestLogContextMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestLogContextMiddleware(RequestDelegate next)
        {
            _next = next;   
        }

        public Task InvokeAsync(HttpContext context, IServiceProvider serviceProvider)
        {
            var _authUtilService = serviceProvider.GetRequiredService<AuthUtilService>();

            using (LogContext.PushProperty("CorrelcationId", context.TraceIdentifier))
            using (LogContext.PushProperty("UserName", _authUtilService.GetUserName() ?? "Guest"))
            {
                return _next(context);
            }

        }
    }
}
