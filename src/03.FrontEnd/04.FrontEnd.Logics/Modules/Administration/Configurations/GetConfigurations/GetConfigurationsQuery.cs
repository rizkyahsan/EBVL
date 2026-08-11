using EBVL.Shared.Dto.Modules.Administration.Configurations.GetConfigurations;

namespace EBVL.FrontEnd.Logics.Modules.Administration.Configurations.GetConfigurations;

public sealed record GetConfigurationsQuery : IRequest<GetConfigurationsResponse>
{
}

public sealed class GetConfigurationsQueryHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<GetConfigurationsQuery, GetConfigurationsResponse>
{
    public async Task<GetConfigurationsResponse> Handle(GetConfigurationsQuery request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(GetConfigurationsRoute.Pattern, Method.Get);

        return await backEndApiService.SendRequestAsync<GetConfigurationsResponse>(restRequest, cancellationToken);
    }
}
