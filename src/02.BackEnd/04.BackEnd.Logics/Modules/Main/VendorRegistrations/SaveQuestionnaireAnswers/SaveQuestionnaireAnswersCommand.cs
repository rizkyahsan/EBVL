using EBVL.BackEnd.Logics.Modules.Main.VendorRegistrations.Common;
using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.Questionnaires;

namespace EBVL.BackEnd.Logics.Modules.Main.VendorRegistrations.SaveQuestionnaireAnswers;

public sealed record SaveQuestionnaireAnswersCommand(Guid RegistrationId, SaveAnswersRequest Request) : IRequest<QuestionnaireRuntimeResponse>;

public sealed class SaveQuestionnaireAnswersHandler(IDatabaseService databaseService) : IRequestHandler<SaveQuestionnaireAnswersCommand, QuestionnaireRuntimeResponse>
{
    public async Task<QuestionnaireRuntimeResponse> Handle(SaveQuestionnaireAnswersCommand request, CancellationToken cancellationToken)
    {
        await using var transaction = await databaseService.BeginTransactionAsync(cancellationToken);
        var registration = await QuestionnaireRuntime.Load(databaseService, request.RegistrationId, request.Request.ResumeToken, true, cancellationToken);
        QuestionnaireRuntime.EnsureDraft(registration);
        QuestionnaireRuntime.SetConcurrency(databaseService, registration, request.Request.RowVersion);
        if (request.Request.Answers.GroupBy(x => x.QuestionId).Any(x => x.Count() > 1))
        {
            throw new ValidationException("Duplicate answers are not allowed.");
        }

        var questions = QuestionnaireRuntime.ApplicableQuestions(registration, request.Request.Answers).ToDictionary(x => x.Id);
        var answers = new List<QuestionnaireAnswer>();
        foreach (var value in request.Request.Answers)
        {
            if (!questions.TryGetValue(value.QuestionId, out var question))
            {
                throw new ValidationException("An answer references an unknown or non-applicable question.");
            }

            QuestionnaireRuntime.ValidateValue(question, value);
            var answer = new QuestionnaireAnswer { VendorRegistrationId = registration.Id, QuestionnaireQuestionId = value.QuestionId };
            QuestionnaireRuntime.Assign(answer, value);
            answers.Add(answer);
        }

        var questionIds = request.Request.Answers.Select(x => x.QuestionId).ToList();
        _ = await databaseService.QuestionnaireAnswers
            .Where(x => x.VendorRegistrationId == registration.Id && questionIds.Contains(x.QuestionnaireQuestionId))
            .ExecuteDeleteAsync(cancellationToken);
        await databaseService.QuestionnaireAnswers.AddRangeAsync(answers, cancellationToken);
        _ = await databaseService.SaveAsync(nameof(SaveQuestionnaireAnswersCommand), cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return await QuestionnaireRuntime.LoadResponse(databaseService, registration.Id, request.Request.ResumeToken, null, cancellationToken);
    }
}
