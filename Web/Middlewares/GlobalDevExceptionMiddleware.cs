using Newtonsoft.Json;
using System.Net;
using System.Text.Json;
using Utils.Exceptions;

namespace Web.Middlewares
{
    public class GlobalDevExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalDevExceptionMiddleware> _logger;
        public GlobalDevExceptionMiddleware(RequestDelegate next, ILogger<GlobalDevExceptionMiddleware> logger) 
        {
            _next = next;
            _logger = logger;
        }
        public async Task Invoke(HttpContext context) 
        {
            try
            {
                _logger.LogInformation("Request Start Point");  

                    await _next(context);

                _logger.LogInformation("Request End Point");

                if (context.Response.StatusCode == 401 && !context.Response.HasStarted)
                    throw new UnauthorizedAccessException("خطا در احراز هویت, لطفا ابتدا وارد شوید");
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            var response = GenerateResponse(exception);
            context.Response.StatusCode = response.StatusCode;

            if (exception is ServiceException appException)
            {
                _logger.LogError(exception, "Error Items: {@Items}", appException.Items);

            }
            else
            {
                _logger.LogError(exception, "Error happned method: {@Path}", context.Request.Path);
            }
            await context.Response.WriteAsync(response.json);
        }

        private static (int StatusCode, string json) GenerateResponse(Exception exception)
            => exception switch
            {
                ServiceException appException => (
                    appException.StatusCode,
                    JsonConvert.SerializeObject(new { 
                        errorMessage = appException.Message,
                        stackTrace = appException.StackTrace,
                        InnerException = appException.InnerException,
                        source = appException.Source,
                        Items = appException.Items
                    })
                ),
                UnauthorizedAccessException unauthorizedAccessException => (
                    (int)HttpStatusCode.Unauthorized,
                        JsonConvert.SerializeObject(new
                        {
                            errorMessage = unauthorizedAccessException.Message,
                            stackTrace = unauthorizedAccessException.StackTrace,
                            source = unauthorizedAccessException.Source
                        })
                ),
                _ => ( 
                        (int)HttpStatusCode.InternalServerError,
                        JsonConvert.SerializeObject(new
                        {
                            errorMessage = exception.Message,
                            stackTrace = exception.StackTrace,
                            InnerException = exception.InnerException,
                            source = exception.Source
                        })
                )
            };
        
    }
}
