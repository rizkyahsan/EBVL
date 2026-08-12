namespace EBVL.BackEnd.Infrastructure.Database.Configurations;

public sealed class VendorDocumentConfiguration : IEntityTypeConfiguration<VendorDocument>
{
    public void Configure(EntityTypeBuilder<VendorDocument> builder)
    {
        _ = builder.ToTable(nameof(IDatabaseService.VendorDocuments), DatabaseService.SchemaName);
        builder.ConfigureModifiableProperties();
        _ = builder.Property(x => x.OriginalFileName).HasColumnType(ColumnTypeFor.Nvarchar(VendorsMaximumLengthFor.FileName));
        _ = builder.Property(x => x.StoredFileName).HasColumnType(ColumnTypeFor.Nvarchar(VendorsMaximumLengthFor.FileName));
        _ = builder.Property(x => x.FileContentType).HasColumnType(ColumnTypeFor.Nvarchar(100));
        _ = builder.Property(x => x.StorageFileId).HasColumnType(ColumnTypeFor.Nvarchar(500));
        _ = builder.Property(x => x.ContentHash).HasColumnType(ColumnTypeFor.Nvarchar(64));
        _ = builder.HasIndex(x => new { x.VendorId, x.DocumentTemplateId });
        _ = builder.HasOne(x => x.Vendor).WithMany(x => x.Documents).HasForeignKey(x => x.VendorId).OnDelete(DeleteBehavior.Restrict);
        _ = builder.HasOne(x => x.DocumentTemplate).WithMany(x => x.VendorDocuments).HasForeignKey(x => x.DocumentTemplateId).OnDelete(DeleteBehavior.Restrict);
    }
}
