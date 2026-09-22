using EBVL.BackEnd.Logics.Modules.MasterData.Questionnaires.Common;
using EBVL.Shared.Dto.Modules.MasterData.Questionnaires;

namespace EBVL.BackEnd.Logics.Modules.MasterData.Questionnaires.GetQuestionnaireQuestions;

[AuthorizeRequest]
public sealed record GetQuestionnaireQuestionsQuery(Guid QuestionnaireId, Guid SectionId) : GetQuestionnaireQuestionsRequest, IRequest<GetQuestionnaireQuestionsResponse>;

public sealed class GetQuestionnaireQuestionsHandler(IDatabaseService db) : IRequestHandler<GetQuestionnaireQuestionsQuery, GetQuestionnaireQuestionsResponse>
{
    public async Task<GetQuestionnaireQuestionsResponse> Handle(GetQuestionnaireQuestionsQuery r, CancellationToken cancellationToken)
    {
        var section = await QuestionnaireGraph.LoadSection(db, r.QuestionnaireId, r.SectionId, false, cancellationToken);
        var query = section.Questions.Where(x => !x.IsDeleted).AsQueryable();
        if (!string.IsNullOrWhiteSpace(r.SearchText))
        {
            query = query.Where(x => x.Code.Contains(r.SearchText, StringComparison.OrdinalIgnoreCase) || x.Label.Contains(r.SearchText, StringComparison.OrdinalIgnoreCase));
        }

        query = r.SortField?.ToLowerInvariant() switch { "code" => query.OrderBy(x => x.Code).ThenBy(x => x.Id), "label" => query.OrderBy(x => x.Label).ThenBy(x => x.Id), _ => query.OrderBy(x => x.Order).ThenBy(x => x.Id) };
        if (r.SortOrder == Pertamina.Common.Dto.Enums.SortOrder.Descending)
        {
            query = query.Reverse();
        }

        var total = query.Count();
        var page = r.Page ?? 1;
        var size = r.PageSize ?? 10;
        return new() { TotalCount = total, Items = query.Skip((page - 1) * size).Take(size).Select(QuestionnaireGraph.MapQuestion).ToList() };
    }
}
