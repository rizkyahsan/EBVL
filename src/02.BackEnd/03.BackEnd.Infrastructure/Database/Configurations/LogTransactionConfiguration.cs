namespace EBVL.BackEnd.Infrastructure.Database.Configurations;

public sealed class LogTransactionConfiguration : IEntityTypeConfiguration<LogTransaction>
{
    public void Configure(EntityTypeBuilder<LogTransaction> builder)
    {
        _ = builder.ToTable(nameof(IDatabaseService.LogTransactions));

        builder.ConfigureModifiableProperties();

        // Foreign key relationship
        _ = builder.HasOne(b => b.Project)
        .WithMany(project => project.LogTransactions)
        .HasForeignKey(log => log.ProjectId)
        .OnDelete(DeleteBehavior.Cascade); // or Restrict, depending on your needs

        _ = builder.Property(entity => entity.Action)
            .HasColumnType(ColumnTypeFor.Nvarchar(LogTransactionsMaximumLengthFor.Action));

        _ = builder.Property(entity => entity.Role)
            .HasColumnType(ColumnTypeFor.Nvarchar(LogTransactionsMaximumLengthFor.Role));

    }
}
