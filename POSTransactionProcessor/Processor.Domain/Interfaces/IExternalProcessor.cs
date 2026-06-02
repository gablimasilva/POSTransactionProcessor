namespace Processor.Domain.Interfaces;
public interface IExternalProcessor {
    Task<ExternalAuthorizationResult> Authorize(string nsu, decimal amount, string terminalId);
    Task<bool> Confirm(string transactionId);
    Task<bool> Undo(string transactionId);
}