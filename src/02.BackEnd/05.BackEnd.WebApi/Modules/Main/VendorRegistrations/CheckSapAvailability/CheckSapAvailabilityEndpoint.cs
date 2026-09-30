using EBVL.BackEnd.Logics.Modules.Main.VendorRegistrations.CheckSapAvailability;
using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.Questionnaires;

namespace EBVL.BackEnd.WebApi.Modules.Main.VendorRegistrations.CheckSapAvailability;

public sealed class CheckSapAvailabilityEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app.MapPost(CheckSapAvailabilityRoute.Pattern, Handle).AllowAnonymous().WithTags(RouteConfig.Tag).WithName("VendorQuestionnaire.CheckSapAvailability");
    }

    private static async Task<IResult> Handle(SapAvailabilityRequest request, ISender sender, CancellationToken cancellationToken)
    {
        return Results.Ok(await sender.Send(new CheckSapAvailabilityQuery(request.SapVendorNumber), cancellationToken));
    }
}
