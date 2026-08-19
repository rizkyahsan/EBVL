namespace EBVL.BackEnd.Infrastructure.Database.Configurations;

public sealed class ProjectFileConfiguration : IEntityTypeConfiguration<ProjectFile>
{
    public void Configure(EntityTypeBuilder<ProjectFile> builder)
    {
        _ = builder.ToTable(nameof(IDatabaseService.ProjectFiles));

        builder.ConfigureModifiableProperties();

        _ = builder.HasOne(x => x.Project)
            .WithMany(x => x.ProjectFiles)
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
