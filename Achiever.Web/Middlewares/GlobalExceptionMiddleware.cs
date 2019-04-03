using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;


namespace Achiever.Web.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        public GlobalExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception e)
            {
                await HandleExceptionAsync(context, e);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception e)
        {
            var response = context.Response;
            
            response.StatusCode = 500;
            response.ContentType = "application/json; charset=utf-8";

            await response.WriteAsync(JsonConvert.SerializeObject(new ErrorResponse
            {
                Message = e.Message
            }, settings: _serializerSettings));
        }

        private class ErrorResponse
        {
            public String Message { get; set; }
        }
    }
}