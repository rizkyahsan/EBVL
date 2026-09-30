namespace EBVL.BackEnd.Infrastructure.Database.Configurations;

public sealed class BrandRegistrationConfiguration : IEntityTypeConfiguration<BrandRegistration>
{
    public void Configure(EntityTypeBuilder<BrandRegistration> builder)
    {
        _ = builder.ToTable(nameof(IDatabaseService.BrandRegistrations));
        builder.ConfigureModifiableProperties();
        _ = builder.Property(x => x.OwnerUsername).HasMaxLength(320);
        _ = builder.Property(x => x.BrandName).HasMaxLength(200);
        _ = builder.Property(x => x.ProductName).HasMaxLength(200);
        _ = builder.Property(x => x.Group).HasMaxLength(200);
        _ = builder.Property(x => x.FactoryCountry).HasMaxLength(200);
        _ = builder.Property(x => x.Category).HasMaxLength(100);
        _ = builder.Property(x => x.ProductDescription).HasMaxLength(2000);
        _ = builder.HasIndex(x => x.WorkflowCaseId).IsUnique();
        _ = builder.HasOne(x => x.WorkflowCase).WithOne().HasForeignKey<BrandRegistration>(x => x.WorkflowCaseId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class BrandInvitationConfiguration : IEntityTypeConfiguration<BrandInvitation>
{
    public void Configure(EntityTypeBuilder<BrandInvitation> builder)
    {
        _ = builder.ToTable(nameof(IDatabaseService.BrandInvitations));
        builder.ConfigureCreatableProperties();
        _ = builder.Property(x => x.Subject).HasMaxLength(300);
        _ = builder.Property(x => x.Recipient).HasMaxLength(1000);
        _ = builder.Property(x => x.Cc).HasMaxLength(1000);
        _ = builder.Property(x => x.Body).HasColumnType("nvarchar(max)");
        _ = builder.Property(x => x.SentBy).HasMaxLength(320);
        _ = builder.Property(x => x.RespondedBy).HasMaxLength(320);
        _ = builder.Property(x => x.Response).HasConversion<string>().HasMaxLength(20);
        _ = builder.HasIndex(x => new { x.BrandRegistrationId, x.SentAt });
        _ = builder.HasOne(x => x.BrandRegistration).WithMany(x => x.Invitations).HasForeignKey(x => x.BrandRegistrationId).OnDelete(DeleteBehavior.Cascade);
    }
}
