using EBVL.BackEnd.Logics.Modules.MasterData.Questionnaires.Common;
using EBVL.Shared.Dto.Modules.MasterData.Questionnaires;
using Pertamina.Services.CurrentUser;

namespace EBVL.BackEnd.Logics.Modules.MasterData.Questionnaires.PublishQuestionnaire;

[AuthorizeRequestByPermission(QuestionnairePermissions.Manage)]
public sealed record PublishQuestionnaireCommand(Guid QuestionnaireId, string RowVersion) : IRequest<GetQuestionnaireResponse>;

public sealed class PublishQuestionnaireHandler(IDatabaseService db, ICurrentUserService currentUser) : IRequestHandler<PublishQuestionnaireCommand, GetQuestionnaireResponse>
{
    public async Task<GetQuestionnaireResponse> Handle(PublishQuestionnaireCommand request, CancellationToken cancellationToken)
    {
        await using var tx = await db.BeginTransactionAsync(cancellationToken);
        var draft = await QuestionnaireGraph.Load(db, request.QuestionnaireId, true, cancellationToken);
        QuestionnaireGraph.EnsureDraft(draft);
        await QuestionnaireGraph.EnsureNotReferenced(db, draft.Id, cancellationToken);
        db.SetQuestionnaireOriginalRowVersion(draft, QuestionnaireGraph.ParseRowVersion(request.RowVersion));
        var state = await QuestionnaireGraph.State(db, draft, cancellationToken);
        if (!state.HasChanges)
        {
            throw new ValidationException("The draft has no changes to publish.");
        }

        var errors = QuestionnaireGraph.Validate(draft, draft.Rules.Select(x => new QuestionnaireRuleItem(x.Id, x.SourceQuestionId, x.TargetQuestionId, x.Operator, x.Action, x.Value)).ToList());
        if (errors.Count != 0)
        {
            throw new ValidationException(string.Join("; ", errors));
        }

        var published = await db.Questionnaires.SingleOrDefaultAsync(x => !x.IsDeleted && x.QuestionnaireSeriesId == draft.QuestionnaireSeriesId && x.Status == QuestionnaireStatus.Publish, cancellationToken);
        try
        {
            if (published is not null)
            {
                published.Status = QuestionnaireStatus.Superseded;
                published.IsActive = false;
                _ = await db.SaveAsync(nameof(PublishQuestionnaireCommand), cancellationToken);
            }

            draft.Status = QuestionnaireStatus.Publish;
            draft.IsActive = true;
            draft.PublishedAt = DateTimeOffset.UtcNow;
            draft.PublishedBy = currentUser.Username ?? "EBVLSystem";
            _ = await db.SaveAsync(nameof(PublishQuestionnaireCommand), cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ValidationException("The questionnaire has changed or was already published. Reload the page and try again.");
        }
        catch (DbUpdateException)
        {
            throw new ValidationException("Another questionnaire version was published concurrently. Reload the page and try again.");
        }

        await tx.CommitAsync(cancellationToken);
        return new() { Item = await QuestionnaireGraph.MapWithState(db, draft, cancellationToken) };
    }
}
