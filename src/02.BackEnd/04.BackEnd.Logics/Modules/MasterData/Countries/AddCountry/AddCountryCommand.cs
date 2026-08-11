using EBVL.Shared.Dto.Modules.MasterData;
using EBVL.Shared.Dto.Modules.MasterData.Countries.AddCountry;

namespace EBVL.BackEnd.Logics.Modules.MasterData.Countries.AddCountry;

[AuthorizeRequestByPermission(Permissions.MasterDataCountriesWrite)]
public sealed record AddCountryCommand : AddCountryRequest, IRequest<AddCountryResponse>
{
}

public sealed class AddCountryCommandValidator : AbstractValidatorBase<AddCountryCommand>
{
    public AddCountryCommandValidator()
    {
        Include(new AddCountryRequestValidator());
    }
}

public sealed class AddCountryCommandHandler(IDatabaseService databaseService)
    : IRequestHandler<AddCountryCommand, AddCountryResponse>
{
    public async Task<AddCountryResponse> Handle(AddCountryCommand request, CancellationToken cancellationToken)
    {
        var anyCountryWithTheSameName = await databaseService.Countries
            .Where(x => !x.IsDeleted && x.Name == request.Name)
            .AnyAsync(cancellationToken);

        if (anyCountryWithTheSameName)
        {
            throw ExceptionFor.EntityAlreadyExists(CountriesDisplayTextFor.Country, CommonDisplayTextFor.Name, request.Name);
        }

        var anyCountryWithTheSameCode = await databaseService.Countries
            .Where(x => !x.IsDeleted && x.Code == request.Code)
            .AnyAsync(cancellationToken);

        if (anyCountryWithTheSameCode)
        {
            throw ExceptionFor.EntityAlreadyExists(CountriesDisplayTextFor.Country, CommonDisplayTextFor.Code, request.Code);
        }

        var country = new Country
        {
            Name = request.Name,
            Code = request.Code
        };

        _ = await databaseService.Countries.AddAsync(country, cancellationToken);
        _ = await databaseService.SaveAsync(nameof(AddCountry), cancellationToken);

        return new AddCountryResponse
        {
            Item = new CountryItem
            {
                Id = country.Id
            }
        };
    }
}
