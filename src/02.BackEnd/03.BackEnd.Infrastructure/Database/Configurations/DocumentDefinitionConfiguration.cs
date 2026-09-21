namespace EBVL.BackEnd.Infrastructure.Database.Configurations;

public sealed class DocumentDefinitionConfiguration : IEntityTypeConfiguration<DocumentDefinition>
{
    public void Configure(EntityTypeBuilder<DocumentDefinition> builder)
    {
        _ = builder.ToTable(nameof(IDatabaseService.DocumentDefinitions));
        builder.ConfigureModifiableProperties();
        _ = builder.Property(x => x.Code).HasColumnType(ColumnTypeFor.Nvarchar(100));
        _ = builder.Property(x => x.BusinessProcess).HasColumnType(ColumnTypeFor.Nvarchar(100));
        _ = builder.Property(x => x.Name).HasColumnType(ColumnTypeFor.Nvarchar(200));
        _ = builder.Property(x => x.RowVersion).IsRowVersion();
        _ = builder.HasIndex(x => new { x.DocumentRequirementSetId, x.Code }).IsUnique().HasFilter("[IsDeleted] = 0");
        _ = builder.HasIndex(x => new { x.DocumentRequirementSetId, x.Name }).IsUnique().HasFilter("[IsDeleted] = 0");
        _ = builder.HasOne(x => x.DocumentRequirementSet).WithMany(x => x.Requirements).HasForeignKey(x => x.DocumentRequirementSetId).OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class DocumentRequirementSetConfiguration : IEntityTypeConfiguration<DocumentRequirementSet>
{
    public void Configure(EntityTypeBuilder<DocumentRequirementSet> builder)
    {
        _ = builder.ToTable(nameof(IDatabaseService.DocumentRequirementSets));
        builder.ConfigureModifiableProperties();
        _ = builder.Property(x => x.BusinessProcess).HasColumnType(ColumnTypeFor.Nvarchar(100));
        _ = builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
        _ = builder.Property(x => x.PublishedBy).HasMaxLength(200);
        _ = builder.Property(x => x.RowVersion).IsRowVersion();
        _ = builder.HasIndex(x => new { x.DocumentRequirementSetSeriesId, x.Version }).IsUnique();
        _ = builder.HasIndex(x => x.BusinessProcess, "IX_DocumentRequirementSets_OneSeriesPerProcess").IsUnique().HasFilter("[Version] = 1 AND [IsDeleted] = 0");
        _ = builder.HasIndex(x => x.DocumentRequirementSetSeriesId, "IX_DocumentRequirementSets_OneDraftPerSeries").IsUnique().HasFilter("[Status] = 'Draft' AND [IsDeleted] = 0");
        _ = builder.HasIndex(x => x.DocumentRequirementSetSeriesId, "IX_DocumentRequirementSets_OnePublishPerSeries").IsUnique().HasFilter("[Status] = 'Publish' AND [IsDeleted] = 0");
        _ = builder.HasOne(x => x.PreviousVersion).WithMany().HasForeignKey(x => x.PreviousVersionId).OnDelete(DeleteBehavior.Restrict);
    }
}
