namespace EBVL.Shared.Dto.Modules.Examples.Documents.DeleteDocument;

public record DeleteDocumentRequest
{
    public required Guid DocumentId { get; init; }
}

public sealed class DeleteDocumentRequestValidator : AbstractValidatorBase<DeleteDocumentRequest>
{
    public DeleteDocumentRequestValidator()
    {
        _ = RuleFor(x => x.DocumentId)
            .NotEmpty();
    }
}
