using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Processor.Domain.Interfaces;
using Processor.Infrastructure.Cache;
using Processor.Infrastructure.Chaos;
using StackExchange.Redis;

public static class DependencyInjection {
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration config) {
        services.AddSingleton<IConnectionMultiplexer>(sp => ConnectionMultiplexer.Connect(config["Redis:Connection"] ?? "localhost:6379"));
        services.AddSingleton<IIdempotencyCache, RedisIdempotencyCache>();
        services.AddDbContext<ProcessorDbContext>(options => options.UseNpgsql(config.GetConnectionString("Postgres")));
        services.Configure<ChaosOptions>(config.GetSection("Chaos"));
        services.AddSingleton(sp => sp.GetRequiredService<IOptions<ChaosOptions>>().Value);
        services.AddScoped<ITransactionRepository, TransactionRepository>();

        return services;
    }
}