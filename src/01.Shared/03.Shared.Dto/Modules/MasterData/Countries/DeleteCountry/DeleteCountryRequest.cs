namespace EBVL.Shared.Dto.Modules.MasterData.Countries.DeleteCountry;

public record DeleteCountryRequest
{
    public required Guid CountryId { get; init; }
}

public sealed class DeleteCountryRequestValidator : AbstractValidatorBase<DeleteCountryRequest>
{
    public DeleteCountryRequestValidator()
    {
        _ = RuleFor(x => x.CountryId)
            .NotEmpty();
    }
}
