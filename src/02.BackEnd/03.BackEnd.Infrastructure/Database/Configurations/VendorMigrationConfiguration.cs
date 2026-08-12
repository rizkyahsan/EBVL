namespace EBVL.BackEnd.Infrastructure.Database.Configurations;

public sealed class VendorMigrationRunConfiguration : IEntityTypeConfiguration<VendorMigrationRun>
{
    public void Configure(EntityTypeBuilder<VendorMigrationRun> builder)
    {
        _ = builder.ToTable("Runs", "Migration");
        _ = builder.HasKey(x => x.Id);
        _ = builder.Property(x => x.SourceDatabase).HasMaxLength(128).IsRequired();
        _ = builder.Property(x => x.Status).HasMaxLength(32).IsRequired();
    }
}

public sealed class VendorMigrationRowConfiguration : IEntityTypeConfiguration<VendorMigrationRow>
{
    public void Configure(EntityTypeBuilder<VendorMigrationRow> builder)
    {
        _ = builder.ToTable("Rows", "Migration");
        _ = builder.HasKey(x => x.Id);
        _ = builder.Property(x => x.SourceTable).HasMaxLength(128).IsRequired();
        _ = builder.Property(x => x.Outcome).HasMaxLength(32).IsRequired();
        _ = builder.Property(x => x.Reason).HasMaxLength(256);
        _ = builder.HasIndex(x => new { x.RunId, x.SourceTable, x.SourceId }).IsUnique();
    }
}

public sealed class VendorMigrationCrosswalkConfiguration : IEntityTypeConfiguration<VendorMigrationCrosswalk>
{
    public void Configure(EntityTypeBuilder<VendorMigrationCrosswalk> builder)
    {
        _ = builder.ToTable("Crosswalks", "Migration");
        _ = builder.HasKey(x => x.Id);
        _ = builder.Property(x => x.SourceTable).HasMaxLength(128).IsRequired();
        _ = builder.Property(x => x.TargetTable).HasMaxLength(128).IsRequired();
        _ = builder.HasIndex(x => new { x.SourceTable, x.SourceId }).IsUnique();
    }
}

public sealed class VendorMigrationQuarantineConfiguration : IEntityTypeConfiguration<VendorMigrationQuarantine>
{
    public void Configure(EntityTypeBuilder<VendorMigrationQuarantine> builder)
    {
        _ = builder.ToTable("Quarantines", "Migration");
        _ = builder.HasKey(x => x.Id);
        _ = builder.Property(x => x.SourceTable).HasMaxLength(128).IsRequired();
        _ = builder.Property(x => x.Reason).HasMaxLength(256).IsRequired();
        _ = builder.Property(x => x.Detail).HasMaxLength(2000);
        _ = builder.HasIndex(x => new { x.RunId, x.SourceTable, x.SourceId });
    }
}
