namespace EBVL.Shared.Dto.Modules.Examples.PublicHolidays.LoadPublicHolidays;

public record LoadPublicHolidaysRequest
{
    public required int Year { get; set; }
    public required string CountryCode { get; set; }
}

public sealed class LoadPublicHolidaysRequestValidator : AbstractValidator<LoadPublicHolidaysRequest>
{
    public LoadPublicHolidaysRequestValidator()
    {

        _ = RuleFor(x => x.Year)
            .NotEmpty()
            .InclusiveBetween(PublicHolidaysMinimumValueFor.Year, DateTime.Now.Year);
    }
}
