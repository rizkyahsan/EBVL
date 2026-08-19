namespace EBVL.BackEnd.Infrastructure.Database.Configurations;

public sealed class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        _ = builder.ToTable(nameof(IDatabaseService.Projects));

        builder.ConfigureModifiableProperties();

        _ = builder.Property(entity => entity.Title)
            .HasColumnType(ColumnTypeFor.Nvarchar(ProjectsMaximumLengthFor.Title));
    }
}
