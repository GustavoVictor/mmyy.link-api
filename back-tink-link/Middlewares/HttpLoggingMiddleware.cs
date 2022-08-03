public class HttpLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;

    public HttpLoggingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
    {
        _next = next;
        _logger = loggerFactory.CreateLogger<HttpLoggingMiddleware>();
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        finally
        {
            _logger.LogInformation(
            "{host} {port} Request {method} {url} => {statusCode}",
            context.Request.Host.Host,
            context.Request.Host.Port,
            context.Request?.Method,
            context.Request?.Path.Value,
            context.Response?.StatusCode);
        }
    }
}