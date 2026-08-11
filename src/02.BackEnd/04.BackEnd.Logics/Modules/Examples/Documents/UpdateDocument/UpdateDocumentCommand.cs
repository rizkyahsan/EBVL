using EBVL.Shared.Dto.Modules.Examples.Documents.UpdateDocument;

namespace EBVL.BackEnd.Logics.Modules.Examples.Documents.UpdateDocument;

[AuthorizeRequest]
public sealed record UpdateDocumentCommand : UpdateDocumentRequest, IRequest
{
}

public sealed class UpdateDocumentCommandValidator : AbstractValidatorBase<UpdateDocumentCommand>
{
    public UpdateDocumentCommandValidator()
    {
        Include(new UpdateDocumentRequestValidator());
    }
}

public sealed class UpdateDocumentCommandHandler(IDatabaseService databaseService)
    : IRequestHandler<UpdateDocumentCommand>
{
    public async Task Handle(UpdateDocumentCommand request, CancellationToken cancellationToken)
    {
        var document = await databaseService.Documents
            .Where(x => !x.IsDeleted && x.Id == request.DocumentId)
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw ExceptionFor.EntityNotFound(DocumentsDisplayTextFor.Document, CommonDisplayTextFor.Id, request.DocumentId);

        document.Description = request.Description;

        _ = await databaseService.SaveAsync(nameof(UpdateDocument), cancellationToken);
    }
}
