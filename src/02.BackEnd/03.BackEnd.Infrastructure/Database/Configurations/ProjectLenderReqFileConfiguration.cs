namespace EBVL.BackEnd.Infrastructure.Database.Configurations;

public sealed class ProjectLenderReqFileConfiguration : IEntityTypeConfiguration<ProjectLenderReqFile>
{
    public void Configure(EntityTypeBuilder<ProjectLenderReqFile> builder)
    {
        _ = builder.ToTable(nameof(IDatabaseService.ProjectLenderReqFiles));

        builder.ConfigureModifiableProperties();

        _ = builder.HasOne(x => x.Project)
            .WithMany(x => x.ProjectLenderReqFiles)
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        _ = builder.HasOne(x => x.ProjectReq)
            .WithMany(x => x.ProjectLenderReqFiles)
            .HasForeignKey(x => x.ProjectReqId)
            .OnDelete(DeleteBehavior.Restrict);

        _ = builder.HasOne(x => x.ProjectLenderReq)
            .WithMany(x => x.ProjectLenderReqFiles)
            .HasForeignKey(x => x.ProjectLenderReqId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
