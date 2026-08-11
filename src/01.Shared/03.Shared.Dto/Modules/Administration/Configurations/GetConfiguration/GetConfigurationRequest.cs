namespace EBVL.Shared.Dto.Modules.Administration.Configurations.GetConfiguration;

public record GetConfigurationRequest
{
    public required Guid ConfigurationId { get; init; }
}

public sealed class GetConfigurationRequestValidator : AbstractValidatorBase<GetConfigurationRequest>
{
    public GetConfigurationRequestValidator()
    {
        _ = RuleFor(x => x.ConfigurationId)
            .NotEmpty();
    }
}
