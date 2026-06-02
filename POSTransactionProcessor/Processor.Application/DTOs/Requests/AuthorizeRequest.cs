namespace Processor.Application.DTOs.Requests {

    public class AuthorizeRequest {
        public string Nsu { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string TerminalId { get; set; } = string.Empty;
    }

}
