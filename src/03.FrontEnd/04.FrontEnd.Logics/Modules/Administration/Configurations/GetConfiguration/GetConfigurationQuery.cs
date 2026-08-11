using EBVL.Shared.Dto.Modules.Administration.Configurations.GetConfiguration;

namespace EBVL.FrontEnd.Logics.Modules.Administration.Configurations.GetConfiguration;

public sealed record GetConfigurationQuery : GetConfigurationRequest, IRequest<GetConfigurationResponse>
{
}

public sealed class GetConfigurationQueryValidator : AbstractValidatorBase<GetConfigurationQuery>
{
    public GetConfigurationQueryValidator()
    {
        Include(new GetConfigurationRequestValidator());
    }
}

public sealed class GetConfigurationQueryHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<GetConfigurationQuery, GetConfigurationResponse>
{
    public async Task<GetConfigurationResponse> Handle(GetConfigurationQuery request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(GetConfigurationRoute.ResourceUri(request.ConfigurationId), Method.Get);

        return await backEndApiService.SendRequestAsync<GetConfigurationResponse>(restRequest, cancellationToken);
    }
}
