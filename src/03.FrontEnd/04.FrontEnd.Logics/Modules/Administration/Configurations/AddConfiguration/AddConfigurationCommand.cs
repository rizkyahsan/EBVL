using EBVL.Shared.Dto.Modules.Administration.Configurations.AddConfiguration;

namespace EBVL.FrontEnd.Logics.Modules.Administration.Configurations.AddConfiguration;

public sealed record AddConfigurationCommand : AddConfigurationRequest, IRequest<AddConfigurationResponse>
{
}

public sealed class AddConfigurationCommandValidator : AbstractValidatorBase<AddConfigurationCommand>
{
    public AddConfigurationCommandValidator()
    {
        Include(new AddConfigurationRequestValidator());
    }
}

public sealed class AddConfigurationCommandHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<AddConfigurationCommand, AddConfigurationResponse>
{
    public async Task<AddConfigurationResponse> Handle(AddConfigurationCommand request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(AddConfigurationRoute.ResourceUri, Method.Post);
        _ = restRequest.AddJsonBody(request);

        return await backEndApiService.SendRequestAsync<AddConfigurationResponse>(restRequest, cancellationToken);
    }
}
