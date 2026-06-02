using Processor.Application.DTOs.Requests;
using Processor.Domain.Entities;
using Processor.Domain.Enums;
using Processor.Domain.Exceptions;
using Processor.Domain.Interfaces;

public class UndoTransactionUseCase {
    private readonly ITransactionRepository _repo;
    private readonly IExternalProcessor _external;

    public UndoTransactionUseCase(ITransactionRepository repo, IExternalProcessor external) {
        _repo = repo;
        _external = external;
    }


    public async Task<bool> Execute(UndoRequest request) {
        Transaction? tx = null;

        if (!string.IsNullOrEmpty(request.TransactionId)) {
            tx = await _repo.GetById(request.TransactionId);
        } else {
            tx = await _repo.GetByNsuAndTerminal(
                request.Nsu,
                request.TerminalId);
        }

        if (tx == null)
            throw new NotFoundException("Transaction not found");

        if (tx.Status == TransactionStatus.VOIDED)
            return true;

        var success = await _external.Undo(tx.TransactionId);
        if (!success)
            throw new ExternalAuthorizationException("Void failed");

        tx.Status = TransactionStatus.VOIDED;

        await _repo.Save(tx);

        return true;
    }

}