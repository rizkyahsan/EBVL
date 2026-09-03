namespace EBVL.BackEnd.Domain.Entities;

public sealed class VendorRegistration : ModifiableEntity
{
    public Guid? UserId { get; set; }
    public VendorCompanyStatusType CompanyType { get; set; }
    public string? SapVendorNumber { get; set; }
    public string? NormalizedSapVendorNumber { get; set; }
    public required string CompanyName { get; set; }
    public required string CompanyEmail { get; set; }
    public required string PicEmail { get; set; }
    public required string CompanyPhoneNumber { get; set; }
    public required string PicPhoneNumber { get; set; }
    public required string Website { get; set; }
    public VendorServiceType CompanyService { get; set; }
    public required string FactoryCountry { get; set; }
    public required string FactoryAddress { get; set; }
    public required string BrandRepresentative { get; set; }
    public required string AdditionalBrandsJson { get; set; }
    public bool IsRepresentativeInIndonesia { get; set; }
    public required string RepresentativeName { get; set; }
    [ExcludeFromAudit] public required byte[] ResumeTokenHash { get; set; }
    public DateTimeOffset? ResumeTokenExpiresAt { get; set; }
    public VendorRegistrationStatus Status { get; set; }
    public DateTimeOffset? SubmittedAt { get; set; }
    public byte[] RowVersion { get; set; } = [];
    public User? User { get; set; }
    public QuestionnaireSubmission Submission { get; set; } = default!;
    public ICollection<VendorRegistrationDocument> Documents { get; set; } = new HashSet<VendorRegistrationDocument>();
}

public sealed class VendorRegistrationDocument : ModifiableEntity
{
    public Guid VendorRegistrationId { get; set; }
    public Guid FileStorageId { get; set; }
    public required string DefinitionKey { get; set; }
    public required string OriginalFileName { get; set; }
    public required string ContentType { get; set; }
    public long Length { get; set; }
    public VendorRegistration VendorRegistration { get; set; } = default!;
    public FileStorage FileStorage { get; set; } = default!;
}

public sealed class QuestionnaireSubmission : ModifiableEntity
{
    public Guid VendorRegistrationId { get; set; }
    public Guid QuestionnaireVersionId { get; set; }
    public VendorRegistration VendorRegistration { get; set; } = default!;
    public QuestionnaireVersion QuestionnaireVersion { get; set; } = default!;
    public ICollection<QuestionnaireAnswer> Answers { get; set; } = new HashSet<QuestionnaireAnswer>();
}

public sealed class QuestionnaireAnswer : ModifiableEntity
{
    public Guid QuestionnaireSubmissionId { get; set; }
    public Guid QuestionnaireQuestionId { get; set; }
    public string? TextValue { get; set; }
    public long? IntegerValue { get; set; }
    public decimal? DecimalValue { get; set; }
    public DateOnly? DateValue { get; set; }
    public bool? BooleanValue { get; set; }
    public string? JsonValue { get; set; }
    public QuestionnaireSubmission QuestionnaireSubmission { get; set; } = default!;
    public QuestionnaireQuestion QuestionnaireQuestion { get; set; } = default!;
    public ICollection<QuestionnaireAnswerOption> SelectedOptions { get; set; } = new HashSet<QuestionnaireAnswerOption>();
    public ICollection<QuestionnaireAnswerFile> Files { get; set; } = new HashSet<QuestionnaireAnswerFile>();
}

public sealed class QuestionnaireAnswerOption : ModifiableEntity
{
    public Guid QuestionnaireAnswerId { get; set; }
    public Guid QuestionnaireOptionId { get; set; }
    public QuestionnaireAnswer QuestionnaireAnswer { get; set; } = default!;
    public QuestionnaireOption QuestionnaireOption { get; set; } = default!;
}

public sealed class QuestionnaireAnswerFile : ModifiableEntity
{
    public Guid VendorRegistrationId { get; set; }
    public Guid QuestionnaireAnswerId { get; set; }
    public Guid QuestionnaireQuestionId { get; set; }
    public Guid FileStorageId { get; set; }
    public required string OriginalFileName { get; set; }
    public required string ContentType { get; set; }
    public long Length { get; set; }
    public QuestionnaireAnswer QuestionnaireAnswer { get; set; } = default!;
    public FileStorage FileStorage { get; set; } = default!;
}
