namespace EBVL.Shared.Dto.Modules.Examples.Documents.UpdateDocument;

public record UpdateDocumentRequest
{
    public required Guid DocumentId { get; init; }
    public required string Description { get; init; }
}

public sealed class UpdateDocumentRequestValidator : AbstractValidatorBase<UpdateDocumentRequest>
{
    public UpdateDocumentRequestValidator()
    {
        _ = RuleFor(x => x.DocumentId)
            .NotEmpty();

        _ = RuleFor(x => x.Description)
            .NotEmpty()
            .MinimumLength(DocumentsMinimumLengthFor.Description)
            .MaximumLength(DocumentsMaximumLengthFor.Description);
    }
}
