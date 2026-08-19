namespace EBVL.BackEnd.Infrastructure.Database.Configurations;

public sealed class LenderConfiguration : IEntityTypeConfiguration<Lender>
{
    public void Configure(EntityTypeBuilder<Lender> builder)
    {
        _ = builder.ToTable(nameof(IDatabaseService.Lenders));

        builder.ConfigureModifiableProperties();

        _ = builder.Property(entity => entity.Name)
            .HasColumnType(ColumnTypeFor.Nvarchar(LendersMaximumLengthFor.Name));

        _ = builder.Property(entity => entity.PhoneNumber)
            .HasColumnType(ColumnTypeFor.Nvarchar(CommonMaximumLengthFor.PhoneNumber));

        _ = builder.Property(entity => entity.EmailAddress)
            .HasColumnType(ColumnTypeFor.Nvarchar(CommonMaximumLengthFor.EmailAddress));

        _ = builder.Property(entity => entity.Website)
            .HasColumnType(ColumnTypeFor.Nvarchar(CommonMaximumLengthFor.Url));

        _ = builder.HasOne(x => x.Country)
            .WithMany(x => x.Lenders)
            .HasForeignKey(x => x.CountryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
