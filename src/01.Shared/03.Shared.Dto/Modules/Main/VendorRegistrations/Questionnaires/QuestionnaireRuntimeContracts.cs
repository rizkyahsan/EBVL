using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.PreRegistration;
using EBVL.Shared.Enums;

namespace EBVL.Shared.Dto.Modules.Main.VendorRegistrations.Questionnaires;

public static class RouteConfig { public const string Tag = "Vendor Registration Questionnaire"; public const string BasePath = "/api/main/vendor-registrations/questionnaires"; }
public static class QuestionnaireRuntimeRoutes
{
    public const string Start = RouteConfig.BasePath + "/start";
    public const string Get = RouteConfig.BasePath + "/{registrationId:guid}";
    public const string Answers = Get + "/answers";
    public const string Profile = Get + "/profile";
    public const string Documents = Get + "/documents/{definitionKey}";
    public const string Document = Get + "/documents/files/{documentId:guid}";
    public const string Files = Get + "/questions/{questionId:guid}/files";
    public const string File = Get + "/files/{fileId:guid}";
    public const string Submit = Get + "/submit";
}
public sealed record RegistrationAuthorizationRequest(Guid RegistrationId, string ResumeToken);
public sealed record ResumeQuestionnaireRequest(Guid RegistrationId, string ResumeToken);
public sealed record QuestionnaireRuntimeResponse(Guid RegistrationId, string? ResumeToken, string RowVersion, VendorRegistrationStatus Status, PreRegistrationRequest Profile, IReadOnlyList<VendorRegistrationDocumentItem> Documents, bool IsDocumentEvidenceComplete, Guid QuestionnaireVersionId, int QuestionnaireVersion, IReadOnlyList<RuntimeSectionItem> Sections);
public sealed record UpdateVendorRegistrationProfileRequest(string ResumeToken, string RowVersion, PreRegistrationRequest Profile);
public sealed record VendorRegistrationDocumentItem(Guid Id, string DefinitionKey, string FileName, string ContentType, long Length);
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
