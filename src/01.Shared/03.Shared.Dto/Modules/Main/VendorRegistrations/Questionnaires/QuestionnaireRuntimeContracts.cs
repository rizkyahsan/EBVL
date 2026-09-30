using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.PreRegistration;
using EBVL.Shared.Enums;

namespace EBVL.Shared.Dto.Modules.Main.VendorRegistrations.Questionnaires;

public sealed record SapAvailabilityRequest(string SapVendorNumber);
public sealed record SapAvailabilityResponse(bool IsAvailable, bool IsSapVerified, string SapVendorNumber, string? Message);
public sealed record RegistrationAuthorizationRequest(Guid RegistrationId, string ResumeToken);
public sealed record ResumeQuestionnaireRequest(Guid RegistrationId, string ResumeToken);
public sealed record QuestionnaireRuntimeResponse(Guid RegistrationId, string? ResumeToken, string RowVersion, VendorRegistrationStatus Status, PreRegistrationRequest Profile, IReadOnlyList<VendorRegistrationDocumentItem> Documents, bool IsDocumentEvidenceComplete, Guid QuestionnaireId, IReadOnlyList<RuntimeSectionItem> Sections);
public sealed record UpdateVendorRegistrationProfileRequest(string ResumeToken, string RowVersion, PreRegistrationRequest Profile);
public sealed record VendorRegistrationDocumentItem(Guid RequirementId, string DefinitionKey, string Name, int Order, bool IsMandatory, int MaxSizeMb, Guid? DocumentId, string? FileName, string? ContentType, long? Length);
public sealed record UploadVendorRegistrationDocumentResponse(VendorRegistrationDocumentItem Document, string RowVersion, bool IsDocumentEvidenceComplete);
public sealed record RuntimeSectionItem(Guid Id, string Code, string Title, int Order, IReadOnlyList<RuntimeQuestionItem> Questions);
public sealed record RuntimeQuestionItem(Guid Id, string Code, string Label, string? Hint, string? Placeholder, QuestionnaireQuestionType Type, int Order, bool IsRequired, IReadOnlyList<RuntimeOptionItem> Options, QuestionnaireAnswerValue? Answer, IReadOnlyList<RuntimeFileItem> Files);
public sealed record RuntimeOptionItem(Guid Id, string Code, string Label, int Order);
public sealed record RuntimeFileItem(Guid Id, string FileName, string ContentType, long Length);
public sealed record QuestionnaireAnswerValue(Guid QuestionId, string? TextValue, long? IntegerValue, decimal? DecimalValue, DateOnly? DateValue, bool? BooleanValue, string? AddressJson, IReadOnlyList<Guid>? OptionIds);
public sealed record SaveAnswersRequest(string ResumeToken, string RowVersion, IReadOnlyList<QuestionnaireAnswerValue> Answers);
public sealed record SubmitQuestionnaireRequest(string ResumeToken, string RowVersion);
public sealed record FileAuthorizationRequest(string ResumeToken);
public sealed record UploadQuestionnaireFileResponse(RuntimeFileItem File, string RowVersion);
