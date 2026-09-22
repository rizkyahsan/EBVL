using EBVL.BackEnd.Logics.Common.Services.FileStorageDb;
using EBVL.BackEnd.Logics.Modules.Main.VendorRegistrations.Common;

namespace EBVL.BackEnd.Logics.Modules.Main.VendorRegistrations.DownloadQuestionnaireFile;

public sealed record DownloadQuestionnaireFileQuery(Guid RegistrationId, Guid FileId, string ResumeToken) : IRequest<QuestionnaireFileContent>;

public sealed class DownloadQuestionnaireFileHandler(IDatabaseService databaseService, IFileStorageDbService storage) : IRequestHandler<DownloadQuestionnaireFileQuery, QuestionnaireFileContent>
{
    public async Task<QuestionnaireFileContent> Handle(DownloadQuestionnaireFileQuery request, CancellationToken cancellationToken)
    {
        _ = await QuestionnaireRuntime.Load(databaseService, request.RegistrationId, request.ResumeToken, false, cancellationToken);
        var file = await databaseService.QuestionnaireAnswerFiles.AsNoTracking().SingleOrDefaultAsync(x => x.Id == request.FileId && x.VendorRegistrationId == request.RegistrationId && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("File was not found.");

        return new(await storage.ReadAsync(file.FileStorageId, cancellationToken), file.ContentType, file.OriginalFileName);
    }
}
