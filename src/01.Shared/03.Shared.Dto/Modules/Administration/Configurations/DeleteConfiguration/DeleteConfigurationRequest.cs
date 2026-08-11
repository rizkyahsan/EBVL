namespace EBVL.Shared.Dto.Modules.Administration.Configurations.DeleteConfiguration;

public record DeleteConfigurationRequest
{
    public required Guid ConfigurationId { get; init; }
}

public sealed class DeleteConfigurationRequestValidator : AbstractValidatorBase<DeleteConfigurationRequest>
{
    public DeleteConfigurationRequestValidator()
    {
        _ = RuleFor(x => x.ConfigurationId)
            .NotEmpty();
    }
}
