namespace EBVL.BackEnd.Infrastructure.Database.Configurations;

public sealed class DocumentDefinitionConfiguration : IEntityTypeConfiguration<DocumentDefinition>
{
    public void Configure(EntityTypeBuilder<DocumentDefinition> builder)
    {
        _ = builder.ToTable(nameof(IDatabaseService.DocumentDefinitions));
        builder.ConfigureModifiableProperties();
        _ = builder.Property(x => x.Code).HasColumnType(ColumnTypeFor.Nvarchar(100));
        _ = builder.Property(x => x.BusinessProcess).HasColumnType(ColumnTypeFor.Nvarchar(100));
        _ = builder.Property(x => x.Name).HasColumnType(ColumnTypeFor.Nvarchar(200));
        _ = builder.Property(x => x.RowVersion).IsRowVersion();
        _ = builder.HasIndex(x => new { x.BusinessProcess, x.Code }).IsUnique().HasFilter("[IsDeleted] = 0");
        _ = builder.HasIndex(x => new { x.BusinessProcess, x.Name }).IsUnique().HasFilter("[IsDeleted] = 0");
    }
}
