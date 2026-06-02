using Microsoft.AspNetCore.Builder;
using Serilog;
using Serilog.Sinks.Graylog;

namespace Processor.Infrastructure.Logging;

public static class GraylogExtensions {
    public static void AddGraylogLogging(this WebApplicationBuilder builder) {
        var graylogHost = builder.Configuration["Graylog:Host"];
        var graylogPort = int.Parse(builder.Configuration["Graylog:Port"] ?? "12201");

        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.Graylog(new GraylogSinkOptions {
                HostnameOrAddress = graylogHost,
                Port = graylogPort
            })
            .CreateLogger();

        builder.Host.UseSerilog();
    }
}