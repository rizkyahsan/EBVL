namespace EBVL.Shared.Dto.Modules.MasterData.Countries.AddCountry;

public record AddCountryRequest
{
    public required string Name { get; set; }
    public required string Code { get; set; }
}

public sealed class AddCountryRequestValidator : AbstractValidatorBase<AddCountryRequest>
{
    public AddCountryRequestValidator()
    {
        _ = RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(CountriesMinimumLengthFor.Name)
            .MaximumLength(CountriesMaximumLengthFor.Name);

        _ = RuleFor(x => x.Code)
            .NotEmpty()
            .MinimumLength(CountriesMinimumLengthFor.Code)
            .MaximumLength(CountriesMaximumLengthFor.Code);
    }
}
