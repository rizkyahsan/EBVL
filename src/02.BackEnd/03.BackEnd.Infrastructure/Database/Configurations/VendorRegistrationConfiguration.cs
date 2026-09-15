namespace EBVL.BackEnd.Infrastructure.Database.Configurations;

public sealed class VendorRegistrationConfiguration : IEntityTypeConfiguration<VendorRegistration>
{
    public void Configure(EntityTypeBuilder<VendorRegistration> builder)
    {
        _ = builder.ToTable(nameof(IDatabaseService.VendorRegistrations));
        builder.ConfigureModifiableProperties();
        _ = builder.Property(x => x.CompanyType).HasConversion<string>().HasMaxLength(40);
        _ = builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(30);
        _ = builder.Property(x => x.AdditionalBrandsJson).HasColumnType("nvarchar(max)");
        _ = builder.Property(x => x.RowVersion).IsRowVersion();
        _ = builder.HasMany(x => x.Documents).WithOne(x => x.VendorRegistration).HasForeignKey(x => x.VendorRegistrationId).OnDelete(DeleteBehavior.Cascade);
        _ = builder.HasMany(x => x.Answers).WithOne(x => x.VendorRegistration).HasForeignKey(x => x.VendorRegistrationId).OnDelete(DeleteBehavior.Cascade);
    }
}
public sealed class VendorRegistrationDocumentConfiguration : IEntityTypeConfiguration<VendorRegistrationDocument>
{
    public void Configure(EntityTypeBuilder<VendorRegistrationDocument> builder)
    {
        _ = builder.ToTable(nameof(IDatabaseService.VendorRegistrationDocuments));
        builder.ConfigureModifiableProperties();
        _ = builder.Property(x => x.DefinitionKey).HasMaxLength(100);
        _ = builder.Property(x => x.Name).HasMaxLength(200);
        _ = builder.Property(x => x.OriginalFileName).HasMaxLength(260);
        _ = builder.Property(x => x.ContentType).HasMaxLength(100);
        _ = builder.HasOne(x => x.DocumentDefinition).WithMany().HasForeignKey(x => x.DocumentDefinitionId).OnDelete(DeleteBehavior.Restrict);
        _ = builder.HasOne(x => x.FileStorage).WithMany().HasForeignKey(x => x.FileStorageId).OnDelete(DeleteBehavior.Restrict);
        _ = builder.HasIndex(x => new { x.VendorRegistrationId, x.DefinitionKey }).IsUnique().HasFilter("[IsDeleted] = 0");
    }
}
public sealed class QuestionnaireAnswerConfiguration : IEntityTypeConfiguration<QuestionnaireAnswer>
{
    public void Configure(EntityTypeBuilder<QuestionnaireAnswer> builder)
    {
        _ = builder.ToTable(nameof(IDatabaseService.QuestionnaireAnswers));
        builder.ConfigureModifiableProperties();
        _ = builder.Property(x => x.JsonValue).HasColumnType("nvarchar(max)");
        _ = builder.Property(x => x.DecimalValue).HasPrecision(38, 10);
        _ = builder.HasIndex(x => new { x.VendorRegistrationId, x.QuestionnaireQuestionId }).IsUnique();
        _ = builder.HasOne(x => x.QuestionnaireQuestion).WithMany().HasForeignKey(x => x.QuestionnaireQuestionId).OnDelete(DeleteBehavior.Restrict);
    }
}
