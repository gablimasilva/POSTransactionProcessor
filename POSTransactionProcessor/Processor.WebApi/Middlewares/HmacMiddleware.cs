using System.Security.Cryptography;
using System.Text;

public class HmacMiddleware {
    private readonly RequestDelegate _next;
    private const string SECRET = "super-secret";

    public HmacMiddleware(RequestDelegate next) {
        _next = next;
    }

    public async Task Invoke(HttpContext context) {
        var signature = context.Request.Headers["X-Signature"];
        var timestamp = context.Request.Headers["X-Timestamp"];

        if (string.IsNullOrEmpty(signature) || string.IsNullOrEmpty(timestamp)) {
            context.Response.StatusCode = 401;
            return;
        }

        context.Request.EnableBuffering();

        var body = await new StreamReader(context.Request.Body).ReadToEndAsync();
        context.Request.Body.Position = 0;

        var payload = timestamp + body;

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(SECRET));
        var computed = Convert.ToHexString(hmac.ComputeHash(Encoding.UTF8.GetBytes(payload)));

        if (!computed.Equals(signature, StringComparison.OrdinalIgnoreCase)) {
            context.Response.StatusCode = 401;
            return;
        }

        await _next(context);
    }
}
