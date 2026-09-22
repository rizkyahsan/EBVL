using EBVL.BackEnd.Logics.Common.Services.FileStorageDb;
using EBVL.BackEnd.Logics.Modules.Main.VendorRegistrations.Common;

namespace EBVL.BackEnd.Logics.Modules.Main.VendorRegistrations.DeleteQuestionnaireFile;

public sealed record DeleteQuestionnaireFileCommand(Guid RegistrationId, Guid FileId, string ResumeToken) : IRequest;

public sealed class DeleteQuestionnaireFileHandler(IDatabaseService databaseService, IFileStorageDbService storage) : IRequestHandler<DeleteQuestionnaireFileCommand>
{
    public async Task Handle(DeleteQuestionnaireFileCommand request, CancellationToken cancellationToken)
    {
        var registration = await QuestionnaireRuntime.Load(databaseService, request.RegistrationId, request.ResumeToken, true, cancellationToken);
        QuestionnaireRuntime.EnsureDraft(registration);
        databaseService.SetQuestionnaireRuntimeGraphUnchanged();
        var file = await databaseService.QuestionnaireAnswerFiles.SingleOrDefaultAsync(x => x.Id == request.FileId && x.VendorRegistrationId == registration.Id && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("File was not found.");
        file.IsDeleted = true;
        await storage.DeleteAsync(file.FileStorageId, cancellationToken);
        _ = await databaseService.SaveAsync(nameof(DeleteQuestionnaireFileCommand), cancellationToken);
    }
}
