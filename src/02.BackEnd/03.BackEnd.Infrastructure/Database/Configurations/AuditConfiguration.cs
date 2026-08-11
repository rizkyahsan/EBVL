namespace EBVL.BackEnd.Infrastructure.Database.Configurations;

public sealed class AuditConfiguration : IEntityTypeConfiguration<Audit>
{
    public void Configure(EntityTypeBuilder<Audit> builder)
    {
        _ = builder.ToTable(nameof(IDatabaseService.Audits));

        builder.ConfigureCreatableProperties();

        _ = builder.Property(entity => entity.EntityName)
            .HasColumnType(ColumnTypeFor.Nvarchar(AuditsMaximumLengthFor.EntityName));

        _ = builder.Property(entity => entity.ActionName)
            .HasColumnType(ColumnTypeFor.Nvarchar(AuditsMaximumLengthFor.ActionName));
    }
}
