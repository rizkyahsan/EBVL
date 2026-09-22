using EBVL.BackEnd.Logics.Modules.MasterData.Questionnaires.Common;
using EBVL.Shared.Dto.Modules.MasterData.Questionnaires;

namespace EBVL.BackEnd.Logics.Modules.MasterData.Questionnaires.CreateQuestionnaireDraft;

[AuthorizeRequestByPermission(QuestionnairePermissions.Manage)]
public sealed record CreateQuestionnaireDraftCommand(Guid QuestionnaireId) : IRequest<GetQuestionnaireResponse>;

public sealed class CreateQuestionnaireDraftHandler(IDatabaseService db) : IRequestHandler<CreateQuestionnaireDraftCommand, GetQuestionnaireResponse>
{
    public async Task<GetQuestionnaireResponse> Handle(CreateQuestionnaireDraftCommand request, CancellationToken cancellationToken)
    {
        await using var tx = await db.BeginTransactionAsync(cancellationToken);
        var source = await QuestionnaireGraph.Load(db, request.QuestionnaireId, true, cancellationToken);
        var existing = await db.Questionnaires.FirstOrDefaultAsync(x => !x.IsDeleted && x.QuestionnaireSeriesId == source.QuestionnaireSeriesId && x.Status == QuestionnaireStatus.Draft, cancellationToken);
        if (existing is not null)
        {
            await tx.CommitAsync(cancellationToken);
            return new() { Item = await QuestionnaireGraph.MapWithState(db, await QuestionnaireGraph.Load(db, existing.Id, false, cancellationToken), cancellationToken) };
        }

        if (source.Status != QuestionnaireStatus.Publish)
        {
            throw new ValidationException("A new draft can only be created from the published questionnaire.");
        }

        var draft = await QuestionnaireGraph.CreateVersion(db, source, QuestionnaireGraph.ToUpdateRequest(source), cancellationToken);
        _ = await db.SaveAsync(nameof(CreateQuestionnaireDraftCommand), cancellationToken);
        await tx.CommitAsync(cancellationToken);
        return new() { Item = await QuestionnaireGraph.MapWithState(db, draft, cancellationToken) };
    }
}
