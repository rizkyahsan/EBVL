namespace EBVL.BackEnd.Infrastructure.Database.Configurations;

public sealed class ProjectStageConfiguration : IEntityTypeConfiguration<ProjectStage>
{
    public void Configure(EntityTypeBuilder<ProjectStage> builder)
    {
        _ = builder.ToTable(nameof(IDatabaseService.ProjectStages));

        builder.ConfigureModifiableProperties();

        _ = builder.Property(entity => entity.Name)
            .HasColumnType(ColumnTypeFor.Nvarchar(ProjectStagesMaximumLengthFor.Name));

        _ = builder.HasOne(x => x.Project)
            .WithMany(x => x.ProjectStages)
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
