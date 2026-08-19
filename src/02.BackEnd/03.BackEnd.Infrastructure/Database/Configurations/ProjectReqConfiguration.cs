namespace EBVL.BackEnd.Infrastructure.Database.Configurations;

public sealed class ProjectReqConfiguration : IEntityTypeConfiguration<ProjectReq>
{
    public void Configure(EntityTypeBuilder<ProjectReq> builder)
    {
        _ = builder.ToTable(nameof(IDatabaseService.ProjectReqs));

        builder.ConfigureModifiableProperties();

        _ = builder.Property(entity => entity.Name)
            .HasColumnType(ColumnTypeFor.Nvarchar(ProjectReqsMaximumLengthFor.Name));

        _ = builder.HasOne(x => x.Project)
            .WithMany(x => x.ProjectReqs)
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        _ = builder.HasOne(x => x.ProjectStage)
            .WithMany(x => x.ProjectReqs)
            .HasForeignKey(x => x.ProjectStageId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
