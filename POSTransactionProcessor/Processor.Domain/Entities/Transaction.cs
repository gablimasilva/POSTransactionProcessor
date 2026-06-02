using Processor.Domain.Enums;

namespace Processor.Domain.Entities;

public class Transaction {
    public string TransactionId { get; set; }
    public string Nsu { get; set; }
    public string TerminalId { get; set; }
    public decimal Amount { get; set; }

    public TransactionStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }
}
