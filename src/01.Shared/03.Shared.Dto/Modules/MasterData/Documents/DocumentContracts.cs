using EBVL.Shared.Dto.Modules.MasterData.Questionnaires;

namespace EBVL.Shared.Dto.Modules.MasterData.Documents;

public static class RouteConfig
{
    public const string Tag = "Master Data Documents";
    public const string BasePath = "/api/master-data/documents";
}

public static class DocumentPermissions
{
    public const string Read = "ebvl.md.documents.read";
    public const string Write = "ebvl.md.documents.write";
    public static readonly string[] All = [Read, Write];
}

public sealed record DocumentListItem(Guid Id, string BusinessProcess, string Name, int MaxSizeMb, bool IsMandatory, bool IsActive, string RowVersion);
public sealed record GetDocumentsResponse : ListResponse<DocumentListItem>;
public sealed record GetDocumentResponse
{
    public required DocumentListItem Item { get; init; }
}

public record AddDocumentRequest
{
    public required string BusinessProcess { get; init; }
    public required string Name { get; init; }
    public int MaxSizeMb { get; init; }
    public bool IsMandatory { get; init; }
    public bool IsActive { get; init; } = true;
}

public sealed record UpdateDocumentRequest : AddDocumentRequest
{
    public required string RowVersion { get; init; }
}

public static class DocumentRoutes
{
    public const string List = RouteConfig.BasePath;
    public const string Add = RouteConfig.BasePath;
    public const string Detail = RouteConfig.BasePath + "/{documentId:guid}";
    public const string Update = Detail;
}

public sealed class AddDocumentRequestValidator : AbstractValidator<AddDocumentRequest>
{
    public AddDocumentRequestValidator()
    {
        _ = RuleFor(x => x.BusinessProcess).NotEmpty().Must(QuestionnaireBusinessProcessFor.All.Contains).WithMessage("The business process is invalid.");
        _ = RuleFor(x => x.Name).Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("The document type name is required.").MaximumLength(200);
        _ = RuleFor(x => x.MaxSizeMb).InclusiveBetween(1, 100);
    }
}

public sealed class UpdateDocumentRequestValidator : AbstractValidator<UpdateDocumentRequest>
{
    public UpdateDocumentRequestValidator()
    {
        Include(new AddDocumentRequestValidator());
        _ = RuleFor(x => x.RowVersion).NotEmpty().Must(BeRowVersion).WithMessage("The row version is invalid.");
    }

    private static bool BeRowVersion(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        try
        {
            return Convert.FromBase64String(value).Length == 8;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
