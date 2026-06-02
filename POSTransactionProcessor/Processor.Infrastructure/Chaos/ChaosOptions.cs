namespace Processor.Infrastructure.Chaos;

public class ChaosOptions {
    public bool Enabled { get; set; }
    public double FailureRate { get; set; } = 0.3;
}