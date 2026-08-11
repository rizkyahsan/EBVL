namespace EBVL.BackEnd.Infrastructure.Database.Configurations;

public sealed class ConfigurationConfiguration : IEntityTypeConfiguration<Configuration>
{
    public void Configure(EntityTypeBuilder<Configuration> builder)
    {
        _ = builder.ToTable(nameof(IDatabaseService.Configurations));

        builder.ConfigureModifiableProperties();

        _ = builder.Property(entity => entity.Key)
            .HasColumnType(ColumnTypeFor.Nvarchar(ConfigurationsMaximumLengthFor.Key));

        _ = builder.Property(entity => entity.Value)
            .HasColumnType(ColumnTypeFor.Nvarchar(ConfigurationsMaximumLengthFor.Value));
    }
}
