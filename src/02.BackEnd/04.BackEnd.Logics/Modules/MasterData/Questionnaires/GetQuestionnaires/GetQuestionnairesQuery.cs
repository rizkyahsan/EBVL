using EBVL.BackEnd.Logics.Modules.MasterData.Questionnaires.Common;
using EBVL.Shared.Dto.Modules.MasterData.Questionnaires;

namespace EBVL.BackEnd.Logics.Modules.MasterData.Questionnaires.GetQuestionnaires;

[AuthorizeRequest]
public sealed record GetQuestionnairesQuery : IRequest<GetQuestionnairesResponse>;

public sealed class GetQuestionnairesHandler(IDatabaseService db) : IRequestHandler<GetQuestionnairesQuery, GetQuestionnairesResponse>
{
    public async Task<GetQuestionnairesResponse> Handle(GetQuestionnairesQuery r, CancellationToken cancellationToken)
    {
        var ids = await db.Questionnaires.AsNoTracking().Where(x => !x.IsDeleted)
            .Where(x => !db.Questionnaires.Any(other => !other.IsDeleted && other.QuestionnaireSeriesId == x.QuestionnaireSeriesId && other.Version > x.Version))
            .OrderBy(x => x.BusinessProcess).ThenBy(x => x.Code)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);
        var rows = new List<QuestionnaireListItem>(ids.Count);
        foreach (var id in ids)
        {
            var q = await QuestionnaireGraph.Load(db, id, false, cancellationToken);
            var state = await QuestionnaireGraph.State(db, q, cancellationToken);
            rows.Add(new(q.Id, q.QuestionnaireSeriesId, q.Code, q.BusinessProcess, q.Version, q.Status, q.Modified ?? q.Created, q.Sections.Count, state.HasChanges, state.IsValidForPublish, state.CanPublish, state.Errors));
        }

        return new() { Items = rows };
    }
}
