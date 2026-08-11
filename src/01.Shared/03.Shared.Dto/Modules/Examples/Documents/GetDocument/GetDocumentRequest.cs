namespace EBVL.Shared.Dto.Modules.Examples.Documents.GetDocument;

public record GetDocumentRequest
{
    public required Guid DocumentId { get; init; }
}

public sealed class GetDocumentRequestValidator : AbstractValidatorBase<GetDocumentRequest>
{
    public GetDocumentRequestValidator()
    {
        _ = RuleFor(x => x.DocumentId)
            .NotEmpty();
    }
}
