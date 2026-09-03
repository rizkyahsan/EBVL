namespace EBVL.BackEnd.Infrastructure.Database.Configurations;

public sealed class VendorRegistrationConfiguration : IEntityTypeConfiguration<VendorRegistration>
{
    public void Configure(EntityTypeBuilder<VendorRegistration> builder)
    {
        _ = builder.ToTable(nameof(IDatabaseService.VendorRegistrations));
        builder.ConfigureModifiableProperties();
        _ = builder.Property(x => x.CompanyType).HasConversion<string>().HasMaxLength(40);
        _ = builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(30);
        _ = builder.Property(x => x.SapVendorNumber).HasMaxLength(50);
        _ = builder.Property(x => x.NormalizedSapVendorNumber).HasMaxLength(50);
        _ = builder.Property(x => x.CompanyName).HasMaxLength(200);
        _ = builder.Property(x => x.CompanyEmail).HasMaxLength(320);
        _ = builder.Property(x => x.PicEmail).HasMaxLength(320);
        _ = builder.Property(x => x.CompanyPhoneNumber).HasMaxLength(50);
        _ = builder.Property(x => x.PicPhoneNumber).HasMaxLength(50);
        _ = builder.Property(x => x.Website).HasMaxLength(2000);
        _ = builder.Property(x => x.CompanyService).HasConversion<string>().HasMaxLength(40);
        _ = builder.Property(x => x.FactoryCountry).HasMaxLength(200);
        _ = builder.Property(x => x.FactoryAddress).HasMaxLength(1000);
        _ = builder.Property(x => x.BrandRepresentative).HasMaxLength(200);
        _ = builder.Property(x => x.AdditionalBrandsJson).HasColumnType("nvarchar(max)");
        _ = builder.Property(x => x.RepresentativeName).HasMaxLength(200);
        _ = builder.Property(x => x.ResumeTokenHash).HasMaxLength(32);
        _ = builder.Property(x => x.RowVersion).IsRowVersion();
        _ = builder.HasIndex(x => x.UserId).IsUnique().HasFilter("[UserId] IS NOT NULL AND [IsDeleted] = 0");
        _ = builder.HasIndex(x => x.NormalizedSapVendorNumber).IsUnique().HasFilter("[NormalizedSapVendorNumber] IS NOT NULL AND [IsDeleted] = 0");
        _ = builder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
        _ = builder.HasOne(x => x.Submission).WithOne(x => x.VendorRegistration).HasForeignKey<QuestionnaireSubmission>(x => x.VendorRegistrationId).OnDelete(DeleteBehavior.Cascade);
        _ = builder.HasMany(x => x.Documents).WithOne(x => x.VendorRegistration).HasForeignKey(x => x.VendorRegistrationId).OnDelete(DeleteBehavior.Cascade);
    }
}
public sealed class VendorRegistrationDocumentConfiguration : IEntityTypeConfiguration<VendorRegistrationDocument>
{
    public void Configure(EntityTypeBuilder<VendorRegistrationDocument> builder)
    {
        _ = builder.ToTable(nameof(IDatabaseService.VendorRegistrationDocuments));
        builder.ConfigureModifiableProperties();
        _ = builder.Property(x => x.DefinitionKey).HasMaxLength(100);
        _ = builder.Property(x => x.OriginalFileName).HasMaxLength(260);
        _ = builder.Property(x => x.ContentType).HasMaxLength(100);
        _ = builder.HasIndex(x => new { x.VendorRegistrationId, x.DefinitionKey }).IsUnique().HasFilter("[IsDeleted] = 0");
        _ = builder.HasIndex(x => x.FileStorageId).IsUnique();
        _ = builder.HasOne(x => x.FileStorage).WithMany().HasForeignKey(x => x.FileStorageId).OnDelete(DeleteBehavior.Restrict);
    }
}
public sealed class QuestionnaireSubmissionConfiguration : IEntityTypeConfiguration<QuestionnaireSubmission>
{
    public void Configure(EntityTypeBuilder<QuestionnaireSubmission> builder)
    {
        _ = builder.ToTable(nameof(IDatabaseService.QuestionnaireSubmissions));
        builder.ConfigureModifiableProperties();
        _ = builder.HasIndex(x => x.VendorRegistrationId).IsUnique();
        _ = builder.HasOne(x => x.QuestionnaireVersion).WithMany().HasForeignKey(x => x.QuestionnaireVersionId).OnDelete(DeleteBehavior.Restrict);
    }
}
public sealed class QuestionnaireAnswerConfiguration : IEntityTypeConfiguration<QuestionnaireAnswer>
{
    public void Configure(EntityTypeBuilder<QuestionnaireAnswer> builder)
    {
        _ = builder.ToTable(nameof(IDatabaseService.QuestionnaireAnswers));
        builder.ConfigureModifiableProperties();
        _ = builder.Property(x => x.TextValue).HasMaxLength(8000);
        _ = builder.Property(x => x.JsonValue).HasColumnType("nvarchar(max)");
        _ = builder.Property(x => x.DecimalValue).HasPrecision(38, 10);
        _ = builder.HasIndex(x => new { x.QuestionnaireSubmissionId, x.QuestionnaireQuestionId }).IsUnique();
        _ = builder.HasOne(x => x.QuestionnaireQuestion).WithMany().HasForeignKey(x => x.QuestionnaireQuestionId).OnDelete(DeleteBehavior.Restrict);
    }
}
public sealed class QuestionnaireAnswerOptionConfiguration : IEntityTypeConfiguration<QuestionnaireAnswerOption>
{
    public void Configure(EntityTypeBuilder<QuestionnaireAnswerOption> builder)
    {
        _ = builder.ToTable(nameof(IDatabaseService.QuestionnaireAnswerOptions));
        builder.ConfigureModifiableProperties();
        _ = builder.HasIndex(x => new { x.QuestionnaireAnswerId, x.QuestionnaireOptionId }).IsUnique();
        _ = builder.HasOne(x => x.QuestionnaireOption).WithMany().HasForeignKey(x => x.QuestionnaireOptionId).OnDelete(DeleteBehavior.Restrict);
    }
}
public sealed class QuestionnaireAnswerFileConfiguration : IEntityTypeConfiguration<QuestionnaireAnswerFile>
{
    public void Configure(EntityTypeBuilder<QuestionnaireAnswerFile> builder)
    {
        _ = builder.ToTable(nameof(IDatabaseService.QuestionnaireAnswerFiles));
        builder.ConfigureModifiableProperties();
        _ = builder.Property(x => x.OriginalFileName).HasMaxLength(260);
        _ = builder.Property(x => x.ContentType).HasMaxLength(100);
        _ = builder.HasIndex(x => x.FileStorageId).IsUnique();
        _ = builder.HasOne(x => x.FileStorage).WithMany().HasForeignKey(x => x.FileStorageId).OnDelete(DeleteBehavior.Restrict);
    }
}
