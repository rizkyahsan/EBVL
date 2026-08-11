namespace EBVL.BackEnd.Infrastructure.Database.Configurations;

public sealed class ApiCallConfiguration : IEntityTypeConfiguration<ApiCall>
{
    public void Configure(EntityTypeBuilder<ApiCall> builder)
    {
        _ = builder.ToTable(nameof(IDatabaseService.ApiCalls));

        builder.ConfigureCreatableProperties();

        _ = builder.Property(entity => entity.ServiceName)
            .HasColumnType(ColumnTypeFor.Nvarchar(ApiCallsMaximumLengthFor.ServiceName));

        _ = builder.Property(entity => entity.ServiceProvider)
            .HasColumnType(ColumnTypeFor.Nvarchar(ApiCallsMaximumLengthFor.ServiceProvider));

        _ = builder.Property(entity => entity.ServiceCategory)
            .HasColumnType(ColumnTypeFor.Nvarchar(ApiCallsMaximumLengthFor.ServiceCategory));

        _ = builder.Property(entity => entity.RequestUrl)
            .HasColumnType(ColumnTypeFor.Nvarchar(ApiCallsMaximumLengthFor.RequestUrl));

        _ = builder.Property(entity => entity.RequestMethod)
            .HasColumnType(ColumnTypeFor.Nvarchar(ApiCallsMaximumLengthFor.RequestMethod));

        _ = builder.Property(entity => entity.ErrorMessage)
            .HasColumnType(ColumnTypeFor.Nvarchar(ApiCallsMaximumLengthFor.ErrorMessage));
    }
}
