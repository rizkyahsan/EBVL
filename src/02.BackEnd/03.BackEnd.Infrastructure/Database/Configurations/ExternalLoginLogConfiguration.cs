namespace EBVL.BackEnd.Infrastructure.Database.Configurations;

public sealed class ExternalLoginLogConfiguration : IEntityTypeConfiguration<ExternalLoginLog>
{
    public void Configure(EntityTypeBuilder<ExternalLoginLog> builder)
    {
        _ = builder.ToTable(nameof(IDatabaseService.ExternalLoginLogs));

        builder.ConfigureModifiableProperties();

        _ = builder.Property(entity => entity.Username)
            .HasColumnType(ColumnTypeFor.Nvarchar(CommonMaximumLengthFor.EmailAddress));

        _ = builder.Property(entity => entity.IpAddress)
            .HasColumnType(ColumnTypeFor.Nvarchar(ExternalLoginLogsMaximumLengthFor.IpAddress));

        _ = builder.HasOne(x => x.ExternalLogin)
            .WithOne(x => x.ExternalLoginLog)
            .HasForeignKey<ExternalLogin>(x => x.ExternalLoginLogId);
    }
}
