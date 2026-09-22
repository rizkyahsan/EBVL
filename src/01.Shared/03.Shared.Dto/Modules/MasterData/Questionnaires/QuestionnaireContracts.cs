using EBVL.Shared.Enums;

namespace EBVL.Shared.Dto.Modules.MasterData.Questionnaires;

public static class RouteConfig { public const string Tag = "Master Data Questionnaires"; public const string BasePath = "/api/master-data/questionnaires"; }
public static class QuestionnairePermissions
{
    public const string View = "ebvl.questionnaire.view";
    public const string Manage = "ebvl.questionnaire.manage";
    public const string Read = View;
    public const string Write = Manage;
    public static readonly string[] All = [View, Manage];
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

public sealed record QuestionnaireListItem(Guid QuestionnaireId, Guid QuestionnaireSeriesId, string Code, string BusinessProcess, int Version, QuestionnaireStatus Status, DateTimeOffset ModifiedAt, int SectionCount, bool HasChanges, bool IsValidForPublish, bool CanPublish, IReadOnlyList<string> PublishValidationErrors);
public sealed record GetQuestionnairesResponse : ListResponse<QuestionnaireListItem>;
public sealed record GetQuestionnaireResponse { public required QuestionnaireItem Item { get; init; } }
public sealed record QuestionnaireItem(Guid QuestionnaireId, Guid QuestionnaireSeriesId, string Code, string BusinessProcess, int Version, QuestionnaireStatus Status, Guid? PreviousVersionId, DateTimeOffset? PublishedAt, string? PublishedBy, bool IsActive, string RowVersion, IReadOnlyList<QuestionnaireSectionItem> Sections, IReadOnlyList<QuestionnaireRuleItem> Rules, bool HasChanges, bool IsValidForPublish, bool CanPublish, IReadOnlyList<string> PublishValidationErrors);
public sealed record QuestionnaireVersionItem(Guid QuestionnaireId, int Version, QuestionnaireStatus Status, DateTimeOffset? PublishedAt, string? PublishedBy, DateTimeOffset CreatedAt);
public sealed record GetQuestionnaireVersionHistoryResponse : ListResponse<QuestionnaireVersionItem>;
public record GetQuestionnaireSectionsRequest : PaginatedListRequest;
public sealed record GetQuestionnaireSectionsResponse : PaginatedListResponse<QuestionnaireSectionListItem>;
public sealed record QuestionnaireSectionListItem(Guid Id, string Code, string Title, string BusinessProcess, VendorCompanyStatusType? VendorType, int Order, bool IsActive, int QuestionCount);
public record GetQuestionnaireQuestionsRequest : PaginatedListRequest;
public sealed record GetQuestionnaireQuestionsResponse : PaginatedListResponse<QuestionnaireQuestionItem>;
public sealed record GetQuestionnaireQuestionResponse { public required QuestionnaireQuestionItem Item { get; init; } }
public sealed record QuestionnaireSectionItem(Guid Id, string Code, string Title, int Order, VendorCompanyStatusType? CompanyType, bool IsActive, IReadOnlyList<QuestionnaireQuestionItem> Questions);
public sealed record QuestionnaireQuestionItem(Guid Id, string Code, string Label, string? Hint, string? Placeholder, QuestionnaireQuestionType Type, VendorCompanyStatusType? CompanyType, int Order, bool IsRequired, bool IsVisible, bool IsActive, QuestionnaireAnswerRule AnswerRule, IReadOnlyList<QuestionnaireOptionItem> Options);
public sealed record AddQuestionnaireQuestionRequest
{
    public required string Code { get; init; }
    public required string Label { get; init; }
    public string? Hint { get; init; }
    public string? Placeholder { get; init; }
    public QuestionnaireQuestionType Type { get; init; }
    public VendorCompanyStatusType? VendorType { get; init; }
    public QuestionnaireAnswerRule AnswerRule { get; init; }
    public int Order { get; init; }
    public bool IsVisible { get; init; } = true;
    public bool IsActive { get; init; } = true;
    public required List<QuestionnaireOptionRequest> Options { get; init; }
    public string RowVersion { get; init; } = string.Empty;
}
public sealed record QuestionnaireOptionItem(Guid Id, string Code, string Label, int Order);
public sealed record QuestionnaireRuleItem(Guid Id, Guid SourceQuestionId, Guid TargetQuestionId, QuestionnaireRuleOperator Operator, QuestionnaireRuleAction Action, string Value);
public record AddQuestionnaireRequest { public required string Code { get; init; } public required string BusinessProcess { get; init; } public bool IsActive { get; init; } = true; }
public sealed record QuestionnaireOptionRequest(Guid? Id, string Code, string Label, int Order);
public sealed record AddQuestionnaireSectionRequest { public required string BusinessProcess { get; init; } public VendorCompanyStatusType? VendorType { get; init; } public required string Code { get; init; } public required string Title { get; init; } public int Order { get; init; } public bool IsActive { get; init; } = true; public required string RowVersion { get; init; } }
public sealed record UpdateQuestionnaireSectionRequest { public required string BusinessProcess { get; init; } public VendorCompanyStatusType? VendorType { get; init; } public string? Code { get; init; } public required string Section { get; init; } public int? Order { get; init; } public bool IsActive { get; init; } = true; public string RowVersion { get; init; } = string.Empty; }
public sealed record UpdateQuestionnaireQuestionRequest { public required string Code { get; init; } public required string Label { get; init; } public string? Hint { get; init; } public string? Placeholder { get; init; } public QuestionnaireQuestionType Type { get; init; } public VendorCompanyStatusType? VendorType { get; init; } public int Order { get; init; } public QuestionnaireAnswerRule AnswerRule { get; init; } public bool IsVisible { get; init; } = true; public bool IsActive { get; init; } = true; public required List<QuestionnaireOptionRequest> Options { get; init; } public required string RowVersion { get; init; } }
public sealed record DeleteQuestionnaireChildRequest { public required string RowVersion { get; init; } }
public sealed record PublishQuestionnaireRequest { public required string RowVersion { get; init; } }
public sealed record UpdateQuestionnaireRequest { public required string BusinessProcess { get; init; } public bool IsActive { get; init; } public required string RowVersion { get; init; } public required List<QuestionnaireSectionItem> Sections { get; init; } public required List<QuestionnaireRuleItem> Rules { get; init; } }

public static class QuestionnaireRoutes
{
    public const string List = RouteConfig.BasePath;
    public const string Add = RouteConfig.BasePath;
    public const string Detail = RouteConfig.BasePath + "/{questionnaireId:guid}";
    public const string Update = Detail;
    public const string AddQuestion = Detail + "/sections/{sectionId:guid}/questions";
    public const string Sections = Detail + "/sections";
    public const string UpdateSection = Detail + "/sections/{sectionId:guid}";
    public const string Questions = UpdateSection + "/questions";
    public const string QuestionDetail = Questions + "/{questionId:guid}";
    public const string Draft = Detail + "/draft";
    public const string Publish = Detail + "/publish";
    public const string History = RouteConfig.BasePath + "/series/{seriesId:guid}/versions";
}

public sealed class AddQuestionnaireRequestValidator : AbstractValidator<AddQuestionnaireRequest>
{
    public AddQuestionnaireRequestValidator()
    {
        _ = RuleFor(x => x.BusinessProcess).NotEmpty().Must(QuestionnaireBusinessProcessFor.All.Contains).WithMessage("The business process is invalid.");
        _ = RuleFor(x => x.Code).NotEmpty().MaximumLength(QuestionnaireMaximumLengthFor.Code);
    }
}
public sealed class AddQuestionnaireQuestionRequestValidator : AbstractValidator<AddQuestionnaireQuestionRequest>
{
    public AddQuestionnaireQuestionRequestValidator()
    {
        _ = RuleFor(x => x.Code).NotEmpty().MaximumLength(QuestionnaireMaximumLengthFor.Code);
        _ = RuleFor(x => x.Label).NotEmpty().MaximumLength(QuestionnaireMaximumLengthFor.Label);
        _ = RuleFor(x => x.Hint).MaximumLength(QuestionnaireMaximumLengthFor.HelpText);
        _ = RuleFor(x => x.Placeholder).MaximumLength(QuestionnaireMaximumLengthFor.HelpText);
        _ = RuleFor(x => x.Type).IsInEnum();
        _ = RuleFor(x => x.VendorType).IsInEnum().When(x => x.VendorType.HasValue);
        _ = RuleFor(x => x.AnswerRule).IsInEnum();
        _ = RuleFor(x => x.Order).GreaterThanOrEqualTo(1);
        _ = RuleForEach(x => x.Options).SetValidator(new QuestionnaireOptionRequestValidator());
        _ = RuleFor(x => x.Options).NotEmpty().When(x => x.Type is QuestionnaireQuestionType.SingleChoice or QuestionnaireQuestionType.MultipleChoice);
        _ = RuleFor(x => x.Options).Empty().When(x => x.Type is not (QuestionnaireQuestionType.SingleChoice or QuestionnaireQuestionType.MultipleChoice));
        _ = RuleFor(x => x.RowVersion).NotEmpty();
    }
}
public sealed class QuestionnaireOptionRequestValidator : AbstractValidator<QuestionnaireOptionRequest>
{
    public QuestionnaireOptionRequestValidator()
    {
        _ = RuleFor(x => x.Code).NotEmpty().MaximumLength(QuestionnaireMaximumLengthFor.Code);
        _ = RuleFor(x => x.Label).NotEmpty().MaximumLength(QuestionnaireMaximumLengthFor.Title);
        _ = RuleFor(x => x.Order).GreaterThanOrEqualTo(1);
    }
}
public sealed class AddQuestionnaireSectionRequestValidator : AbstractValidator<AddQuestionnaireSectionRequest>
{
    public AddQuestionnaireSectionRequestValidator()
    {
        _ = RuleFor(x => x.BusinessProcess).NotEmpty().Must(QuestionnaireBusinessProcessFor.All.Contains).WithMessage("The business process is invalid.");
        _ = RuleFor(x => x.VendorType).IsInEnum().When(x => x.VendorType.HasValue);
        _ = RuleFor(x => x.Code).NotEmpty().MaximumLength(QuestionnaireMaximumLengthFor.Code);
        _ = RuleFor(x => x.Title).NotEmpty().MaximumLength(QuestionnaireMaximumLengthFor.Title);
        _ = RuleFor(x => x.Order).GreaterThanOrEqualTo(1);
        _ = RuleFor(x => x.RowVersion).NotEmpty();
    }
}
public sealed class UpdateQuestionnaireSectionRequestValidator : AbstractValidator<UpdateQuestionnaireSectionRequest>
{
    public UpdateQuestionnaireSectionRequestValidator()
    {
        _ = RuleFor(x => x.BusinessProcess).NotEmpty().Must(QuestionnaireBusinessProcessFor.All.Contains).WithMessage("The business process is invalid.");
        _ = RuleFor(x => x.VendorType).IsInEnum().When(x => x.VendorType.HasValue);
        _ = RuleFor(x => x.Code).NotEmpty().MaximumLength(QuestionnaireMaximumLengthFor.Code).When(x => x.Code is not null);
        _ = RuleFor(x => x.Section).NotEmpty().MaximumLength(QuestionnaireMaximumLengthFor.Title);
        _ = RuleFor(x => x.Order).GreaterThanOrEqualTo(1).When(x => x.Order.HasValue);
        _ = RuleFor(x => x.RowVersion).NotEmpty();
    }
}
public sealed class UpdateQuestionnaireQuestionRequestValidator : AbstractValidator<UpdateQuestionnaireQuestionRequest>
{
    public UpdateQuestionnaireQuestionRequestValidator()
    {
        _ = RuleFor(x => x.Code).NotEmpty().MaximumLength(QuestionnaireMaximumLengthFor.Code);
        _ = RuleFor(x => x.Label).NotEmpty().MaximumLength(QuestionnaireMaximumLengthFor.Label);
        _ = RuleFor(x => x.Hint).MaximumLength(QuestionnaireMaximumLengthFor.HelpText);
        _ = RuleFor(x => x.Placeholder).MaximumLength(QuestionnaireMaximumLengthFor.HelpText);
        _ = RuleFor(x => x.Type).IsInEnum();
        _ = RuleFor(x => x.VendorType).IsInEnum().When(x => x.VendorType.HasValue);
        _ = RuleFor(x => x.AnswerRule).IsInEnum();
        _ = RuleFor(x => x.Order).GreaterThanOrEqualTo(1);
        _ = RuleForEach(x => x.Options).SetValidator(new QuestionnaireOptionRequestValidator());
        _ = RuleFor(x => x.Options).NotEmpty().When(x => x.Type is QuestionnaireQuestionType.SingleChoice or QuestionnaireQuestionType.MultipleChoice);
        _ = RuleFor(x => x.Options).Empty().When(x => x.Type is not (QuestionnaireQuestionType.SingleChoice or QuestionnaireQuestionType.MultipleChoice));
        _ = RuleFor(x => x.RowVersion).NotEmpty();
    }
}
public sealed class UpdateQuestionnaireRequestValidator : AbstractValidator<UpdateQuestionnaireRequest>
{
    public UpdateQuestionnaireRequestValidator()
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
