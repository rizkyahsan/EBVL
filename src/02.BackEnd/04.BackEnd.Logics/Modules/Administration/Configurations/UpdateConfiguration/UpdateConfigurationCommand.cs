using EBVL.Shared.Dto.Modules.Administration.Configurations.UpdateConfiguration;

namespace EBVL.BackEnd.Logics.Modules.Administration.Configurations.UpdateConfiguration;

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

public sealed class UpdateConfigurationCommandHandler(IDatabaseService databaseService)
    : IRequestHandler<UpdateConfigurationCommand>
{
    public async Task Handle(UpdateConfigurationCommand request, CancellationToken cancellationToken)
    {
        var configuration = await databaseService.Configurations
            .Where(x => !x.IsDeleted && x.Id == request.ConfigurationId)
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw ExceptionFor.EntityNotFound(ConfigurationsDisplayTextFor.Configuration, CommonDisplayTextFor.Id, request.ConfigurationId);

        var anyOtherConfigurationWithTheSameKey = await databaseService.Configurations
            .Where(x => !x.IsDeleted && x.Id != request.ConfigurationId && x.Key == request.Key)
            .AnyAsync(cancellationToken);

        if (anyOtherConfigurationWithTheSameKey)
        {
            throw ExceptionFor.EntityAlreadyExists(ConfigurationsDisplayTextFor.Configuration, ConfigurationsDisplayTextFor.Key, request.Key);
        }

        configuration.Key = request.Key;
        configuration.Value = request.Value;

        _ = await databaseService.SaveAsync(nameof(UpdateConfiguration), cancellationToken);
    }
}
