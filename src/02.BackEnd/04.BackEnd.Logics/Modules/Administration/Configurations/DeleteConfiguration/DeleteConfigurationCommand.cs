using EBVL.Shared.Dto.Modules.Administration;
using EBVL.Shared.Dto.Modules.Administration.Configurations.DeleteConfiguration;

namespace EBVL.BackEnd.Logics.Modules.Administration.Configurations.DeleteConfiguration;

[AuthorizeRequestByPermission(Permissions.AdministrationConfigurationsWrite)]
public sealed record DeleteConfigurationCommand : DeleteConfigurationRequest, IRequest
{
}

public sealed class DeleteConfigurationCommandValidator : AbstractValidatorBase<DeleteConfigurationCommand>
{
    public DeleteConfigurationCommandValidator()
    {
        Include(new DeleteConfigurationRequestValidator());
    }
}

public sealed class DeleteConfigurationCommandHandler(IDatabaseService databaseService)
    : IRequestHandler<DeleteConfigurationCommand>
{
    public async Task Handle(DeleteConfigurationCommand request, CancellationToken cancellationToken)
    {
        var configuration = await databaseService.Configurations
            .Where(x => !x.IsDeleted && x.Id == request.ConfigurationId)
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw ExceptionFor.EntityNotFound(ConfigurationsDisplayTextFor.Configuration, CommonDisplayTextFor.Id, request.ConfigurationId);

        configuration.IsDeleted = true;

        _ = await databaseService.SaveAsync(nameof(DeleteConfiguration), cancellationToken);
    }
}
