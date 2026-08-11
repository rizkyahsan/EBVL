using EBVL.Shared.Dto.Modules.MasterData.Countries.UpdateCountry;

namespace EBVL.FrontEnd.Logics.Modules.MasterData.Countries.UpdateCountry;

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

public sealed class UpdateCountryCommandHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<UpdateCountryCommand>
{
    public async Task Handle(UpdateCountryCommand request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(UpdateCountryRoute.ResourceUri(request.CountryId), Method.Patch);
        _ = restRequest.AddJsonBody(request);

        await backEndApiService.SendRequestAsync(restRequest, cancellationToken);
    }
}
