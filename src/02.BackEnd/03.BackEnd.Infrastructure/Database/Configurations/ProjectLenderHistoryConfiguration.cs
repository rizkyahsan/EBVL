namespace EBVL.BackEnd.Infrastructure.Database.Configurations;

public sealed class ProjectLenderHistoryConfiguration : IEntityTypeConfiguration<ProjectLenderHistory>
{
    public void Configure(EntityTypeBuilder<ProjectLenderHistory> builder)
    {
        _ = builder.ToTable(nameof(IDatabaseService.ProjectLenderHistories));

        builder.ConfigureModifiableProperties();

        _ = builder.HasOne(x => x.Project)
            .WithMany(x => x.ProjectLenderHistories)
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        _ = builder.HasOne(x => x.ProjectLenderReq)
            .WithMany(x => x.ProjectLenderHistories)
            .HasForeignKey(x => x.ProjectLenderReqId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
