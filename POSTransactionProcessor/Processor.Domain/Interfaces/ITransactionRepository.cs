using Processor.Domain.Entities;

namespace Processor.Domain.Interfaces;

public interface ITransactionRepository {
    Task<Transaction?> GetById(string transactionId);
    Task<Transaction?> GetByNsuAndTerminal(string nsu, string terminalId);
    Task Save(Transaction transaction);
}
