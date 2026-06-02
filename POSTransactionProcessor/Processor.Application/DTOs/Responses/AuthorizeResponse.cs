namespace Processor.Application.DTOs.Responses {

    public class AuthorizeResponse {
        public string Nsu { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string TerminalId { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
    }

}
