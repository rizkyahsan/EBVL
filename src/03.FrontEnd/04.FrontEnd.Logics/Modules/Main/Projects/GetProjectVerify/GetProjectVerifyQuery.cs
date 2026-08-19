using EBVL.Shared.Dto.Modules.Main.Projects.GetProjectVerify;

namespace EBVL.FrontEnd.Logics.Modules.Main.Projects.GetProjectVerify;

public sealed record GetProjectVerifyQuery : GetProjectVerifyRequest, IRequest<GetProjectVerifyResponse> { }

public sealed class GetProjectVerifyQueryValidator : AbstractValidatorBase<GetProjectVerifyQuery>
{
    public GetProjectVerifyQueryValidator()
    {
        Include(new GetProjectVerifyRequestValidator());
    }
}

public sealed class GetProjectVerifyQueryHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<GetProjectVerifyQuery, GetProjectVerifyResponse>
{
    public async Task<GetProjectVerifyResponse> Handle(GetProjectVerifyQuery request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(GetProjectVerifyRoute.ResourceUri(request.Id), Method.Get);

        return await backEndApiService.SendRequestAsync<GetProjectVerifyResponse>(restRequest, cancellationToken);
    }
}
