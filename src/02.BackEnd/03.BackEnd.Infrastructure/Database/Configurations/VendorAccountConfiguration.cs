namespace EBVL.BackEnd.Infrastructure.Database.Configurations;

public sealed class VendorAccountConfiguration : IEntityTypeConfiguration<VendorAccount>
{
    public void Configure(EntityTypeBuilder<VendorAccount> builder)
    {
        _ = builder.ToTable(nameof(IDatabaseService.VendorAccounts));
        builder.ConfigureModifiableProperties();

        _ = builder.Property(x => x.EmailAddress).HasColumnType(ColumnTypeFor.Nvarchar(VendorsMaximumLengthFor.EmailAddress));
        _ = builder.Property(x => x.PasswordHash).HasColumnType(ColumnTypeFor.Nvarchar(VendorsMaximumLengthFor.PasswordHash));
        _ = builder.Property(x => x.PasswordSalt).HasColumnType(ColumnTypeFor.Nvarchar(VendorsMaximumLengthFor.PasswordHash));
        _ = builder.HasIndex(x => x.EmailAddress).IsUnique();
        _ = builder.Property(x => x.Status).HasColumnType("int");
        _ = builder.HasIndex(x => x.VendorId);
        _ = builder.HasIndex(x => x.VendorRegistrationId).IsUnique().HasFilter("[VendorRegistrationId] IS NOT NULL");
        _ = builder.HasOne(x => x.Vendor)
            .WithMany(x => x.Accounts)
            .HasForeignKey(x => x.VendorId)
            .OnDelete(DeleteBehavior.Restrict);
        _ = builder.HasOne(x => x.VendorRegistration)
            .WithMany()
            .HasForeignKey(x => x.VendorRegistrationId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
