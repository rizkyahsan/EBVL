using EBVL.Shared.Dto.Modules.MasterData.EmailTemplates.GetEmailTemplate;

namespace EBVL.BackEnd.Logics.Modules.MasterData.EmailTemplates.GetEmailTemplate;

public sealed record GetEmailTemplateQuery : GetEmailTemplateRequest, IRequest<GetEmailTemplateResponse>
{
}

public sealed class GetEmailTemplateQueryValidator : AbstractValidatorBase<GetEmailTemplateQuery>
{
    public GetEmailTemplateQueryValidator()
    {
        Include(new GetEmailTemplateRequestValidator());
    }
}

public sealed class GetEmailTemplateQueryHandler(IDatabaseService databaseService)
    : IRequestHandler<GetEmailTemplateQuery, GetEmailTemplateResponse>
{
    public async Task<GetEmailTemplateResponse> Handle(GetEmailTemplateQuery request, CancellationToken cancellationToken)
    {
        var audits = await databaseService.Audits
            .Where(audit => audit.EntityName == nameof(EmailTemplate) && audit.EntityId == request.EmailTemplateId)
            .OrderBy(audit => audit.Created)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var emailTemplate = await databaseService.EmailTemplates
            .Where(x => !x.IsDeleted && x.Id == request.EmailTemplateId)
            .Select(x => new EmailTemplateItem
            {
                Id = x.Id,
                Module = x.Module,
                Action = x.Action,
                To = x.DefaultTo,
                Cc = x.DefaultCc,
                Subject = x.Subject,
                Content = x.Content,
                Created = x.Created,
                CreatedBy = x.CreatedBy,
                Modified = x.Modified,
                ModifiedBy = x.ModifiedBy,
                Audits = audits.ToAuditItems<AuditItem>()
            })
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw ExceptionFor.EntityNotFound(EmailTemplatesDisplayTextFor.EmailTemplate, CommonDisplayTextFor.Id, request.EmailTemplateId);

        return new GetEmailTemplateResponse
        {
            Item = emailTemplate
        };
    }
}
