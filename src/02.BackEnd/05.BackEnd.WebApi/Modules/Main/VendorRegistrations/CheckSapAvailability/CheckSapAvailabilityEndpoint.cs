using EBVL.BackEnd.Logics.Modules.Main.VendorRegistrations.CheckSapAvailability;
using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.Questionnaires;

namespace EBVL.BackEnd.WebApi.Modules.Main.VendorRegistrations.CheckSapAvailability;

public sealed class CheckSapAvailabilityEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app.MapGet(QuestionnaireRuntimeRoutes.SapAvailability, Handle).AllowAnonymous().WithTags(RouteConfig.Tag).WithName("VendorQuestionnaire.CheckSapAvailability");
    }

    private static async Task<IResult> Handle(string sapVendorNumber, Guid? registrationId, ISender sender, CancellationToken cancellationToken)
    {
        return Results.Ok(await sender.Send(new CheckSapAvailabilityQuery(sapVendorNumber, registrationId), cancellationToken));
    }
}
