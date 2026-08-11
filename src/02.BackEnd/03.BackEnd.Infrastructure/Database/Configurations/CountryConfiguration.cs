namespace EBVL.BackEnd.Infrastructure.Database.Configurations;

public sealed class CountryConfiguration : IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        _ = builder.ToTable(nameof(IDatabaseService.Countries));

        builder.ConfigureModifiableProperties();

        _ = builder.Property(entity => entity.Name)
            .HasColumnType(ColumnTypeFor.Nvarchar(CountriesMaximumLengthFor.Name));

        _ = builder.Property(entity => entity.Code)
            .HasColumnType(ColumnTypeFor.Nvarchar(CountriesMaximumLengthFor.Code));
    }
}
