using Microsoft.IO;

public class HttpLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _loggerRequests;
    private readonly ILogger _loggerLoggedUserRequests;
    private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;

    public HttpLoggingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
    {
        _next = next;
        _loggerRequests = loggerFactory.CreateLogger("all_requests");
        _loggerLoggedUserRequests = loggerFactory.CreateLogger("logged_user_requests");
        _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
    }

    public async Task Invoke(HttpContext context)
    {
        var originalBodyStream = context.Response.Body;

        await using var responseBody = _recyclableMemoryStreamManager.GetStream();
        context.Response.Body = responseBody;

        try
        {

            await _next(context);
        }
        finally
        {
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var body = await new StreamReader(context.Response.Body).ReadToEndAsync();
            context.Response.Body.Seek(0, SeekOrigin.Begin);

            var hasUserAuthenticated = context.User?.Identities?.FirstOrDefault()?.IsAuthenticated ?? false;

            var message = "METHOD:{method}\tPATH:{host}:{port}{url}\tSTATUSCODE:{statusCode}\tBODYRESPONSE:{body}";
            
            if (hasUserAuthenticated)
            {
                message = "USER: {user}\t" + message;

                _loggerLoggedUserRequests.LogInformation(
                    message,
                    context.User.Claims.ElementAt(0).Value,
                    context.Request?.Method,
                    context.Request.Host.Host,
                    context.Request.Host.Port,
                    context.Request?.Path.Value,
                    context.Response?.StatusCode,
                    body
                );
            } else
            {
                _loggerRequests.LogInformation(
                    message,
                    context.Request?.Method,
                    context.Request.Host.Host,
                    context.Request.Host.Port,
                    context.Request?.Path.Value,
                    context.Response?.StatusCode,
                    body
                );
            }

            await responseBody.CopyToAsync(originalBodyStream);
        }
    }
}