using EBVL.Shared.Dto.Modules.MasterData;
using EBVL.Shared.Dto.Modules.MasterData.Countries.UpdateCountry;

namespace EBVL.BackEnd.Logics.Modules.MasterData.Countries.UpdateCountry;

[AuthorizeRequestByPermission(Permissions.MasterDataCountriesWrite)]
public sealed record UpdateCountryCommand : UpdateCountryRequest, IRequest
{
}

public sealed class UpdateCountryCommandValidator : AbstractValidatorBase<UpdateCountryCommand>
{
    public UpdateCountryCommandValidator()
    {
        Include(new UpdateCountryRequestValidator());
    }
}

public sealed class UpdateCountryCommandHandler(IDatabaseService databaseService)
    : IRequestHandler<UpdateCountryCommand>
{
    public async Task Handle(UpdateCountryCommand request, CancellationToken cancellationToken)
    {
        var country = await databaseService.Countries
            .Where(x => !x.IsDeleted && x.Id == request.CountryId)
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw ExceptionFor.EntityNotFound(CountriesDisplayTextFor.Country, CommonDisplayTextFor.Id, request.CountryId);

        var anyOtherCountryWithTheSameName = await databaseService.Countries
            .Where(x => !x.IsDeleted && x.Id != request.CountryId && x.Name == request.Name)
            .AnyAsync(cancellationToken);

        if (anyOtherCountryWithTheSameName)
        {
            throw ExceptionFor.EntityAlreadyExists(CountriesDisplayTextFor.Country, CommonDisplayTextFor.Name, request.Name);
        }

        var anyOtherCountryWithTheSameCode = await databaseService.Countries
            .Where(x => !x.IsDeleted && x.Id != request.CountryId && x.Code == request.Code)
            .AnyAsync(cancellationToken);

        if (anyOtherCountryWithTheSameCode)
        {
            throw ExceptionFor.EntityAlreadyExists(CountriesDisplayTextFor.Country, CommonDisplayTextFor.Code, request.Code);
        }

        country.Name = request.Name;
        country.Code = request.Code;

        _ = await databaseService.SaveAsync(nameof(UpdateCountry), cancellationToken);
    }
}
