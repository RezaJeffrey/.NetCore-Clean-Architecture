using Newtonsoft.Json;
using System.Net;
using System.Text.Json;
using Utils.Exceptions;

namespace Web.Middlewares
{
    public class GlobalDevExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        public GlobalDevExceptionMiddleware(RequestDelegate next) 
        {
            _next = next;
        }
        public async Task Invoke(HttpContext context) 
        {
            try
            {
                await _next(context);
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

            // TODO : LogError if it's not BusinessException
            await context.Response.WriteAsync(response.json);
        }

        private static (int StatusCode, string json) GenerateResponse(Exception exception)
            => exception switch
            {
                BusinessException appException => (
                    appException.StatusCode,
                    JsonConvert.SerializeObject(new { 
                        errorMessage = appException.Message,
                        stackTrace = appException.StackTrace,
                        InnerException = appException.InnerException,
                        source = appException.Source 
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
