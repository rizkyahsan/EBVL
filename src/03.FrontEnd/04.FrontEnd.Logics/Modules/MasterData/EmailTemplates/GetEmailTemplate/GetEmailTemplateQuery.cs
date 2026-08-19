using EBVL.Shared.Dto.Modules.MasterData.EmailTemplates.GetEmailTemplate;

namespace EBVL.FrontEnd.Logics.Modules.MasterData.EmailTemplates.GetEmailTemplate;

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

public sealed class GetEmailTemplateQueryHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<GetEmailTemplateQuery, GetEmailTemplateResponse>
{
    public async Task<GetEmailTemplateResponse> Handle(GetEmailTemplateQuery request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(GetEmailTemplateRoute.ResourceUri(request.EmailTemplateId), Method.Get);

        return await backEndApiService.SendRequestAsync<GetEmailTemplateResponse>(restRequest, cancellationToken);
    }
}
