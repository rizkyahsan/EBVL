using Pertamina.Services.FileStorage;
using EBVL.Shared.Dto.Modules.Examples.Documents.DeleteDocument;

namespace EBVL.BackEnd.Logics.Modules.Examples.Documents.DeleteDocument;

[AuthorizeRequest]
public sealed record DeleteDocumentCommand : DeleteDocumentRequest, IRequest
{
}

public sealed class DeleteDocumentCommandValidator : AbstractValidatorBase<DeleteDocumentCommand>
{
    public DeleteDocumentCommandValidator()
    {
        Include(new DeleteDocumentRequestValidator());
    }
}

public sealed class DeleteDocumentCommandHandler(
    IDatabaseService databaseService,
    IFileStorageService fileStorageService)
    : IRequestHandler<DeleteDocumentCommand>
{
    public async Task Handle(DeleteDocumentCommand request, CancellationToken cancellationToken)
    {
        var document = await databaseService.Documents
            .Where(x => !x.IsDeleted && x.Id == request.DocumentId)
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw ExceptionFor.EntityNotFound(DocumentsDisplayTextFor.Document, CommonDisplayTextFor.Id, request.DocumentId);

        var filePath = Path.Combine(DocumentsValueFor.SubFolder, document.StoredFileName);
        await fileStorageService.DeleteAsync(filePath, cancellationToken);

        document.IsDeleted = true;

        _ = await databaseService.SaveAsync(nameof(DeleteDocument), cancellationToken);
    }
}
