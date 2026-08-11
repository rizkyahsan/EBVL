using EBVL.Shared.Dto.Modules.Administration;
using EBVL.Shared.Dto.Modules.Administration.Configurations.AddConfiguration;

namespace EBVL.BackEnd.Logics.Modules.Administration.Configurations.AddConfiguration;

[AuthorizeRequestByPermission(Permissions.AdministrationConfigurationsWrite)]
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

public sealed class AddConfigurationCommandHandler(IDatabaseService databaseService)
    : IRequestHandler<AddConfigurationCommand, AddConfigurationResponse>
{
    public async Task<AddConfigurationResponse> Handle(AddConfigurationCommand request, CancellationToken cancellationToken)
    {
        var anyConfigurationWithTheSameKey = await databaseService.Configurations
            .Where(x => x.Key == request.Key)
            .AnyAsync(cancellationToken);

        if (anyConfigurationWithTheSameKey)
        {
            throw ExceptionFor.EntityAlreadyExists(ConfigurationsDisplayTextFor.Configuration, ConfigurationsDisplayTextFor.Key, request.Key);
        }

        var configuration = new Configuration
        {
            Key = request.Key,
            Value = request.Value
        };

        _ = await databaseService.Configurations.AddAsync(configuration, cancellationToken);
        _ = await databaseService.SaveAsync(nameof(AddConfiguration), cancellationToken);

        return new AddConfigurationResponse
        {
            Item = new ConfigurationItem
            {
                Id = configuration.Id
            }
        };
    }
}
