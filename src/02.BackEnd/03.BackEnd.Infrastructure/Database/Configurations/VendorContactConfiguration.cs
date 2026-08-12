namespace EBVL.BackEnd.Infrastructure.Database.Configurations;

public sealed class VendorContactConfiguration : IEntityTypeConfiguration<VendorContact>
{
    public void Configure(EntityTypeBuilder<VendorContact> builder)
    {
        _ = builder.ToTable(nameof(IDatabaseService.VendorContacts), DatabaseService.SchemaName);
        builder.ConfigureModifiableProperties();
        _ = builder.Property(x => x.Name).HasColumnType(ColumnTypeFor.Nvarchar(250));
        _ = builder.Property(x => x.Address).HasColumnType(ColumnTypeFor.Nvarchar(500));
        _ = builder.Property(x => x.Phone).HasColumnType(ColumnTypeFor.Nvarchar(50));
        _ = builder.Property(x => x.Email).HasColumnType(ColumnTypeFor.Nvarchar(320));
        _ = builder.HasIndex(x => x.VendorId);
        _ = builder.HasIndex(x => x.ContactTypeId);
        _ = builder.HasOne(x => x.Vendor).WithMany(x => x.Contacts).HasForeignKey(x => x.VendorId).OnDelete(DeleteBehavior.Restrict);
        _ = builder.HasOne(x => x.ContactType).WithMany(x => x.Contacts).HasForeignKey(x => x.ContactTypeId).OnDelete(DeleteBehavior.Restrict);
    }
}
