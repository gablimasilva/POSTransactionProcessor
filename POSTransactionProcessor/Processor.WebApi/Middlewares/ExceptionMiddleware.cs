using Processor.Domain.Exceptions;

public class ExceptionMiddleware {
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next) {
        _next = next;
    }


    public async Task Invoke(HttpContext context) {
        try {
            await _next(context);
        }
        catch (NotFoundException ex) {
            context.Response.StatusCode = 404;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsJsonAsync(new {
                message = ex.Message,
                code = "NOT_FOUND"
            });
        }
        catch (ExternalAuthorizationException ex) {
            context.Response.StatusCode = 422;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsJsonAsync(new {
                message = ex.Message,
                code = "BUSINESS_ERROR"
            });
        }
        catch (ExternalServiceUnavailableException ex) {
            context.Response.StatusCode = 503;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsJsonAsync(new {
                message = ex.Message,
                code = "EXTERNAL_SERVICE_UNAVAILABLE"
            });
        }
        catch (Exception ex) {
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsJsonAsync(new {
                message = "Internal server error",
                code = "INTERNAL_ERROR"
            });
        }
    }

}