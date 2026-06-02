using Processor.Application.DTOs.Requests;
using Processor.Domain.Entities;
using Processor.Domain.Enums;

namespace Processor.UnitTests.UseCases;

public class UndoTests {
    [Fact]
    public async Task Should_Be_Idempotent() {
        var tx = new Transaction {
            TransactionId = "1",
            Status = TransactionStatus.AUTHORIZED
        };

        var repo = new FakeRepo(tx);
        var external = new FakeExternal();

        var usecase = new UndoTransactionUseCase(repo, external);

        var request = new UndoRequest { TransactionId = "1" };

        await usecase.Execute(request);
        await usecase.Execute(request);

        Assert.Equal(TransactionStatus.VOIDED, tx.Status);
    }
}
