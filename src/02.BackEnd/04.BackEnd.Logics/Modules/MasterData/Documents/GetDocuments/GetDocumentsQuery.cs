using EBVL.Shared.Dto.Modules.MasterData.Documents;

namespace EBVL.BackEnd.Logics.Modules.MasterData.Documents.GetDocuments;

[AuthorizeRequest]
public sealed record GetDocumentsQuery : IRequest<GetDocumentsResponse>;

public sealed class GetDocumentsHandler(IDatabaseService db) : IRequestHandler<GetDocumentsQuery, GetDocumentsResponse>
{
    public async Task<GetDocumentsResponse> Handle(GetDocumentsQuery request, CancellationToken cancellationToken)
    {
        var items = await db.DocumentRequirementSets.AsNoTracking().Where(x => !x.IsDeleted)
            .Where(x => !db.DocumentRequirementSets.Any(other => !other.IsDeleted && other.DocumentRequirementSetSeriesId == x.DocumentRequirementSetSeriesId && other.Version > x.Version))
            .OrderBy(x => x.BusinessProcess).Select(x => new DocumentRequirementSetListItem(x.Id, x.DocumentRequirementSetSeriesId, x.BusinessProcess, x.Version, x.Status, x.Modified ?? x.Created, x.Requirements.Count(r => !r.IsDeleted), Convert.ToBase64String(x.RowVersion))).ToListAsync(cancellationToken);
        return new() { Items = items };
    }
}
