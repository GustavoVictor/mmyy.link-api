using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net;

public static class ExceptionHandlerMiddleware
{
    public static void InvokeExceptionHandlerLocal(this IApplicationBuilder app)
    {
        app.Use(async (context, next) =>
        {
            try
            {
                await next();
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        });
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var error = ex switch 
        {
            ErrorException err => err.ErrorCode,
            _ => ErrorCode.InternalServerError
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = error.Status;

        return context.Response.WriteAsync(SerializeToJson(error));
    }

    private static string SerializeToJson(object value)
    {
        return JsonConvert.SerializeObject(
            value,
            new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
    }
}