namespace EBVL.BackEnd.Infrastructure.Database.Configurations;

public sealed class VendorTypeConfiguration : IEntityTypeConfiguration<VendorType>
{
    public void Configure(EntityTypeBuilder<VendorType> builder)
    {
        _ = builder.ToTable(nameof(IDatabaseService.VendorTypes), DatabaseService.SchemaName);
        builder.ConfigureModifiableProperties();
        _ = builder.Property(x => x.Name).HasColumnType(ColumnTypeFor.Nvarchar(100));
        _ = builder.Property(x => x.Description).HasColumnType(ColumnTypeFor.Nvarchar(500));
    }
}
