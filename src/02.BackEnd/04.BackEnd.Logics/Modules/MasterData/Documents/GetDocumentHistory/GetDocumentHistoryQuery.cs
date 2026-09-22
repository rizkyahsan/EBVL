using EBVL.Shared.Dto.Modules.MasterData.Documents;

namespace EBVL.BackEnd.Logics.Modules.MasterData.Documents.GetDocumentHistory;

[AuthorizeRequest]
public sealed record GetDocumentHistoryQuery(Guid SeriesId) : IRequest<GetDocumentHistoryResponse>;

public sealed class GetDocumentHistoryHandler(IDatabaseService db) : IRequestHandler<GetDocumentHistoryQuery, GetDocumentHistoryResponse>
{
    public async Task<GetDocumentHistoryResponse> Handle(GetDocumentHistoryQuery request, CancellationToken cancellationToken)
    {
        return new() { Items = await db.DocumentRequirementSets.AsNoTracking().Where(x => !x.IsDeleted && x.DocumentRequirementSetSeriesId == request.SeriesId).OrderByDescending(x => x.Version).Select(x => new DocumentRequirementSetVersionItem(x.Id, x.Version, x.Status, x.PublishedAt, x.PublishedBy, x.Created)).ToListAsync(cancellationToken) };
    }
}
