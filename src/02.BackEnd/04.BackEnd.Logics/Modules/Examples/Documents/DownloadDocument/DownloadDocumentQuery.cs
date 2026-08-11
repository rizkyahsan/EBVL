using Pertamina.Services.FileStorage;
using EBVL.Shared.Dto.Modules.Examples.Documents.DownloadDocument;

namespace EBVL.BackEnd.Logics.Modules.Examples.Documents.DownloadDocument;

[AuthorizeRequest]
public sealed record DownloadDocumentQuery : DownloadDocumentRequest, IRequest<DownloadDocumentResponse>
{
}

public sealed class DownloadDocumentQueryValidator : AbstractValidatorBase<DownloadDocumentQuery>
{
    public DownloadDocumentQueryValidator()
    {
        Include(new DownloadDocumentRequestValidator());
    }
}

public sealed class DownloadDocumentQueryHandler(
    IDatabaseService databaseService,
    IFileStorageService fileStorageService)
    : IRequestHandler<DownloadDocumentQuery, DownloadDocumentResponse>
{
    public async Task<DownloadDocumentResponse> Handle(DownloadDocumentQuery request, CancellationToken cancellationToken)
    {
        var document = await databaseService.Documents
            .Where(x => !x.IsDeleted && x.Id == request.DocumentId)
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw ExceptionFor.EntityNotFound(DocumentsDisplayTextFor.Document, CommonDisplayTextFor.Id, request.DocumentId);

        var filePath = Path.Combine(DocumentsValueFor.SubFolder, document.StoredFileName);
        var fileContent = await fileStorageService.ReadAsync(filePath, cancellationToken);

        return new DownloadDocumentResponse
        {
            FileName = document.OriginalFileName,
            FileContentType = document.FileContentType,
            FileContent = fileContent,
        };
    }
}
