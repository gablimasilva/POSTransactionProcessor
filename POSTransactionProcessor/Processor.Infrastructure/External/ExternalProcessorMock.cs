using Polly;
using Polly.CircuitBreaker;
using Polly.Timeout;
using Processor.Domain.Exceptions;
using Processor.Domain.Interfaces;
using Processor.Infrastructure.Chaos;
using Processor.Infrastructure.Resilience;

public class ExternalProcessorMock : IExternalProcessor {
    private readonly AsyncPolicy _policy;
    private readonly ChaosOptions _chaos;
    private readonly Random _random = new();

    public ExternalProcessorMock(ChaosOptions chaos) {
        _chaos = chaos;
        _policy = ResiliencePolicies.CreatePolicy();
    }

    public async Task<ExternalAuthorizationResult> Authorize(string nsu, decimal amount, string terminalId) {
        return await ExecuteWithResilience(async () => {
            SimulateFailure();
            await Task.Delay(100);

            return new ExternalAuthorizationResult {
                Approved = true,
                AuthorizationCode = Guid.NewGuid().ToString("N")[..6].ToUpper(),
                Message = "Approved"
            };
        });
    }

    public async Task<bool> Confirm(string transactionId) {
        return await ExecuteWithResilience(async () => {
            SimulateFailure();
            await Task.Delay(100);
            return true;
        });
    }

    public async Task<bool> Undo(string transactionId) {
        return await ExecuteWithResilience(async () => {
            SimulateFailure();
            await Task.Delay(100);
            return true;
        });
    }

    private async Task<T> ExecuteWithResilience<T>(Func<Task<T>> action) {
        try {
            return await _policy.ExecuteAsync(action);
        }
        catch (BrokenCircuitException) {
            throw new ExternalServiceUnavailableException("Circuit breaker aberto - serviço indisponível");
        }
        catch (TimeoutRejectedException) {
            throw new ExternalServiceUnavailableException("Timeout ao chamar serviço externo");
        }
        catch (Exception ex) {
            throw new ExternalServiceUnavailableException($"Erro externo: {ex.Message}");
        }
    }

    private void SimulateFailure() {
        if (!_chaos.Enabled)
            return;

        if (_random.NextDouble() < _chaos.FailureRate) {
            throw new Exception("🔥 Chaos: simulated external failure");
        }
    }
}