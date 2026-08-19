using EBVL.Shared.Dto.Modules.Log.LogEmails.GetLogEmail;

namespace EBVL.BackEnd.Logics.Modules.Log.LogEmails.GetLogEmail;

[AuthorizeRequest]
public sealed record GetLogEmailQuery : GetLogEmailRequest, IRequest<GetLogEmailResponse> { }

public sealed class GetLogEmailQueryValidator : AbstractValidatorBase<GetLogEmailQuery>
{
    public GetLogEmailQueryValidator()
    {
        Include(new GetLogEmailRequestValidator());
    }
}

public sealed class GetLogEmailQueryHandler(IDatabaseService databaseService)
    : IRequestHandler<GetLogEmailQuery, GetLogEmailResponse>
{
    public async Task<GetLogEmailResponse> Handle(GetLogEmailQuery request, CancellationToken cancellationToken)
    {
        var audits = await databaseService.Audits
            .Where(audit => audit.EntityName == nameof(LogEmail) && audit.EntityId == request.Id)
            .OrderBy(audit => audit.Created)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var logEmail = await databaseService.LogEmails
            .Where(x => !x.IsDeleted && x.Id == request.Id)
            .Select(x => new LogEmailItem
            {
                Id = x.Id,
                Module = x.Module,
                Action = x.Action,
                Provider = x.Provider,
                To = x.To,
                Cc = x.Cc,
                Subject = x.Subject,
                Content = x.Content,
                SentAt = x.SentAt,
                IsSuccessful = x.IsSuccessful,
                RetryCount = x.RetryCount,
                Message = x.Message,
                ExternalMessageId = x.ExternalMessageId,
                CorrelationId = x.CorrelationId,
                Created = x.Created,
                CreatedBy = x.CreatedBy,
                Modified = x.Modified,
                ModifiedBy = x.ModifiedBy,
                Audits = audits.ToAuditItems<AuditItem>()
            })
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw ExceptionFor.EntityNotFound(LogEmailsDisplayTextFor.LogEmail, CommonDisplayTextFor.Id, request.Id);

        return new GetLogEmailResponse
        {
            Item = logEmail
        };
    }
}
