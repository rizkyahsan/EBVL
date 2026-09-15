namespace EBVL.BackEnd.Infrastructure.Database.Configurations;

public sealed class ExternalLoginConfiguration : IEntityTypeConfiguration<ExternalLogin>
{
    public void Configure(EntityTypeBuilder<ExternalLogin> builder)
    {
        _ = builder.ToTable(nameof(IDatabaseService.ExternalLogins));

        builder.ConfigureModifiableProperties();
        _ = builder.Property(x => x.Username).HasMaxLength(320);
        _ = builder.Property(x => x.IpAddress).HasMaxLength(ExternalLoginLogsMaximumLengthFor.IpAddress);

        _ = builder.HasOne(x => x.User)
            .WithMany(x => x.ExternalLogins)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
