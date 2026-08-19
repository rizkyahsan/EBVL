using EBVL.Shared.Dto.Modules.MasterData.EmailTemplates.GetEmailTemplates;

namespace EBVL.BackEnd.Logics.Modules.MasterData.EmailTemplates.GetEmailTemplates;

public sealed record GetEmailTemplatesQuery : IRequest<GetEmailTemplatesResponse>
{
}

public sealed class GetEmailTemplatesQueryHandler(IDatabaseService databaseService)
    : IRequestHandler<GetEmailTemplatesQuery, GetEmailTemplatesResponse>
{
    public async Task<GetEmailTemplatesResponse> Handle(GetEmailTemplatesQuery request, CancellationToken cancellationToken)
    {
        var emailTemplates = await databaseService.EmailTemplates
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.Module).ThenBy(x => x.Action)
            .Select(x => new EmailTemplateItem
            {
                Id = x.Id,
                Module = x.Module,
                Action = x.Action,
                DefaultTo = x.DefaultTo,
                DefaultCc = x.DefaultCc,
                Subject = x.Subject,
                Content = x.Content,
            })
            .ToListAsync(cancellationToken);

        return new GetEmailTemplatesResponse
        {
            Items = emailTemplates
        };
    }
}
