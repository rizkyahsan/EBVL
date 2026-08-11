namespace EBVL.Shared.Dto.Modules.Examples.Documents.DownloadDocument;

public record DownloadDocumentRequest
{
    public required Guid DocumentId { get; init; }
}

public sealed class DownloadDocumentRequestValidator : AbstractValidatorBase<DownloadDocumentRequest>
{
    public DownloadDocumentRequestValidator()
    {
        _ = RuleFor(x => x.DocumentId)
            .NotEmpty();
    }
}
