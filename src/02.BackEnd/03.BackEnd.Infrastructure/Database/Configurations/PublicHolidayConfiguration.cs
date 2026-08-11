namespace EBVL.BackEnd.Infrastructure.Database.Configurations;

public sealed class PublicHolidayConfiguration : IEntityTypeConfiguration<PublicHoliday>
{
    public void Configure(EntityTypeBuilder<PublicHoliday> builder)
    {
        _ = builder.ToTable(nameof(IDatabaseService.PublicHolidays));

        builder.ConfigureModifiableProperties();

        _ = builder.Property(entity => entity.CountryCode)
            .HasColumnType(ColumnTypeFor.Nvarchar(PublicHolidaysMaximumLengthFor.CountryCode));

        _ = builder.Property(entity => entity.Name)
            .HasColumnType(ColumnTypeFor.Nvarchar(PublicHolidaysMaximumLengthFor.Name));

        _ = builder.Property(entity => entity.LocalName)
            .HasColumnType(ColumnTypeFor.Nvarchar(PublicHolidaysMaximumLengthFor.LocalName));
    }
}
