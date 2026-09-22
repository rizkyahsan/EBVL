using EBVL.BackEnd.Logics.Modules.MasterData.Questionnaires.Common;
using EBVL.Shared.Dto.Modules.MasterData.Questionnaires;

namespace EBVL.BackEnd.Logics.Modules.MasterData.Questionnaires.DeleteQuestionnaireQuestion;

[AuthorizeRequestByPermission(QuestionnairePermissions.Manage)]
public sealed record DeleteQuestionnaireQuestionCommand(Guid QuestionnaireId, Guid SectionId, Guid QuestionId, string RowVersion) : IRequest<GetQuestionnaireResponse>;

public sealed class DeleteQuestionnaireQuestionHandler(IDatabaseService db) : IRequestHandler<DeleteQuestionnaireQuestionCommand, GetQuestionnaireResponse>
{
    public async Task<GetQuestionnaireResponse> Handle(DeleteQuestionnaireQuestionCommand r, CancellationToken cancellationToken)
    {
        await using var tx = await db.BeginTransactionAsync(cancellationToken);
        var q = await QuestionnaireGraph.Load(db, r.QuestionnaireId, true, cancellationToken);
        QuestionnaireGraph.EnsureDraft(q);
        await QuestionnaireGraph.EnsureNotReferenced(db, q.Id, cancellationToken);
        db.SetQuestionnaireOriginalRowVersion(q, QuestionnaireGraph.ParseRowVersion(r.RowVersion));
        var section = q.Sections.SingleOrDefault(x => x.Id == r.SectionId && !x.IsDeleted) ?? throw new KeyNotFoundException("Questionnaire section was not found.");
        var question = section.Questions.SingleOrDefault(x => x.Id == r.QuestionId && !x.IsDeleted) ?? throw new KeyNotFoundException("Questionnaire question was not found.");
        if (q.Rules.Any(x => x.SourceQuestionId == question.Id || x.TargetQuestionId == question.Id))
        {
            throw new ValidationException("The question cannot be deleted while it is used by a rule.");
        }

        _ = db.QuestionnaireQuestions.Remove(question);
        _ = section.Questions.Remove(question);
        var order = 1;
        foreach (var item in section.Questions.Where(x => !x.IsDeleted).OrderBy(x => x.Order))
        {
            item.Order = order++;
        }

        q.Modified = DateTimeOffset.UtcNow;
        await QuestionnaireGraph.SaveMutation(db, nameof(DeleteQuestionnaireQuestionCommand), cancellationToken);
        await tx.CommitAsync(cancellationToken);
        return new() { Item = await QuestionnaireGraph.MapWithState(db, q, cancellationToken) };
    }
}
