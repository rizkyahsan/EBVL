using EBVL.Shared.Dto.Modules.Log.LogTransactions.GetLogTransactions;

namespace EBVL.BackEnd.Logics.Modules.Log.LogTransactions.GetLogTransactions;

[AuthorizeRequest]
public sealed record GetLogTransactionsQuery : GetLogTransactionsRequest, IRequest<GetLogTransactionsResponse> { }

public sealed class GetLogTransactionsQueryHandler(IDatabaseService databaseService)
    : IRequestHandler<GetLogTransactionsQuery, GetLogTransactionsResponse>
{
    public async Task<GetLogTransactionsResponse> Handle(GetLogTransactionsQuery request, CancellationToken cancellationToken)
    {
        var logTransactions = await databaseService.LogTransactions
            .AsNoTracking()
            .Where(x => request.ProjectId == Guid.Empty || x.ProjectId == request.ProjectId)
            .Where(x => request.ProjectStageId == null || x.ProjectStageId == request.ProjectStageId)
            .Where(x => request.ProjectLenderId == null || x.ProjectId == request.ProjectLenderId)
            .OrderByDescending(x => x.Created)
            .Select(x => new LogTransactionItem
            {
                Id = x.Id,
                ProjectId = x.ProjectId,
                ProjectStageId = x.ProjectStageId,
                ProjectLenderId = x.ProjectLenderId,
                Action = x.Action,
                Role = x.Role,
                Created = x.Created,
                CreatedBy = x.CreatedBy
            })
            .ToListAsync(cancellationToken);

        return new GetLogTransactionsResponse
        {
            Items = logTransactions
        };
    }
}
