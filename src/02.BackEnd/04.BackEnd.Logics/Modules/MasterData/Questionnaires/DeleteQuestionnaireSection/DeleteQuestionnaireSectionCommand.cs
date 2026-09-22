using EBVL.BackEnd.Logics.Modules.MasterData.Questionnaires.Common;
using EBVL.Shared.Dto.Modules.MasterData.Questionnaires;

namespace EBVL.BackEnd.Logics.Modules.MasterData.Questionnaires.DeleteQuestionnaireSection;

[AuthorizeRequestByPermission(QuestionnairePermissions.Manage)]
public sealed record DeleteQuestionnaireSectionCommand(Guid QuestionnaireId, Guid SectionId, string RowVersion) : IRequest<GetQuestionnaireResponse>;

public sealed class DeleteQuestionnaireSectionHandler(IDatabaseService db) : IRequestHandler<DeleteQuestionnaireSectionCommand, GetQuestionnaireResponse>
{
    public async Task<GetQuestionnaireResponse> Handle(DeleteQuestionnaireSectionCommand r, CancellationToken cancellationToken)
    {
        await using var tx = await db.BeginTransactionAsync(cancellationToken);
        var q = await QuestionnaireGraph.Load(db, r.QuestionnaireId, true, cancellationToken);
        QuestionnaireGraph.EnsureDraft(q);
        await QuestionnaireGraph.EnsureNotReferenced(db, q.Id, cancellationToken);
        db.SetQuestionnaireOriginalRowVersion(q, QuestionnaireGraph.ParseRowVersion(r.RowVersion));
        var section = q.Sections.SingleOrDefault(x => x.Id == r.SectionId && !x.IsDeleted) ?? throw new KeyNotFoundException("Questionnaire section was not found.");
        var questionIds = section.Questions.Select(x => x.Id).ToHashSet();
        if (q.Rules.Any(x => questionIds.Contains(x.SourceQuestionId) || questionIds.Contains(x.TargetQuestionId)))
        {
            throw new ValidationException("The section cannot be deleted while its questions are used by rules.");
        }

        _ = db.QuestionnaireSections.Remove(section);
        _ = q.Sections.Remove(section);
        var order = 1;
        foreach (var item in q.Sections.Where(x => !x.IsDeleted).OrderBy(x => x.Order))
        {
            item.Order = order++;
        }

        q.Modified = DateTimeOffset.UtcNow;
        await QuestionnaireGraph.SaveMutation(db, nameof(DeleteQuestionnaireSectionCommand), cancellationToken);
        await tx.CommitAsync(cancellationToken);
        return new() { Item = await QuestionnaireGraph.MapWithState(db, q, cancellationToken) };
    }
}
