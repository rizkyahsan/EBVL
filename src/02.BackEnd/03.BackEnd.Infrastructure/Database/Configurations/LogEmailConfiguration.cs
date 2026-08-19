namespace EBVL.BackEnd.Infrastructure.Database.Configurations;

public sealed class LogEmailConfiguration : IEntityTypeConfiguration<LogEmail>
{
    public void Configure(EntityTypeBuilder<LogEmail> builder)
    {
        _ = builder.ToTable(nameof(IDatabaseService.LogEmails));

        builder.ConfigureModifiableProperties();

        _ = builder.Property(entity => entity.Module)
            .HasColumnType(ColumnTypeFor.Nvarchar(LogEmailsMaximumLengthFor.Module));

        _ = builder.Property(entity => entity.Action)
            .HasColumnType(ColumnTypeFor.Nvarchar(LogEmailsMaximumLengthFor.Action));

        _ = builder.Property(entity => entity.Provider)
            .HasColumnType(ColumnTypeFor.Nvarchar(LogEmailsMaximumLengthFor.Provider));
    }
}
