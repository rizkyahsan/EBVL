using EBVL.Shared.Dto.Modules.MasterData.Questionnaires;
using EBVL.Shared.Enums;

namespace EBVL.Shared.Dto.Modules.MasterData.Documents;

public static class RouteConfig { public const string Tag = "Master Data Documents"; public const string BasePath = "/api/master-data/documents"; }
public static class DocumentPermissions { public const string Read = "ebvl.md.documents.read"; public const string Write = "ebvl.md.documents.write"; public static readonly string[] All = [Read, Write]; }

public sealed record DocumentRequirementSetListItem(Guid Id, Guid SeriesId, string BusinessProcess, int Version, QuestionnaireStatus Status, DateTimeOffset ModifiedAt, int RequirementCount, string RowVersion);
public sealed record GetDocumentsResponse : ListResponse<DocumentRequirementSetListItem>;
public sealed record DocumentListItem(Guid Id, Guid DocumentRequirementSetId, string BusinessProcess, string Name, int Order, int MaxSizeMb, bool IsMandatory, bool IsActive, string RowVersion);
public sealed record DocumentRequirementSetItem(Guid Id, Guid SeriesId, string BusinessProcess, int Version, QuestionnaireStatus Status, Guid? PreviousVersionId, DateTimeOffset? PublishedAt, string? PublishedBy, string RowVersion, IReadOnlyList<DocumentListItem> Requirements);
public sealed record GetDocumentResponse { public required DocumentRequirementSetItem Item { get; init; } }
public sealed record DocumentRequirementSetVersionItem(Guid Id, int Version, QuestionnaireStatus Status, DateTimeOffset? PublishedAt, string? PublishedBy, DateTimeOffset CreatedAt);
public sealed record GetDocumentHistoryResponse : ListResponse<DocumentRequirementSetVersionItem>;

public record AddDocumentRequest { public required string BusinessProcess { get; init; } public required string Name { get; init; } public int Order { get; init; } public int MaxSizeMb { get; init; } public bool IsMandatory { get; init; } public bool IsActive { get; init; } = true; public string RowVersion { get; init; } = string.Empty; }
public sealed record UpdateDocumentRequest : AddDocumentRequest;
public sealed record PublishDocumentRequirementSetRequest { public required string RowVersion { get; init; } }

public static class DocumentRoutes
{
    public const string List = RouteConfig.BasePath;
    public const string Add = Detail + "/requirements";
    public const string Detail = RouteConfig.BasePath + "/{documentRequirementSetId:guid}";
    public const string Requirement = Detail + "/requirements/{documentId:guid}";
    public const string Draft = Detail + "/draft";
    public const string Publish = Detail + "/publish";
    public const string History = RouteConfig.BasePath + "/series/{seriesId:guid}/versions";
}

public sealed class AddDocumentRequestValidator : AbstractValidator<AddDocumentRequest>
{
    public AddDocumentRequestValidator()
    {
        _ = RuleFor(x => x.BusinessProcess).NotEmpty().Must(QuestionnaireBusinessProcessFor.All.Contains).WithMessage("The business process is invalid.");
        _ = RuleFor(x => x.Name).Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("The document type name is required.").MaximumLength(200);
        _ = RuleFor(x => x.MaxSizeMb).InclusiveBetween(1, 100);
        _ = RuleFor(x => x.Order).GreaterThanOrEqualTo(1);
        _ = RuleFor(x => x.RowVersion).NotEmpty();
    }
}
public sealed class UpdateDocumentRequestValidator : AbstractValidator<UpdateDocumentRequest> { public UpdateDocumentRequestValidator() { Include(new AddDocumentRequestValidator()); } }
