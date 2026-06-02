namespace Processor.Application.DTOs.Requests {

    public class UndoRequest {
        public string TransactionId { get; set; } = string.Empty; 
        public string Nsu { get; set; } = string.Empty;
        public string TerminalId { get; set; } = string.Empty;
    }

}
