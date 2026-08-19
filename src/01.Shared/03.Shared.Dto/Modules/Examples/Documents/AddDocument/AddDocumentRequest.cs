using EBVL.Shared.Dto.Common.FileStorages;

namespace EBVL.Shared.Dto.Modules.Examples.Documents.AddDocument;

public record AddDocumentRequest
{
    public required string Description { get; init; }
    public required FileItem File { get; init; }
}

public sealed class AddDocumentRequestValidator : AbstractValidatorBase<AddDocumentRequest>
{
    public AddDocumentRequestValidator()
    {
        _ = RuleFor(x => x.Description)
            .NotEmpty()
            .MinimumLength(DocumentsMinimumLengthFor.Description)
            .MaximumLength(DocumentsMaximumLengthFor.Description);

        _ = RuleFor(x => x.File)
            .SetValidator(new FileItemValidator());
    }
}
