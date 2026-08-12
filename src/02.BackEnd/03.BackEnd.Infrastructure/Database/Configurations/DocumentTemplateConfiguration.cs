namespace EBVL.BackEnd.Infrastructure.Database.Configurations;

public sealed class DocumentTemplateConfiguration : IEntityTypeConfiguration<DocumentTemplate>
{
    public void Configure(EntityTypeBuilder<DocumentTemplate> builder)
    {
        _ = builder.ToTable(nameof(IDatabaseService.DocumentTemplates), DatabaseService.SchemaName);
        builder.ConfigureModifiableProperties();
        _ = builder.Property(x => x.Name).HasColumnType(ColumnTypeFor.Nvarchar(200));
        _ = builder.Property(x => x.Alias).HasColumnType(ColumnTypeFor.Nvarchar(200));
        _ = builder.HasIndex(x => x.Alias).IsUnique().HasFilter("[Alias] IS NOT NULL");
    }
}
