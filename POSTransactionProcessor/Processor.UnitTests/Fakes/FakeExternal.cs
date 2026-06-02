using Processor.Domain.Interfaces;

public class FakeExternal : IExternalProcessor {
    public Task<ExternalAuthorizationResult> Authorize(string nsu, decimal amount, string terminalId)
        => Task.FromResult(new ExternalAuthorizationResult {
            Approved = true,
            AuthorizationCode = "FAKE123",
            Message = "Approved (fake)"
        });

    public Task<bool> Confirm(string transactionId)
        => Task.FromResult(true);

    public Task<bool> Undo(string transactionId)
        => Task.FromResult(true);
}