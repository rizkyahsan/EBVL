using EBVL.Shared.Dto.Modules.MasterData.EmailTemplates.GetEmailTemplates;

namespace EBVL.FrontEnd.Logics.Modules.MasterData.EmailTemplates.GetEmailTemplates;

public sealed record GetEmailTemplatesQuery : IRequest<GetEmailTemplatesResponse>
{
}

public sealed class GetEmailTemplatesQueryHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<GetEmailTemplatesQuery, GetEmailTemplatesResponse>
{
    public async Task<GetEmailTemplatesResponse> Handle(GetEmailTemplatesQuery request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(GetEmailTemplatesRoute.ResourceUri, Method.Get);

        return await backEndApiService.SendRequestAsync<GetEmailTemplatesResponse>(restRequest, cancellationToken);
    }
}
