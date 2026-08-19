namespace EBVL.BackEnd.Infrastructure.Database.Configurations;

public sealed class StatusConfiguration : IEntityTypeConfiguration<Status>
{
    public void Configure(EntityTypeBuilder<Status> builder)
    {
        _ = builder.ToTable(nameof(IDatabaseService.Statuses));

        builder.ConfigureModifiableProperties();

        _ = builder.Property(entity => entity.Table)
            .HasColumnType(ColumnTypeFor.Nvarchar(StatusesMaximumLengthFor.Table));

        _ = builder.Property(entity => entity.Name)
            .HasColumnType(ColumnTypeFor.Nvarchar(StatusesMaximumLengthFor.Name));

        _ = builder.Property(entity => entity.Code)
            .HasColumnType(ColumnTypeFor.Nvarchar(StatusesMaximumLengthFor.Code));
    }
}
