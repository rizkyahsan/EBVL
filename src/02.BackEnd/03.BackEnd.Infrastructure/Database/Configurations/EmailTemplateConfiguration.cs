namespace EBVL.BackEnd.Infrastructure.Database.Configurations;

public sealed class EmailTemplateConfiguration : IEntityTypeConfiguration<EmailTemplate>
{
    public void Configure(EntityTypeBuilder<EmailTemplate> builder)
    {
        _ = builder.ToTable(nameof(IDatabaseService.EmailTemplates));

        builder.ConfigureModifiableProperties();

        _ = builder.Property(entity => entity.Module)
            .HasColumnType(ColumnTypeFor.Nvarchar(EmailTemplatesMaximumLengthFor.Module));

        _ = builder.Property(entity => entity.Action)
            .HasColumnType(ColumnTypeFor.Nvarchar(EmailTemplatesMaximumLengthFor.Action));

    }
}
