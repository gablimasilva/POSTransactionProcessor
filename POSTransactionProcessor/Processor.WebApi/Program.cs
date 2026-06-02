using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Processor.Domain.Interfaces;
using Processor.Infrastructure.Cache;
using Processor.Infrastructure.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource =>
        resource.AddService("Processor.Api"))
    .WithTracing(tracing => {
        tracing
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddConsoleExporter();
    })
    .WithMetrics(metrics => {
        metrics
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddRuntimeInstrumentation()
            .AddMeter("Processor.Resilience")
            .AddConsoleExporter();
    });
builder.Services.AddControllers();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddScoped<AuthorizeTransactionUseCase>();
builder.Services.AddScoped<ConfirmTransactionUseCase>();
builder.Services.AddScoped<UndoTransactionUseCase>();

builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();

builder.Services.AddSingleton<IExternalProcessor, ExternalProcessorMock>();

builder.Services.AddSingleton<IIdempotencyCache, RedisIdempotencyCache>();


// health
builder.Services.AddHealthChecks();

builder.AddGraylogLogging();

var app = builder.Build();

app.MapHealthChecks("/health");

app.UseMiddleware<CorrelationMiddleware>();
app.UseMiddleware<HmacMiddleware>();
app.UseMiddleware<LoggingMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment()) {
    using (var scope = app.Services.CreateScope()) {
        var db = scope.ServiceProvider.GetRequiredService<ProcessorDbContext>();
        db.Database.Migrate();
    }
}

app.MapControllers();

app.Run();
