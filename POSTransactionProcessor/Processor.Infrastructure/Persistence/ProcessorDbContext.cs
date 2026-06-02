using Microsoft.EntityFrameworkCore;
using Processor.Domain.Entities;

public class ProcessorDbContext : DbContext {
    public ProcessorDbContext(DbContextOptions<ProcessorDbContext> options)
        : base(options) { }

    public DbSet<Transaction> Transactions => Set<Transaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProcessorDbContext).Assembly);
    }
}