using Processor.Domain.Entities;
using Processor.Domain.Interfaces;

public class FakeRepo : ITransactionRepository {
    private readonly Dictionary<string, Transaction> _data = new();

    public FakeRepo() { }

    public FakeRepo(Transaction tx) {
        _data[tx.TransactionId] = tx;
    }

    public Task<Transaction?> GetById(string id) {
        _data.TryGetValue(id, out var tx);
        return Task.FromResult(tx);
    }

    public Task<Transaction?> GetByNsuAndTerminal(string nsu, string terminalId) {
        return Task.FromResult(_data.Values
            .FirstOrDefault(x => x.Nsu == nsu && x.TerminalId == terminalId));
    }

    public Task Save(Transaction tx) {
        _data[tx.TransactionId] = tx;
        return Task.CompletedTask;
    }
}