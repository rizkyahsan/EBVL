using EBVL.BackEnd.Logics.Common.Services.FileStorageDb;
using EBVL.BackEnd.Logics.Modules.Main.VendorRegistrations.Common;

namespace EBVL.BackEnd.Logics.Modules.Main.VendorRegistrations.DownloadVendorRegistrationDocument;

public sealed record DownloadVendorRegistrationDocumentQuery(Guid RegistrationId, Guid DocumentId, string ResumeToken) : IRequest<QuestionnaireFileContent>;

public sealed class DownloadVendorRegistrationDocumentHandler(IDatabaseService databaseService, IFileStorageDbService storage) : IRequestHandler<DownloadVendorRegistrationDocumentQuery, QuestionnaireFileContent>
{
    public async Task<QuestionnaireFileContent> Handle(DownloadVendorRegistrationDocumentQuery request, CancellationToken cancellationToken)
    {
        _ = await QuestionnaireRuntime.Load(databaseService, request.RegistrationId, request.ResumeToken, false, cancellationToken);
        var document = await databaseService.VendorRegistrationDocuments.AsNoTracking().SingleOrDefaultAsync(x => x.Id == request.DocumentId && x.VendorRegistrationId == request.RegistrationId && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("Document was not found.");

        return new(await storage.ReadAsync(document.FileStorageId, cancellationToken), document.ContentType, document.OriginalFileName);
    }
}
