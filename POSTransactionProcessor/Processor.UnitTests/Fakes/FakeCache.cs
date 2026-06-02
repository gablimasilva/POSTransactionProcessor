public class FakeCache : IIdempotencyCache {
    private readonly Dictionary<string, string> _data = new();

    public Task<string?> GetTransactionId(string terminalId, string nsu) {
        _data.TryGetValue($"{terminalId}:{nsu}", out var value);
        return Task.FromResult(value);
    }

    public Task Set(string terminalId, string nsu, string transactionId) {
        _data[$"{terminalId}:{nsu}"] = transactionId;
        return Task.CompletedTask;
    }
}