using Pertamina.Services.FileStorage;
using EBVL.Shared.Dto.Modules.Examples.Documents.AddDocument;

namespace EBVL.BackEnd.Logics.Modules.Examples.Documents.AddDocument;

[AuthorizeRequest]
public sealed record AddDocumentCommand : AddDocumentRequest, IRequest<AddDocumentResponse>
{
}

public sealed class AddDocumentCommandValidator : AbstractValidatorBase<AddDocumentCommand>
{
    public AddDocumentCommandValidator()
    {
        Include(new AddDocumentRequestValidator());
    }
}

public sealed class AddDocumentCommandHandler(
    IDatabaseService databaseService,
    IFileStorageService fileStorageService)
    : IRequestHandler<AddDocumentCommand, AddDocumentResponse>
{
    public async Task<AddDocumentResponse> Handle(AddDocumentCommand request, CancellationToken cancellationToken)
    {
        var storedFileName = $"{Guid.CreateVersion7()}{Path.GetExtension(request.File.FileName)}";

        var document = new Document
        {
            Description = request.Description,
            OriginalFileName = request.File.FileName,
            StoredFileName = storedFileName,
            FileContentType = request.File.ContentType,
            FileSize = request.File.FileContent.LongLength
        };

        _ = await databaseService.Documents.AddAsync(document, cancellationToken);
        _ = await databaseService.SaveAsync(nameof(AddDocument), cancellationToken);

        var filePath = Path.Combine(DocumentsValueFor.SubFolder, storedFileName);
        await fileStorageService.CreateAsync(filePath, request.File.FileContent, cancellationToken);

        return new AddDocumentResponse
        {
            Item = new DocumentItem
            {
                Id = document.Id
            }
        };
    }
}
