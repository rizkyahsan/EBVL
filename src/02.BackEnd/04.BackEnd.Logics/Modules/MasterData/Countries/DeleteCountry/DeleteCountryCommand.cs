using EBVL.Shared.Dto.Modules.MasterData.Countries.DeleteCountry;

namespace EBVL.BackEnd.Logics.Modules.MasterData.Countries.DeleteCountry;

[AuthorizeRequest]
public sealed record DeleteCountryCommand : DeleteCountryRequest, IRequest { }

public sealed class DeleteCountryCommandValidator : AbstractValidatorBase<DeleteCountryCommand>
{
    public DeleteCountryCommandValidator()
    {
        Include(new DeleteCountryRequestValidator());
    }
}

public sealed class DeleteCountryCommandHandler(IDatabaseService databaseService)
    : IRequestHandler<DeleteCountryCommand>
{
    public async Task Handle(DeleteCountryCommand request, CancellationToken cancellationToken)
    {
        var country = await databaseService.Countries
            .Where(x => !x.IsDeleted && x.Id == request.CountryId)
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw ExceptionFor.EntityNotFound(CountriesDisplayTextFor.Country, CommonDisplayTextFor.Id, request.CountryId);

        country.IsDeleted = true;

        _ = await databaseService.SaveAsync(nameof(DeleteCountry), cancellationToken);
    }
}
