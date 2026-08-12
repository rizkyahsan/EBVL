namespace EBVL.BackEnd.Infrastructure.Database.Configurations;

public sealed class VendorRegistrationConfiguration : IEntityTypeConfiguration<VendorRegistration>
{
    public void Configure(EntityTypeBuilder<VendorRegistration> builder)
    {
        _ = builder.ToTable(nameof(IDatabaseService.VendorRegistrations));
        builder.ConfigureModifiableProperties();

        _ = builder.Property(x => x.SapVendorNumber).HasColumnType(ColumnTypeFor.Nvarchar(VendorsMaximumLengthFor.SapVendorNumber));
        _ = builder.Property(x => x.CompanyName).HasColumnType(ColumnTypeFor.Nvarchar(VendorsMaximumLengthFor.CompanyName));
        _ = builder.Property(x => x.CompanyEmail).HasColumnType(ColumnTypeFor.Nvarchar(VendorsMaximumLengthFor.EmailAddress));
        _ = builder.Property(x => x.PicEmail).HasColumnType(ColumnTypeFor.Nvarchar(VendorsMaximumLengthFor.EmailAddress));
        _ = builder.Property(x => x.CompanyPhone).HasColumnType(ColumnTypeFor.Nvarchar(VendorsMaximumLengthFor.PhoneNumber));
        _ = builder.Property(x => x.PicPhone).HasColumnType(ColumnTypeFor.Nvarchar(VendorsMaximumLengthFor.PhoneNumber));
        _ = builder.Property(x => x.Website).HasColumnType(ColumnTypeFor.Nvarchar(VendorsMaximumLengthFor.Url));
        _ = builder.Property(x => x.CompanyService).HasColumnType(ColumnTypeFor.Nvarchar(VendorsMaximumLengthFor.ShortText));
        _ = builder.Property(x => x.FactoryCountry).HasColumnType(ColumnTypeFor.Nvarchar(VendorsMaximumLengthFor.ShortText));
        _ = builder.Property(x => x.FactoryAddress).HasColumnType(ColumnTypeFor.Nvarchar(VendorsMaximumLengthFor.Address));
        _ = builder.Property(x => x.BrandRepresentative).HasColumnType(ColumnTypeFor.Nvarchar(VendorsMaximumLengthFor.CompanyName));
        _ = builder.Property(x => x.CompanyStatus).HasColumnType(ColumnTypeFor.Nvarchar(VendorsMaximumLengthFor.ShortText));
        _ = builder.Property(x => x.IndonesiaRepresentativeName).HasColumnType(ColumnTypeFor.Nvarchar(VendorsMaximumLengthFor.CompanyName));
        _ = builder.Property(x => x.BrandRegistrationLetterFileName).HasColumnType(ColumnTypeFor.Nvarchar(VendorsMaximumLengthFor.FileName));
        _ = builder.Property(x => x.CompanyProfileFileName).HasColumnType(ColumnTypeFor.Nvarchar(VendorsMaximumLengthFor.FileName));
        _ = builder.Property(x => x.ProductCatalogFileName).HasColumnType(ColumnTypeFor.Nvarchar(VendorsMaximumLengthFor.FileName));
        _ = builder.Property(x => x.ProjectExperienceFileName).HasColumnType(ColumnTypeFor.Nvarchar(VendorsMaximumLengthFor.FileName));
        _ = builder.Property(x => x.TaxCardFileName).HasColumnType(ColumnTypeFor.Nvarchar(VendorsMaximumLengthFor.FileName));
        _ = builder.Property(x => x.MainCertificateFileName).HasColumnType(ColumnTypeFor.Nvarchar(VendorsMaximumLengthFor.FileName));
        _ = builder.HasIndex(x => x.SapVendorNumber).IsUnique();
        _ = builder.HasIndex(x => x.Status);
    }
}
