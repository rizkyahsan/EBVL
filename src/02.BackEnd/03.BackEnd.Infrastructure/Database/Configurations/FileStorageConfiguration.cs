namespace EBVL.BackEnd.Infrastructure.Database.Configurations;

public sealed class FileStorageConfiguration : IEntityTypeConfiguration<Domain.Entities.FileStorage>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.FileStorage> builder)
    {
        _ = builder.ToTable(nameof(IDatabaseService.FileStorages));

        builder.ConfigureFileProperties();

        _ = builder.Property(entity => entity.FileExtension)
            .HasColumnType(ColumnTypeFor.Nvarchar(FileStoragesMaximumLengthFor.FileExtension));

        _ = builder.Property(entity => entity.FileHash)
            .HasColumnType(ColumnTypeFor.Nvarchar(FileStoragesMaximumLengthFor.FileHash));
    }
}
