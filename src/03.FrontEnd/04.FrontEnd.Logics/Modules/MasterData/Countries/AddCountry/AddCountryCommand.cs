using EBVL.Shared.Dto.Modules.MasterData.Countries.AddCountry;

namespace EBVL.FrontEnd.Logics.Modules.MasterData.Countries.AddCountry;

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

public sealed class AddCountryCommandHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<AddCountryCommand, AddCountryResponse>
{
    public async Task<AddCountryResponse> Handle(AddCountryCommand request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(AddCountryRoute.ResourceUri, Method.Post);
        _ = restRequest.AddJsonBody(request);

        return await backEndApiService.SendRequestAsync<AddCountryResponse>(restRequest, cancellationToken);
    }
}
