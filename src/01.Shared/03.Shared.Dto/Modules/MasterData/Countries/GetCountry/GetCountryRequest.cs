namespace EBVL.Shared.Dto.Modules.MasterData.Countries.GetCountry;

public record GetCountryRequest
{
    public required Guid CountryId { get; init; }
}

public sealed class GetCountryRequestValidator : AbstractValidatorBase<GetCountryRequest>
{
    public GetCountryRequestValidator()
    {
        _ = RuleFor(x => x.CountryId)
            .NotEmpty();
    }
}
