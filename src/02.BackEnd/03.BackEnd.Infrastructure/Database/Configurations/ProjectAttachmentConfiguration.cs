namespace EBVL.BackEnd.Infrastructure.Database.Configurations;

public sealed class ProjectAttachmentConfiguration : IEntityTypeConfiguration<ProjectAttachment>
{
    public void Configure(EntityTypeBuilder<ProjectAttachment> builder)
    {
        _ = builder.ToTable(nameof(IDatabaseService.ProjectAttachments));

        builder.ConfigureModifiableProperties();

        _ = builder.Property(entity => entity.Name)
            .HasColumnType(ColumnTypeFor.Nvarchar(ProjectAttachmentsMaximumLengthFor.Name));

        _ = builder.HasOne(x => x.Project)
            .WithMany(x => x.ProjectAttachments)
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        _ = builder.HasOne(x => x.ProjectStage)
            .WithMany(x => x.ProjectAttachments)
            .HasForeignKey(x => x.ProjectStageId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
