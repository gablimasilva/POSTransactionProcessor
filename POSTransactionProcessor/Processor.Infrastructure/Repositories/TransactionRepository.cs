using Microsoft.EntityFrameworkCore;
using Processor.Domain.Entities;
using Processor.Domain.Interfaces;

public class TransactionRepository : ITransactionRepository {
    private readonly ProcessorDbContext _context;

    public TransactionRepository(ProcessorDbContext context) {
        _context = context;
    }

    public async Task<Transaction?> GetById(string id) {
        return await _context.Transactions.FirstOrDefaultAsync(x => x.TransactionId == id);
    }

    public async Task<Transaction?> GetByNsuAndTerminal(string nsu, string terminal) {
        return await _context.Transactions
            .FirstOrDefaultAsync(x => x.Nsu == nsu && x.TerminalId == terminal);
    }

    public async Task Save(Transaction tx) {
        var exists = await _context.Transactions
            .AnyAsync(x => x.TransactionId == tx.TransactionId);

        if (!exists)
            _context.Transactions.Add(tx);
        else
            _context.Transactions.Update(tx);

        await _context.SaveChangesAsync();
    }
}