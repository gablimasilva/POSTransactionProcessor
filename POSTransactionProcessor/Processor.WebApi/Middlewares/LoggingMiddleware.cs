using System.Text;
using Serilog;

public class LoggingMiddleware {
    private readonly RequestDelegate _next;

    public LoggingMiddleware(RequestDelegate next) {
        _next = next;
    }

    public async Task Invoke(HttpContext context) {
        context.Request.EnableBuffering();

        var requestBody = await new StreamReader(context.Request.Body)
            .ReadToEndAsync();

        context.Request.Body.Position = 0;

        Log.Information(
            "HTTP {Method} {Path} CorrelationId={CorrelationId} Request={Request}",
            context.Request.Method,
            context.Request.Path,
            context.TraceIdentifier,
            requestBody
        );

        var originalBody = context.Response.Body;

        using var newBody = new MemoryStream();
        context.Response.Body = newBody;

        await _next(context);

        newBody.Seek(0, SeekOrigin.Begin);

        var responseBody = await new StreamReader(newBody).ReadToEndAsync();

        Log.Information("HTTP Response {Status} Body: {Body}",
            context.Response.StatusCode,
            responseBody);

        newBody.Seek(0, SeekOrigin.Begin);
        await newBody.CopyToAsync(originalBody);
    }
}
