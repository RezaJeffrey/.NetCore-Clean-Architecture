using Newtonsoft.Json;
using System.Net;
using System.Text.Json;
using Utils.Exceptions;

namespace WebFater.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        public GlobalExceptionMiddleware(RequestDelegate next) 
        {
            _next = next;
        }
        public async void Invoke(HttpContext context) 
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

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            var response = GenerateResponse(exception);
            context.Response.StatusCode = response.StatusCode;

            // TODO : LogError if it's not appRuleException
            return context.Response.WriteAsync(response.json);
        }

        private static (int StatusCode, string json) GenerateResponse(Exception exception)
            => exception switch
            {
                AppRuleException appException => (
                    appException.StatusCode,
                    JsonConvert.SerializeObject(new { errorMessage = appException.Message })
                ),
                _ => ( 
                        (int)HttpStatusCode.InternalServerError,
                        JsonConvert.SerializeObject( new { errorMessage = exception.Message })
                )
            };
        
    }
}
