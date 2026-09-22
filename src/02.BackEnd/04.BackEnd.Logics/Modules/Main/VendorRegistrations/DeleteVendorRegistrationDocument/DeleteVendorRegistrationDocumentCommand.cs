using EBVL.BackEnd.Logics.Common.Services.FileStorageDb;
using EBVL.BackEnd.Logics.Modules.Main.VendorRegistrations.Common;

namespace EBVL.BackEnd.Logics.Modules.Main.VendorRegistrations.DeleteVendorRegistrationDocument;

public sealed record DeleteVendorRegistrationDocumentCommand(Guid RegistrationId, Guid DocumentId, string ResumeToken) : IRequest;

public sealed class DeleteVendorRegistrationDocumentHandler(IDatabaseService databaseService, IFileStorageDbService storage) : IRequestHandler<DeleteVendorRegistrationDocumentCommand>
{
    public async Task Handle(DeleteVendorRegistrationDocumentCommand request, CancellationToken cancellationToken)
    {
        var registration = await QuestionnaireRuntime.Load(databaseService, request.RegistrationId, request.ResumeToken, true, cancellationToken);
        QuestionnaireRuntime.EnsureDraft(registration);
        databaseService.SetQuestionnaireRuntimeGraphUnchanged();
        var document = registration.Documents.SingleOrDefault(x => x.Id == request.DocumentId && !x.IsDeleted)
            ?? throw new InvalidOperationException("Document was not found.");
        document.IsDeleted = true;
        await storage.DeleteAsync(document.FileStorageId, cancellationToken);
        _ = await databaseService.SaveAsync(nameof(DeleteVendorRegistrationDocumentCommand), cancellationToken);
    }
}
