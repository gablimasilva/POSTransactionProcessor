using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Processor.Domain.Entities;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction> {
    public void Configure(EntityTypeBuilder<Transaction> builder) {
        builder.HasKey(t => t.TransactionId);

        builder.HasIndex(t => new { t.TerminalId, t.Nsu })
            .IsUnique();

        builder.Property(t => t.Amount).IsRequired();
        builder.Property(t => t.Status).IsRequired();
    }
}
