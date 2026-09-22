using EBVL.BackEnd.Logics.Modules.Main.VendorRegistrations.UpdateVendorRegistrationProfile;
using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.Questionnaires;

namespace EBVL.BackEnd.WebApi.Modules.Main.VendorRegistrations.UpdateVendorRegistrationProfile;

public sealed class UpdateVendorRegistrationProfileEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app.MapPut(QuestionnaireRuntimeRoutes.Profile, Handle).AllowAnonymous().WithTags(RouteConfig.Tag).WithName("VendorQuestionnaire.UpdateProfile");
    }

    private static async Task<IResult> Handle(Guid registrationId, UpdateVendorRegistrationProfileRequest body, ISender sender, CancellationToken cancellationToken)
    {
        return Results.Ok(await sender.Send(new UpdateVendorRegistrationProfileCommand(registrationId, body), cancellationToken));
    }
}
