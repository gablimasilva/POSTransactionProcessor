using Processor.Domain.Enums;
using Processor.Domain.Interfaces;
using Processor.Domain.Exceptions;

public class ConfirmTransactionUseCase {
    private readonly ITransactionRepository _repo;
    private readonly IExternalProcessor _external;

    public ConfirmTransactionUseCase(ITransactionRepository repo, IExternalProcessor external) {
        _repo = repo;
        _external = external;
    }


    public async Task<bool> Execute(string transactionId) {
        var tx = await _repo.GetById(transactionId);

        if (tx == null)
            throw new NotFoundException("Transaction not found");

        if (tx.Status == TransactionStatus.CONFIRMED)
            return true;

        if (tx.Status != TransactionStatus.AUTHORIZED)
            throw new InvalidOperationException("Invalid transaction state");

        var success = await _external.Confirm(transactionId);
        if (!success)
            throw new ExternalAuthorizationException("Confirm failed");

        tx.Status = TransactionStatus.CONFIRMED;

        await _repo.Save(tx);

        return true;
    }

}
