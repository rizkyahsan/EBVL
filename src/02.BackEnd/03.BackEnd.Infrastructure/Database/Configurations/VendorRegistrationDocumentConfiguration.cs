namespace EBVL.BackEnd.Infrastructure.Database.Configurations;

public sealed class VendorRegistrationDocumentConfiguration : IEntityTypeConfiguration<VendorRegistrationDocument>
{
    public void Configure(EntityTypeBuilder<VendorRegistrationDocument> builder)
    {
        _ = builder.ToTable(nameof(IDatabaseService.VendorRegistrationDocuments));
        builder.ConfigureModifiableProperties();
        _ = builder.Property(x => x.OriginalFileName).HasColumnType(ColumnTypeFor.Nvarchar(VendorsMaximumLengthFor.FileName));
        _ = builder.Property(x => x.StoredFileName).HasColumnType(ColumnTypeFor.Nvarchar(VendorsMaximumLengthFor.FileName));
        _ = builder.Property(x => x.FileContentType).HasColumnType(ColumnTypeFor.Nvarchar(100));
        _ = builder.Property(x => x.StorageFileId).HasColumnType(ColumnTypeFor.Nvarchar(500));
        _ = builder.Property(x => x.ContentHash).HasColumnType(ColumnTypeFor.Nvarchar(64));
        _ = builder.HasIndex(x => new { x.VendorRegistrationId, x.DocumentTemplateId }).IsUnique();
        _ = builder.HasOne(x => x.VendorRegistration).WithMany(x => x.Documents).HasForeignKey(x => x.VendorRegistrationId).OnDelete(DeleteBehavior.Cascade);
        _ = builder.HasOne(x => x.DocumentTemplate).WithMany().HasForeignKey(x => x.DocumentTemplateId).OnDelete(DeleteBehavior.Restrict);
    }
}
