using Newtonsoft.Json;
using System.Net;
using System.Text.Json;
using Utils.Exceptions;

namespace Web.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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

            // TODO : LogError if it's not ServiceException
            await context.Response.WriteAsync(response.json);
        }

        private static (int StatusCode, string json) GenerateResponse(Exception exception)
            => exception switch
            {
                ServiceException appException => (
                    appException.StatusCode,
                    JsonConvert.SerializeObject(new { errorMessage = appException.Message })
                ),
                UnauthorizedAccessException unauthorizedAccessException => (
                    (int)HttpStatusCode.Unauthorized,
                        JsonConvert.SerializeObject(new
                        {
                            errorMessage = unauthorizedAccessException.Message,
                        })
                ),
                _ => (
                        (int)HttpStatusCode.InternalServerError,
                        JsonConvert.SerializeObject(new { errorMessage = exception.Message })
                )
            };

    }
}
