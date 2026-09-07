using EBVL.Shared.Enums;

namespace EBVL.Shared.Dto.Modules.MasterData.Questionnaires;

public static class RouteConfig { public const string Tag = "Master Data Questionnaires"; public const string BasePath = "/api/master-data/questionnaires"; }
public static class QuestionnairePermissions
{
    public const string Read = "ebvl.md.questionnaires.read";
    public const string Write = "ebvl.md.questionnaires.write";
    // Assign only to MSAI administrators; authorization is permission-based by design.
    public const string Publish = "ebvl.md.questionnaires.publish";
    public static readonly string[] All = [Read, Write, Publish];
}
public static class QuestionnaireMaximumLengthFor { public const int Code = 50; public const int BusinessProcess = 100; public const int Title = 300; public const int Label = 1000; public const int HelpText = 1000; public const int RuleValue = 1000; public const long FileBytes = 50L * 1024 * 1024; public const string PdfContentType = "application/pdf"; }
public static class QuestionnaireBusinessProcessFor
{
    public const string VendorRegistration = "Vendor Registration";
    public const string BrandSubmissions = "Brand Submissions";
    public const string UpdateTechnicalBrand = "Update Technical Brand";
    public const string BrandClaim = "Brand Claim";
    public const string ReviewPerformance = "Review Performance";
    public static readonly string[] All = [VendorRegistration, BrandSubmissions, UpdateTechnicalBrand, BrandClaim, ReviewPerformance];
}

public sealed record QuestionnaireListItem(Guid Id, Guid VersionId, Guid SectionId, string Code, string BusinessProcess, VendorCompanyStatusType? VendorType, string Section, int Version, QuestionnaireVersionStatus Status, bool IsActive);
public sealed record GetQuestionnairesResponse : ListResponse<QuestionnaireListItem>;
public sealed record GetQuestionnaireResponse { public required QuestionnaireDraftItem Item { get; init; } }
public sealed record QuestionnaireDraftItem(Guid Id, Guid QuestionnaireId, string Code, string BusinessProcess, int Version, QuestionnaireVersionStatus Status, bool IsActive, string RowVersion, IReadOnlyList<QuestionnaireSectionItem> Sections, IReadOnlyList<QuestionnaireRuleItem> Rules);
public sealed record QuestionnaireSectionItem(Guid Id, string Code, string Title, int Order, VendorCompanyStatusType? CompanyType, bool IsActive, IReadOnlyList<QuestionnaireQuestionItem> Questions);
public sealed record QuestionnaireQuestionItem(Guid Id, string Code, string Label, string? Hint, string? Placeholder, QuestionnaireQuestionType Type, VendorCompanyStatusType? CompanyType, int Order, bool IsRequired, bool IsVisible, bool IsActive, QuestionnaireAnswerRule AnswerRule, IReadOnlyList<QuestionnaireOptionItem> Options);
public sealed record AddQuestionnaireQuestionRequest
{
    public required string Name { get; init; }
    public required string Code { get; init; }
    public string? Description { get; init; }
    public QuestionnaireQuestionType AnswerType { get; init; }
    public VendorCompanyStatusType VendorType { get; init; }
    public QuestionnaireAnswerRule AnswerRule { get; init; }
    public bool IsActive { get; init; } = true;
}
public sealed record QuestionnaireOptionItem(Guid Id, string Code, string Label, int Order);
public sealed record QuestionnaireRuleItem(Guid Id, Guid SourceQuestionId, Guid TargetQuestionId, QuestionnaireRuleOperator Operator, QuestionnaireRuleAction Action, string Value);

public record AddQuestionnaireRequest
{
    public required string BusinessProcess { get; init; }
    public VendorCompanyStatusType? VendorType { get; init; }
    public required string Section { get; init; }
    public bool IsActive { get; init; } = true;
}
public sealed record UpdateQuestionnaireDraftRequest { public required string BusinessProcess { get; init; } public required string RowVersion { get; init; } public required List<QuestionnaireSectionItem> Sections { get; init; } public required List<QuestionnaireRuleItem> Rules { get; init; } }
public sealed record QuestionnaireIdRequest { public Guid Id { get; init; } }
public sealed record ValidateQuestionnaireResponse { public required IReadOnlyList<string> Errors { get; init; } public bool IsValid => Errors.Count == 0; }

public static class QuestionnaireRoutes
{
    public const string List = RouteConfig.BasePath;
    public const string Add = RouteConfig.BasePath;
    public const string Detail = RouteConfig.BasePath + "/{id:guid}";
    public const string Update = RouteConfig.BasePath + "/{id:guid}/draft";
    public const string AddQuestion = RouteConfig.BasePath + "/{id:guid}/sections/{sectionId:guid}/questions";
    public const string Validate = RouteConfig.BasePath + "/{id:guid}/validate";
    public const string Publish = RouteConfig.BasePath + "/{id:guid}/publish";
    public const string Deactivate = RouteConfig.BasePath + "/{id:guid}/deactivate";
    public const string Archive = RouteConfig.BasePath + "/{id:guid}/archive";
}

public sealed class AddQuestionnaireRequestValidator : AbstractValidator<AddQuestionnaireRequest>
{
    public AddQuestionnaireRequestValidator()
    {
        _ = RuleFor(x => x.BusinessProcess).NotEmpty().Must(QuestionnaireBusinessProcessFor.All.Contains).WithMessage("The business process is invalid.");
        _ = RuleFor(x => x.Section).NotEmpty().MaximumLength(QuestionnaireMaximumLengthFor.Title);
    }
}
public sealed class UpdateQuestionnaireDraftRequestValidator : AbstractValidator<UpdateQuestionnaireDraftRequest>
{
    public UpdateQuestionnaireDraftRequestValidator()
    {
        _ = RuleFor(x => x.BusinessProcess).NotEmpty().MaximumLength(QuestionnaireMaximumLengthFor.BusinessProcess);
        _ = RuleFor(x => x.RowVersion).NotEmpty();
        _ = RuleForEach(x => x.Sections).ChildRules(s =>
        {
            _ = s.RuleFor(x => x.Code).NotEmpty().MaximumLength(QuestionnaireMaximumLengthFor.Code);
            _ = s.RuleFor(x => x.Title).NotEmpty().MaximumLength(QuestionnaireMaximumLengthFor.Title);
            _ = s.RuleForEach(x => x.Questions).ChildRules(q =>
            {
                _ = q.RuleFor(x => x.Code).NotEmpty().MaximumLength(QuestionnaireMaximumLengthFor.Code);
                _ = q.RuleFor(x => x.Label).NotEmpty().MaximumLength(QuestionnaireMaximumLengthFor.Label);
            });
        });
    }
}
