using AutoMapper.Features;
using Microsoft.AspNetCore.Diagnostics;
using Newtonsoft.Json;

namespace MagicVilla_VillaAPI.Middleware
{
    public class CustomExceptionMiddleware
    {
        private readonly RequestDelegate _requestDelegate;
        public CustomExceptionMiddleware(RequestDelegate requestDelegate)
        {
            _requestDelegate = requestDelegate;
        }
        public async Task InvokeAsync(HttpContext httpContext)
        {
            // this will call everytime even if there is no exception thrown.
            try
            {
                await _requestDelegate(httpContext);
            }
            catch (Exception ex)
            {
                await ProcessException(httpContext, ex);
            }
        }

        private async Task ProcessException(HttpContext httpContext, Exception ex)
        {
            httpContext.Response.StatusCode = 500;
            httpContext.Response.ContentType = "application/json";
            if (ex is BadImageFormatException badImageFormatException)
            {
                await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(new
                {
                    StatusCode = 776,
                    ErrorMessage = "Hello from Custom Middleware Handler invoked in Program.cs! Image format is invalid."
                }));
            }
            else
            {
                await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(new
                {
                    StatusCode = httpContext.Response,
                    ErrorMessage = "Hello from middleware! - Finale"
                }));
            }
        }
    }
}
