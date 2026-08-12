namespace EBVL.BackEnd.Infrastructure.Database.Configurations;

public sealed class ContactTypeConfiguration : IEntityTypeConfiguration<ContactType>
{
    public void Configure(EntityTypeBuilder<ContactType> builder)
    {
        _ = builder.ToTable(nameof(IDatabaseService.ContactTypes), DatabaseService.SchemaName);
        builder.ConfigureModifiableProperties();
        _ = builder.Property(x => x.Name).HasColumnType(ColumnTypeFor.Nvarchar(100));
        _ = builder.Property(x => x.Description).HasColumnType(ColumnTypeFor.Nvarchar(500));
    }
}
