namespace EBVL.BackEnd.Infrastructure.Database.Configurations;

public sealed class VendorConfiguration : IEntityTypeConfiguration<Vendor>
{
    public void Configure(EntityTypeBuilder<Vendor> builder)
    {
        _ = builder.ToTable(nameof(IDatabaseService.Vendors), DatabaseService.SchemaName);
        builder.ConfigureModifiableProperties();
        _ = builder.Property(x => x.SapVendorNumber).HasColumnType(ColumnTypeFor.Nvarchar(VendorsMaximumLengthFor.SapVendorNumber));
        _ = builder.Property(x => x.Name).HasColumnType(ColumnTypeFor.Nvarchar(VendorsMaximumLengthFor.CompanyName));
        _ = builder.Property(x => x.Email).HasColumnType(ColumnTypeFor.Nvarchar(VendorsMaximumLengthFor.EmailAddress));
        _ = builder.Property(x => x.TaxId).HasColumnType(ColumnTypeFor.Nvarchar(100));
        _ = builder.Property(x => x.Website).HasColumnType(ColumnTypeFor.Nvarchar(VendorsMaximumLengthFor.Url));
        _ = builder.Property(x => x.LegacyConfirmedStatus).HasColumnType(ColumnTypeFor.Nvarchar(50));
        _ = builder.HasIndex(x => x.SapVendorNumber).IsUnique();
        _ = builder.HasIndex(x => x.Email).IsUnique();
        _ = builder.HasIndex(x => x.TaxId).IsUnique().HasFilter("[TaxId] IS NOT NULL");
        _ = builder.HasOne(x => x.VendorType).WithMany(x => x.Vendors).HasForeignKey(x => x.VendorTypeId).OnDelete(DeleteBehavior.Restrict);
    }
}
