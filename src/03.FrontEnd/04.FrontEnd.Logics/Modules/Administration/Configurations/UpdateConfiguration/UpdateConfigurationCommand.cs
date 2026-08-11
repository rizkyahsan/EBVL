using EBVL.Shared.Dto.Modules.Administration.Configurations.UpdateConfiguration;

namespace EBVL.FrontEnd.Logics.Modules.Administration.Configurations.UpdateConfiguration;

public sealed record UpdateConfigurationCommand : UpdateConfigurationRequest, IRequest
{
}

public sealed class UpdateConfigurationCommandValidator : AbstractValidatorBase<UpdateConfigurationCommand>
{
    public UpdateConfigurationCommandValidator()
    {
        Include(new UpdateConfigurationRequestValidator());
    }
}

public sealed class UpdateConfigurationCommandHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<UpdateConfigurationCommand>
{
    public async Task Handle(UpdateConfigurationCommand request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(UpdateConfigurationRoute.ResourceUri(request.ConfigurationId), Method.Patch);
        _ = restRequest.AddJsonBody(request);

        await backEndApiService.SendRequestAsync(restRequest, cancellationToken);
    }
}
