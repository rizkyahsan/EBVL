using EBVL.BackEnd.Logics.Modules.Examples.PublicHolidays.LoadPublicHolidays;
using EBVL.Shared.Dto.Modules.Examples.PublicHolidays;
using EBVL.Shared.Dto.Modules.Examples.PublicHolidays.LoadPublicHolidays;

namespace EBVL.BackEnd.WebApi.Modules.Examples.PublicHolidays.LoadPublicHolidays;

public sealed class LoadPublicHolidaysEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapPost(LoadPublicHolidaysRoute.Pattern, Handle)
            .AllowAnonymous()
            .WithTags(RouteConfig.Tag)
            .WithName(LoadPublicHolidaysRoute.Name)
            .WithDescription(LoadPublicHolidaysRoute.Description)
            .Produces<LoadPublicHolidaysResponse>();
    }

    private static async Task<IResult> Handle(
        LoadPublicHolidaysCommand command,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var response = await sender.Send(command, cancellationToken);

        return Results.Ok(response);
    }
}
