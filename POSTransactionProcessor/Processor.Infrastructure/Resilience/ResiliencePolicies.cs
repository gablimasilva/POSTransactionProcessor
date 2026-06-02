using Polly;
using System.Diagnostics.Metrics;

namespace Processor.Infrastructure.Resilience;

public static class ResiliencePolicies {
    private static readonly Meter Meter = new("Processor.Resilience");

    private static readonly Counter<int> CircuitOpened =
        Meter.CreateCounter<int>("circuit_opened");

    private static readonly Counter<int> CircuitClosed =
        Meter.CreateCounter<int>("circuit_closed");

    public static AsyncPolicy CreatePolicy() {
        var circuitBreaker = Policy
            .Handle<Exception>()
            .CircuitBreakerAsync(
                exceptionsAllowedBeforeBreaking: 5,
                durationOfBreak: TimeSpan.FromSeconds(30),
                onBreak: (ex, ts) => {
                    CircuitOpened.Add(1);
                },
                onReset: () => {
                    CircuitClosed.Add(1);
                });

        var rand = new Random();
        var retry = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(3, attempt =>
                TimeSpan.FromMilliseconds((200 * Math.Pow(2, attempt)) + rand.Next(0, 100))
            );


        var timeout = Policy.TimeoutAsync(2);

        var bulkhead = Policy.BulkheadAsync(50, 100);

        return Policy.WrapAsync(bulkhead, timeout, retry, circuitBreaker);
    }
}