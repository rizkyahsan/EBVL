using EBVL.BackEnd.Logics.Modules.MasterData.Questionnaires.Common;
using EBVL.Shared.Dto.Modules.MasterData.Questionnaires;

namespace EBVL.BackEnd.Logics.Modules.MasterData.Questionnaires.AddQuestionnaire;

[AuthorizeRequestByPermission(QuestionnairePermissions.Manage)]
public sealed record AddQuestionnaireCommand : AddQuestionnaireRequest, IRequest<GetQuestionnaireResponse>;

public sealed class AddQuestionnaireCommandValidator : AbstractValidatorBase<AddQuestionnaireCommand>
{
    public AddQuestionnaireCommandValidator()
    {
        Include(new AddQuestionnaireRequestValidator());
    }
}

public sealed class AddQuestionnaireHandler(IDatabaseService db) : IRequestHandler<AddQuestionnaireCommand, GetQuestionnaireResponse>
{
    public async Task<GetQuestionnaireResponse> Handle(AddQuestionnaireCommand r, CancellationToken cancellationToken)
    {
        var code = Code(r.Code);
        if (await db.Questionnaires.AnyAsync(x => x.Code == code && !x.IsDeleted, cancellationToken))
        {
            throw new ValidationException("A questionnaire business process with the same code already exists.");
        }

        var questionnaire = new Questionnaire { QuestionnaireSeriesId = Guid.CreateVersion7(), Code = code, BusinessProcess = r.BusinessProcess.Trim(), Version = 1, Status = QuestionnaireStatus.Draft, IsActive = r.IsActive };
        _ = await db.Questionnaires.AddAsync(questionnaire, cancellationToken);
        _ = await db.SaveAsync(nameof(AddQuestionnaireCommand), cancellationToken);
        return new() { Item = await QuestionnaireGraph.MapWithState(db, questionnaire, cancellationToken) };
    }
    private static string Code(string value)
    {
        return string.Join('_', value.Trim().ToUpperInvariant().Split([' ', '-', '/'], StringSplitOptions.RemoveEmptyEntries));
    }
}
