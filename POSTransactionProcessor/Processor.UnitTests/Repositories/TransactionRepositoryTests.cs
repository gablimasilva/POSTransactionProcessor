using Microsoft.EntityFrameworkCore;
using Processor.Domain.Entities;

namespace Processor.UnitTests.Repositories;

public class TransactionRepositoryTests {
    [Fact]
    public async Task Should_Save_And_Retrieve() {
        var options = new DbContextOptionsBuilder<ProcessorDbContext>()
            .UseInMemoryDatabase("test-db")
            .Options;

        var context = new ProcessorDbContext(options);
        var repo = new TransactionRepository(context);

        var tx = new Transaction {
            TransactionId = "1",
            Nsu = "123",
            TerminalId = "T1"
        };

        await repo.Save(tx);

        var result = await repo.GetById("1");

        Assert.NotNull(result);
    }
}