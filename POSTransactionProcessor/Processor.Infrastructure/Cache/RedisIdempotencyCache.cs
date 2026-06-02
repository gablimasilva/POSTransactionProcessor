using StackExchange.Redis;

namespace Processor.Infrastructure.Cache;

public class RedisIdempotencyCache : IIdempotencyCache {
    private readonly IDatabase _db;

    public RedisIdempotencyCache(IConnectionMultiplexer redis) {
        _db = redis.GetDatabase();
    }

    public async Task<string?> GetTransactionId(string terminalId, string nsu) {
        var key = $"tx:{terminalId}:{nsu}";
        return await _db.StringGetAsync(key);
    }

    public async Task Set(string terminalId, string nsu, string transactionId) {
        var key = $"tx:{terminalId}:{nsu}";

        await _db.StringSetAsync(
            key,
            transactionId,
            TimeSpan.FromHours(24)
        );
    }
}