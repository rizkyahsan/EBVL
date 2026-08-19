namespace EBVL.Shared.Dto.Modules.MasterData.Countries.AddCountry;

public record AddCountryRequest
{
    public required string Name { get; set; }
    public required string Code { get; set; }
    public required string PhoneCode { get; set; }
    public required string CurrencyCode { get; set; }
    public string Region { get; set; } = string.Empty;
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

        _ = RuleFor(x => x.PhoneCode)
            .NotEmpty();

        _ = RuleFor(x => x.CurrencyCode)
            .NotEmpty()
            .MinimumLength(CountriesMinimumLengthFor.CurrencyCode)
            .MaximumLength(CountriesMaximumLengthFor.CurrencyCode);
    }
}
