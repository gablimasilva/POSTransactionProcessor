using Processor.Domain.Entities;
using Processor.Domain.Enums;

namespace Processor.UnitTests.UseCases;

public class ConfirmTests {
    [Fact]
    public async Task Should_Not_Confirm_Twice() {
        var tx = new Transaction {
            TransactionId = "1",
            Status = TransactionStatus.AUTHORIZED
        };

        var repo = new FakeRepo(tx);
        var external = new FakeExternal();

        var usecase = new ConfirmTransactionUseCase(repo, external);

        await usecase.Execute("1");
        await usecase.Execute("1");

        Assert.Equal(TransactionStatus.CONFIRMED, tx.Status);
    }
}