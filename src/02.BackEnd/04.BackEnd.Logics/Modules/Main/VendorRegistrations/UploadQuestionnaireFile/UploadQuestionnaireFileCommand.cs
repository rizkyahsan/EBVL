using EBVL.BackEnd.Logics.Common.Services.FileStorageDb;
using EBVL.BackEnd.Logics.Modules.Main.VendorRegistrations.Common;
using EBVL.Shared.Dto.Common.FileStorages;
using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.Questionnaires;

namespace EBVL.BackEnd.Logics.Modules.Main.VendorRegistrations.UploadQuestionnaireFile;

public sealed record UploadQuestionnaireFileCommand(Guid RegistrationId, Guid QuestionId, string ResumeToken, FileItem File) : IRequest<UploadQuestionnaireFileResponse>;

public sealed class UploadQuestionnaireFileHandler(IDatabaseService databaseService, IFileStorageDbService storage) : IRequestHandler<UploadQuestionnaireFileCommand, UploadQuestionnaireFileResponse>
{
    public async Task<UploadQuestionnaireFileResponse> Handle(UploadQuestionnaireFileCommand request, CancellationToken cancellationToken)
    {
        var registration = await QuestionnaireRuntime.Load(databaseService, request.RegistrationId, request.ResumeToken, true, cancellationToken);
        QuestionnaireRuntime.EnsureDraft(registration);
        databaseService.SetQuestionnaireRuntimeGraphUnchanged();
        var question = QuestionnaireRuntime.ApplicableQuestions(registration, []).SingleOrDefault(x => x.Id == request.QuestionId && x.Type == QuestionnaireQuestionType.File)
            ?? throw new ValidationException("The file question is unknown or non-applicable.");
        QuestionnaireRuntime.ValidatePdf(request.File);

        var answer = registration.Answers.SingleOrDefault(x => x.QuestionnaireQuestionId == question.Id);
        if (answer is null)
        {
            answer = new QuestionnaireAnswer { Id = Guid.CreateVersion7(), VendorRegistrationId = registration.Id, QuestionnaireQuestionId = question.Id };
            _ = databaseService.QuestionnaireAnswers.Add(answer);
        }

        var stored = await storage.CreateAsync(request.File, cancellationToken);
        var existingFiles = answer.Files.Where(x => !x.IsDeleted).ToList();
        foreach (var existingFile in existingFiles)
        {
            existingFile.IsDeleted = true;
        }

        var file = new QuestionnaireAnswerFile { Id = Guid.CreateVersion7(), VendorRegistrationId = registration.Id, QuestionnaireAnswerId = answer.Id, QuestionnaireQuestionId = question.Id, FileStorageId = stored.Id, OriginalFileName = Path.GetFileName(request.File.FileName), ContentType = "application/pdf", Length = request.File.FileContent.LongLength };
        _ = databaseService.QuestionnaireAnswerFiles.Add(file);
        _ = await databaseService.SaveAsync(nameof(UploadQuestionnaireFileCommand), cancellationToken);
        foreach (var existingFile in existingFiles)
        {
            await storage.DeleteAsync(existingFile.FileStorageId, cancellationToken);
        }

        return new(new(file.Id, file.OriginalFileName, file.ContentType, file.Length), Convert.ToBase64String(registration.RowVersion));
    }
}
