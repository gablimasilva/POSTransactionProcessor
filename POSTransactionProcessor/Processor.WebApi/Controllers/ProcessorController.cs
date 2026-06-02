using Microsoft.AspNetCore.Mvc;
using Processor.Application.DTOs.Requests;
using Serilog;

[ApiController]
[Route("v1/pos/transactions")]
public class ProcessorController : ControllerBase {
    private readonly AuthorizeTransactionUseCase _authorize;
    private readonly ConfirmTransactionUseCase _confirm;
    private readonly UndoTransactionUseCase _undo;

    public ProcessorController(
        AuthorizeTransactionUseCase authorize,
        ConfirmTransactionUseCase confirm,
        UndoTransactionUseCase undo) {
        _authorize = authorize;
        _confirm = confirm;
        _undo = undo;
    }

    [HttpPost("authorize")]
    public async Task<IActionResult> Authorize([FromBody] AuthorizeRequest request) {
        var result = await _authorize.Execute(request);
        return Ok(result);
    }

    [HttpPost("confirm")]
    public async Task<IActionResult> Confirm([FromBody] ConfirmRequest request) {
        await _confirm.Execute(request.TransactionId);
        return NoContent();
    }

    [HttpPost("void")]
    public async Task<IActionResult> Void([FromBody] UndoRequest request) {
        await _undo.Execute(request);
        return NoContent();
    }
}
