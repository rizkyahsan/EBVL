using EBVL.BackEnd.Logics.Modules.Examples.PublicHolidays.GetPublicHolidays;
using EBVL.Shared.Dto.Modules.Examples.PublicHolidays;
using EBVL.Shared.Dto.Modules.Examples.PublicHolidays.GetPublicHolidays;

namespace EBVL.BackEnd.WebApi.Modules.Examples.PublicHolidays.GetPublicHolidays;

public sealed class GetPublicHolidaysEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapGet(GetPublicHolidaysRoute.Pattern, Handle)
            .AllowAnonymous()
            .WithTags(RouteConfig.Tag)
            .WithName(GetPublicHolidaysRoute.Name)
            .WithDescription(GetPublicHolidaysRoute.Description)
            .Produces<GetPublicHolidaysResponse>();
    }

    private static async Task<IResult> Handle(
        ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new GetPublicHolidaysQuery();
        var response = await sender.Send(query, cancellationToken);

        return Results.Ok(response);
    }
}
