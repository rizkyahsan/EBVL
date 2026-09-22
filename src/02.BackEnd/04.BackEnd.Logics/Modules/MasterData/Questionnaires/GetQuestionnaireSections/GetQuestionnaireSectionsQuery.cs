using EBVL.Shared.Dto.Modules.MasterData.Questionnaires;

namespace EBVL.BackEnd.Logics.Modules.MasterData.Questionnaires.GetQuestionnaireSections;

[AuthorizeRequest]
public sealed record GetQuestionnaireSectionsQuery(Guid QuestionnaireId) : GetQuestionnaireSectionsRequest, IRequest<GetQuestionnaireSectionsResponse>;

public sealed class GetQuestionnaireSectionsHandler(IDatabaseService db) : IRequestHandler<GetQuestionnaireSectionsQuery, GetQuestionnaireSectionsResponse>
{
    public async Task<GetQuestionnaireSectionsResponse> Handle(GetQuestionnaireSectionsQuery r, CancellationToken cancellationToken)
    {
        _ = await db.Questionnaires.AsNoTracking().Where(x => x.Id == r.QuestionnaireId && !x.IsDeleted).Select(x => x.Id).SingleOrDefaultAsync(cancellationToken) is var id && id != Guid.Empty ? id : throw new KeyNotFoundException("Questionnaire was not found.");
        var query = db.QuestionnaireSections.AsNoTracking().Where(x => x.QuestionnaireId == r.QuestionnaireId && !x.IsDeleted);
        if (!string.IsNullOrWhiteSpace(r.SearchText))
        {
            query = query.Where(x => x.Code.Contains(r.SearchText) || x.Title.Contains(r.SearchText));
        }

        var desc = r.SortOrder == Pertamina.Common.Dto.Enums.SortOrder.Descending;
        var ordered = r.SortField?.ToLowerInvariant() switch
        {
            "code" => desc ? query.OrderByDescending(x => x.Code).ThenByDescending(x => x.Id) : query.OrderBy(x => x.Code).ThenBy(x => x.Id),
            "title" => desc ? query.OrderByDescending(x => x.Title).ThenByDescending(x => x.Id) : query.OrderBy(x => x.Title).ThenBy(x => x.Id),
            _ => desc ? query.OrderByDescending(x => x.Order).ThenByDescending(x => x.Id) : query.OrderBy(x => x.Order).ThenBy(x => x.Id)
        };
        var total = await query.CountAsync(cancellationToken);
        var page = r.Page ?? 1;
        var size = r.PageSize ?? 10;
        var items = await ordered.Skip((page - 1) * size).Take(size).Select(x => new QuestionnaireSectionListItem(x.Id, x.Code, x.Title, x.Questionnaire.BusinessProcess, x.CompanyType, x.Order, x.IsActive, x.Questions.Count(q => !q.IsDeleted))).ToListAsync(cancellationToken);
        return new() { Items = items, TotalCount = total };
    }
}
