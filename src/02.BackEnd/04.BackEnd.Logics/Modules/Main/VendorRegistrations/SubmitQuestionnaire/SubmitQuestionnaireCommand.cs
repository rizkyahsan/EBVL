using EBVL.BackEnd.Logics.Modules.Main.VendorRegistrations.Common;
using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.Questionnaires;

namespace EBVL.BackEnd.Logics.Modules.Main.VendorRegistrations.SubmitQuestionnaire;

public sealed record SubmitQuestionnaireCommand(Guid RegistrationId, SubmitQuestionnaireRequest Request) : IRequest<QuestionnaireRuntimeResponse>;

public sealed class SubmitQuestionnaireHandler(IDatabaseService databaseService) : IRequestHandler<SubmitQuestionnaireCommand, QuestionnaireRuntimeResponse>
{
    public async Task<QuestionnaireRuntimeResponse> Handle(SubmitQuestionnaireCommand request, CancellationToken cancellationToken)
    {
        var registration = await QuestionnaireRuntime.Load(databaseService, request.RegistrationId, request.Request.ResumeToken, true, cancellationToken);
        if (registration.Status == VendorRegistrationStatus.Submitted)
        {
            return await QuestionnaireRuntime.LoadResponse(databaseService, registration.Id, request.Request.ResumeToken, null, cancellationToken);
        }

        QuestionnaireRuntime.SetConcurrency(databaseService, registration, request.Request.RowVersion);
        if (!await QuestionnaireRuntime.IsDocumentEvidenceComplete(databaseService, registration, cancellationToken))
        {
            throw new ValidationException("All mandatory documents must be uploaded before submission.");
        }

        var applicable = QuestionnaireRuntime.ApplicableQuestions(registration, []).ToList();
        foreach (var question in applicable)
        {
            var answer = registration.Answers.SingleOrDefault(x => x.QuestionnaireQuestionId == question.Id);
            var value = answer is null ? null : QuestionnaireRuntime.ToValue(answer);
            if (QuestionnaireRuntime.IsRequired(registration, question) && !QuestionnaireRuntime.HasValue(value, answer))
            {
                throw new ValidationException($"'{question.Label}' is required.");
            }

            if (value is not null)
            {
                QuestionnaireRuntime.ValidateValue(question, value);
            }
        }

        var ids = applicable.Select(x => x.Id).ToHashSet();
        databaseService.QuestionnaireAnswers.RemoveRange(registration.Answers.Where(x => !ids.Contains(x.QuestionnaireQuestionId) && x.Files.Count == 0));
        registration.Status = VendorRegistrationStatus.Submitted;
        registration.SubmittedAt = DateTimeOffset.UtcNow;
        _ = await databaseService.SaveAsync(nameof(SubmitQuestionnaireCommand), cancellationToken);

        return await QuestionnaireRuntime.LoadResponse(databaseService, registration.Id, request.Request.ResumeToken, null, cancellationToken);
    }
}
