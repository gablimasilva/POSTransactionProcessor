public class InMemoryIdempotencyCache : IIdempotencyCache {
    private static readonly Dictionary<string, string> _cache = new();

    public Task<string?> GetTransactionId(string terminalId, string nsu) {
        _cache.TryGetValue($"{terminalId}:{nsu}", out var value);
        return Task.FromResult(value);
    }

    public Task Set(string terminalId, string nsu, string transactionId) {
        _cache[$"{terminalId}:{nsu}"] = transactionId;
        return Task.CompletedTask;
    }
}
