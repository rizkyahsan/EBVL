using EBVL.Shared.Dto.Modules.MasterData.Questionnaires;

namespace EBVL.BackEnd.Logics.Modules.MasterData.Questionnaires.GetQuestionnaireVersionHistory;

[AuthorizeRequest]
public sealed record GetQuestionnaireVersionHistoryQuery(Guid QuestionnaireSeriesId) : IRequest<GetQuestionnaireVersionHistoryResponse>;

public sealed class GetQuestionnaireVersionHistoryHandler(IDatabaseService db) : IRequestHandler<GetQuestionnaireVersionHistoryQuery, GetQuestionnaireVersionHistoryResponse>
{
    public async Task<GetQuestionnaireVersionHistoryResponse> Handle(GetQuestionnaireVersionHistoryQuery request, CancellationToken cancellationToken)
    {
        var items = await db.Questionnaires.AsNoTracking()
            .Where(x => !x.IsDeleted && x.QuestionnaireSeriesId == request.QuestionnaireSeriesId)
            .OrderByDescending(x => x.Version)
            .Select(x => new QuestionnaireVersionItem(x.Id, x.Version, x.Status, x.PublishedAt, x.PublishedBy, x.Created))
            .ToListAsync(cancellationToken);
        return new() { Items = items };
    }
}
