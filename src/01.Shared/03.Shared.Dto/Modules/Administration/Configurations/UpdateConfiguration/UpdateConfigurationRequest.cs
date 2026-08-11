namespace EBVL.Shared.Dto.Modules.Administration.Configurations.UpdateConfiguration;

public record UpdateConfigurationRequest
{
    public required Guid ConfigurationId { get; init; }
    public required string Key { get; set; }
    public required string Value { get; set; }
}

public sealed class UpdateConfigurationRequestValidator : AbstractValidatorBase<UpdateConfigurationRequest>
{
    public UpdateConfigurationRequestValidator()
    {
        _ = RuleFor(x => x.ConfigurationId)
            .NotEmpty();

        _ = RuleFor(x => x.Key)
            .NotEmpty()
            .MinimumLength(ConfigurationsMinimumLengthFor.Key)
            .MaximumLength(ConfigurationsMaximumLengthFor.Key);

        _ = RuleFor(x => x.Value)
            .NotEmpty()
            .MinimumLength(ConfigurationsMinimumLengthFor.Value)
            .MaximumLength(ConfigurationsMaximumLengthFor.Value);
    }
}
