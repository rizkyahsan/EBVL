namespace EBVL.BackEnd.Infrastructure.Database.Configurations;

public sealed class ProjectLenderReqConfiguration : IEntityTypeConfiguration<ProjectLenderReq>
{
    public void Configure(EntityTypeBuilder<ProjectLenderReq> builder)
    {
        _ = builder.ToTable(nameof(IDatabaseService.ProjectLenderReqs));

        builder.ConfigureModifiableProperties();

        _ = builder.HasOne(x => x.Project)
            .WithMany(x => x.ProjectLenderReqs)
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        _ = builder.HasOne(x => x.ProjectLender)
            .WithMany(x => x.ProjectLenderReqs)
            .HasForeignKey(x => x.ProjectLenderId)
            .OnDelete(DeleteBehavior.Restrict);

        _ = builder.HasOne(x => x.ProjectStage)
            .WithMany(x => x.ProjectLenderReqs)
            .HasForeignKey(x => x.ProjectStageId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}
