using Microsoft.EntityFrameworkCore;
using Processor.Application.DTOs.Requests;
using Processor.Application.DTOs.Responses;
using Processor.Domain.Entities;
using Processor.Domain.Enums;
using Processor.Domain.Exceptions;
using Processor.Domain.Interfaces;

public class AuthorizeTransactionUseCase {
    private readonly ITransactionRepository _repo;
    private readonly IExternalProcessor _external;
    private readonly IIdempotencyCache _cache;

    public AuthorizeTransactionUseCase(
        ITransactionRepository repo,
        IExternalProcessor external,
        IIdempotencyCache cache) {
        _repo = repo;
        _external = external;
        _cache = cache;
    }

    public async Task<AuthorizeResponse> Execute(AuthorizeRequest request) {
        var cachedId = await _cache.GetTransactionId(request.TerminalId, request.Nsu);
        if (cachedId != null) {
            var transaction = await _repo.GetById(cachedId);
            if (transaction != null)
                return Map(transaction);
        }

        var existing = await _repo.GetByNsuAndTerminal(request.Nsu, request.TerminalId);
        if (existing != null) {
            await _cache.Set(request.TerminalId, request.Nsu, existing.TransactionId);
            return Map(existing);
        }

        var response = await _external.Authorize(request.Nsu, request.Amount, request.TerminalId);
        if (!response.Approved) {
            throw new ExternalAuthorizationException(response.Message);
        }

        var tx = new Transaction {
            TransactionId = Guid.NewGuid().ToString("N"),
            Nsu = request.Nsu,
            TerminalId = request.TerminalId,
            Amount = request.Amount,
            Status = TransactionStatus.AUTHORIZED,
            CreatedAt = DateTime.UtcNow
        };


        try {
            await _repo.Save(tx);
        }
        catch (DbUpdateException) {
            existing = await _repo.GetByNsuAndTerminal(request.Nsu, request.TerminalId);
            if (existing == null) {
                throw new ExternalServiceUnavailableException("External service unavailable");
            }
            return Map(existing);
        }

        await _cache.Set(tx.TerminalId, tx.Nsu, tx.TransactionId);

        return Map(tx);
    }

    private AuthorizeResponse Map(Transaction tx) => new() {
        Nsu = tx.Nsu,
        Amount = tx.Amount,
        TerminalId = tx.TerminalId,
        TransactionId = tx.TransactionId
    };
}
