namespace EBVL.BackEnd.Infrastructure.Database.Configurations;

public sealed class ProjectLenderConfiguration : IEntityTypeConfiguration<ProjectLender>
{
    public void Configure(EntityTypeBuilder<ProjectLender> builder)
    {
        _ = builder.ToTable(nameof(IDatabaseService.ProjectLenders));

        builder.ConfigureModifiableProperties();

        _ = builder.Property(entity => entity.Note)
            .HasColumnType(ColumnTypeFor.Nvarchar(ProjectLendersMaximumLengthFor.Note));

        _ = builder.HasOne(x => x.Project)
            .WithMany(x => x.ProjectLenders)
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        _ = builder.HasOne(x => x.Lender)
            .WithMany(x => x.ProjectLenders)
            .HasForeignKey(x => x.LenderId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
