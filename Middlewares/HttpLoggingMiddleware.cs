using Microsoft.IO;

public class HttpLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _loggerRequests;
    private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;

    public HttpLoggingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
    {
        _next = next;
        _loggerRequests = loggerFactory.CreateLogger("log_requests");
        _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
    }

    public async Task Invoke(HttpContext context)
    {
        context.Request.EnableBuffering();

        await using var requestStream = _recyclableMemoryStreamManager.GetStream();
        await context.Request.Body.CopyToAsync(requestStream);

        var hasUserAuthenticated = context.User?.Identities?.FirstOrDefault()?.IsAuthenticated ?? false;

        var reqBody = ReadStreamInChunks(requestStream);

        _loggerRequests.LogInformation($"REQUEST - " 
                                        + (hasUserAuthenticated ? $"USER: {context.User.Claims.ElementAt(0).Value} " : "")
                                        + $"SCHEMA:{context.Request.Scheme} "
                                        + $"METHOD:{context.Request?.Method} "
                                        + $"PATH:{context.Request.Host.Host}:{context.Request.Host.Port}{context.Request?.Path.Value} "
                                        + "QUERY_STRING:" + (!context.Request.QueryString.HasValue ? "EMPTY" : context.Request.QueryString) + " "
                                        + "BODY:" + (string.IsNullOrEmpty(reqBody) ? "EMPTY" : reqBody));
                            
        context.Request.Body.Position = 0;

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
            var respBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            
            _loggerRequests.LogInformation($"RESPONSE - " 
                                            + (hasUserAuthenticated ? $"AUTH_USER: {context.User.Claims.ElementAt(0).Value} " : "")
                                            + $"BODY:{respBody}");

            await responseBody.CopyToAsync(originalBodyStream);
        }
    }

    private static string ReadStreamInChunks(Stream stream)
    {
        const int readChunkBufferLength = 4096;
        
        stream.Seek(0, SeekOrigin.Begin);
        using var textWriter = new StringWriter();
        using var reader = new StreamReader(stream);
        var readChunk = new char[readChunkBufferLength];
        int readChunkLength;

        do
        {
            readChunkLength = reader.ReadBlock(readChunk, 0, readChunkBufferLength);
            textWriter.Write(readChunk, 0, readChunkLength);
        } while (readChunkLength > 0);
        
        return textWriter.ToString();
    }
}