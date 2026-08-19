using EBVL.Shared.Dto.Modules.Log.LogTransactions.GetLogTransaction;

namespace EBVL.BackEnd.Logics.Modules.Log.LogTransactions.GetLogTransaction;

[AuthorizeRequest]
public sealed record GetLogTransactionQuery : GetLogTransactionRequest, IRequest<GetLogTransactionResponse> { }

public sealed class GetLogTransactionQueryValidator : AbstractValidatorBase<GetLogTransactionQuery>
{
    public GetLogTransactionQueryValidator()
    {
        Include(new GetLogTransactionRequestValidator());
    }
}

public sealed class GetLogTransactionQueryHandler(IDatabaseService databaseService)
    : IRequestHandler<GetLogTransactionQuery, GetLogTransactionResponse>
{
    public async Task<GetLogTransactionResponse> Handle(GetLogTransactionQuery request, CancellationToken cancellationToken)
    {
        var audits = await databaseService.Audits
            .Where(audit => audit.EntityName == nameof(LogTransaction) && audit.EntityId == request.Id)
            .OrderBy(audit => audit.Created)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var logTransaction = await databaseService.LogTransactions
            .Where(x => !x.IsDeleted && x.Id == request.Id)
            .Select(x => new LogTransactionItem
            {
                Id = x.Id,
                ProjectId = x.ProjectId,
                ProjectStageId = x.ProjectStageId,
                ProjectLenderId = x.ProjectLenderId,
                Action = x.Action,
                Role = x.Role,
                Created = x.Created,
                CreatedBy = x.CreatedBy,
                Modified = x.Modified,
                ModifiedBy = x.ModifiedBy,
                Audits = audits.ToAuditItems<AuditItem>()
            })
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw ExceptionFor.EntityNotFound(LogTransactionsDisplayTextFor.LogTransaction, CommonDisplayTextFor.Id, request.Id);

        return new GetLogTransactionResponse
        {
            Item = logTransaction
        };
    }
}
