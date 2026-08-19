namespace EBVL.BackEnd.Infrastructure.Database.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        _ = builder.ToTable(nameof(IDatabaseService.Users));

        builder.ConfigureModifiableProperties();

        _ = builder.Property(entity => entity.Username)
            .HasColumnType(ColumnTypeFor.Nvarchar(CommonMaximumLengthFor.UserPrincipalName));

        _ = builder.Property(entity => entity.DisplayName)
            .HasColumnType(ColumnTypeFor.Nvarchar(CommonMaximumLengthFor.PersonName));

        _ = builder.Property(entity => entity.EmailAddress)
            .HasColumnType(ColumnTypeFor.Nvarchar(CommonMaximumLengthFor.EmailAddress));

        _ = builder.Property(entity => entity.PhoneNumber)
            .HasColumnType(ColumnTypeFor.Nvarchar(CommonMaximumLengthFor.PhoneNumber));

        _ = builder.Property(entity => entity.OtpSecret)
            .HasColumnType(ColumnTypeFor.Nvarchar(UsersMaximumLengthFor.OtpSecret));

        _ = builder.Property(entity => entity.OtpUrl)
            .HasColumnType(ColumnTypeFor.Nvarchar(CommonMaximumLengthFor.Url));

        _ = builder.HasOne(x => x.Lender)
            .WithMany(x => x.Users)
            .HasForeignKey(x => x.LenderId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
