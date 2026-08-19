namespace EBVL.BackEnd.Infrastructure.Database.Configurations;

public sealed class ExternalLoginConfiguration : IEntityTypeConfiguration<ExternalLogin>
{
    public void Configure(EntityTypeBuilder<ExternalLogin> builder)
    {
        _ = builder.ToTable(nameof(IDatabaseService.ExternalLogins));

        builder.ConfigureModifiableProperties();

        _ = builder.HasOne(x => x.User)
            .WithMany(x => x.ExternalLogins)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
