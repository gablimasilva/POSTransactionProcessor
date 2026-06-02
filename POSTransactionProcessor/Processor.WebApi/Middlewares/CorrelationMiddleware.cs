public class CorrelationMiddleware {
    private readonly RequestDelegate _next;

    public CorrelationMiddleware(RequestDelegate next) {
        _next = next;
    }

    public async Task Invoke(HttpContext context) {
        var correlationId =
            context.Request.Headers["X-Correlation-Id"].FirstOrDefault()
            ?? Guid.NewGuid().ToString();

        context.Response.Headers["X-Correlation-Id"] = correlationId;

        await _next(context);
    }
}