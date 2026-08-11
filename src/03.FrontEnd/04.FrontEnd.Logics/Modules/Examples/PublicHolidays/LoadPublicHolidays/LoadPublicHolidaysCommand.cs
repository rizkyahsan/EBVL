using EBVL.Shared.Dto.Modules.Examples.PublicHolidays.LoadPublicHolidays;

namespace EBVL.FrontEnd.Logics.Modules.Examples.PublicHolidays.LoadPublicHolidays;

public sealed record LoadPublicHolidaysCommand : LoadPublicHolidaysRequest, IRequest<LoadPublicHolidaysResponse>
{
}

public sealed class LoadPublicHolidaysCommandValidator : AbstractValidator<LoadPublicHolidaysCommand>
{
    public LoadPublicHolidaysCommandValidator()
    {
        Include(new LoadPublicHolidaysRequestValidator());
    }
}

public sealed class LoadPublicHolidaysCommandHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<LoadPublicHolidaysCommand, LoadPublicHolidaysResponse>
{
    public async Task<LoadPublicHolidaysResponse> Handle(LoadPublicHolidaysCommand request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(LoadPublicHolidaysRoute.ResourceUri, Method.Post);
        _ = restRequest.AddBody(request);

        return await backEndApiService.SendRequestAsync<LoadPublicHolidaysResponse>(restRequest, cancellationToken);
    }
}
