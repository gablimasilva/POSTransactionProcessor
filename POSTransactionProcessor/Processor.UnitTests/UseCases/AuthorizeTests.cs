using Processor.Application.DTOs.Requests;

namespace Processor.UnitTests.UseCases;

public class AuthorizeTests {
    [Fact]
    public async Task Should_Be_Idempotent() {
        var repo = new FakeRepo();
        var external = new FakeExternal();
        var cache = new FakeCache();

        var usecase = new AuthorizeTransactionUseCase(repo, external, cache);

        var request = new AuthorizeRequest {
            Nsu = "123",
            TerminalId = "T1",
            Amount = 100
        };

        var first = await usecase.Execute(request);
        var second = await usecase.Execute(request);

        Assert.Equal(first.TransactionId, second.TransactionId);
    }
}
