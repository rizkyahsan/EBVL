namespace EBVL.BackEnd.Infrastructure.Database.Configurations;

public sealed class DocumentConfiguration : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        _ = builder.ToTable(nameof(IDatabaseService.Documents));

        builder.ConfigureFileProperties();

        _ = builder.Property(entity => entity.Description)
            .HasColumnType(ColumnTypeFor.Nvarchar(DocumentsMaximumLengthFor.Description));
    }
}
