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
        _ = builder.HasIndex(x => x.VendorRegistrationId).IsUnique();
        _ = builder.HasOne(x => x.VendorRegistration)
            .WithOne(x => x.Account)
            .HasForeignKey<VendorAccount>(x => x.VendorRegistrationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
