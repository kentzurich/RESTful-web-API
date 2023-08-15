using Microsoft.AspNetCore.Diagnostics;
using Newtonsoft.Json;

namespace MagicVilla_VillaAPI.Extensions
{
    public static class CustomExceptionExtensions
    {
        public static void ErrorHandler(this IApplicationBuilder app, bool isDevelopment)
        {
            app.UseExceptionHandler(error =>
            {
                error.Run(async context =>
                {
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "application/json";
                    var feature = context.Features.Get<IExceptionHandlerFeature>();

                    if (feature != null)
                    {
                        if (isDevelopment)
                        {
                            if(feature.Error is BadImageFormatException badImageFormatException)
                            {
                                await context.Response.WriteAsync(JsonConvert.SerializeObject(new
                                {
                                    StatusCode = 776,
                                    ErrorMessage = "Hello from Custom Handler invoked in Program.cs! Image format is invalid."
                                }));
                            }
                            else
                            {
                                await context.Response.WriteAsync(JsonConvert.SerializeObject(new
                                {
                                    StatusCode = context.Response,
                                    ErrorMessage = feature.Error.Message,
                                    StackTrace = feature.Error.StackTrace
                                }));
                            }
                        }
                        else
                        {
                            await context.Response.WriteAsync(JsonConvert.SerializeObject(new
                            {
                                StatusCode = context.Response.StatusCode,
                                ErrorMessage = "Hello from Program.cs excecption handler"
                            }));
                        }
                    }

                });
            });
        }
    }
}
