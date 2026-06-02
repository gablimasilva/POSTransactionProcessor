public interface IIdempotencyCache {
    Task<string?> GetTransactionId(string terminalId, string nsu);
    Task Set(string terminalId, string nsu, string transactionId);
}