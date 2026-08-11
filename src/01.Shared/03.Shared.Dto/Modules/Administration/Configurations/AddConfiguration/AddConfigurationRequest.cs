namespace EBVL.Shared.Dto.Modules.Administration.Configurations.AddConfiguration;

public record AddConfigurationRequest
{
    public required string Key { get; set; }
    public required string Value { get; set; }
}

public sealed class AddConfigurationRequestValidator : AbstractValidatorBase<AddConfigurationRequest>
{
    public AddConfigurationRequestValidator()
    {
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
