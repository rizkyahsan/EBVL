using EBVL.BackEnd.Logics.Modules.Main.VendorRegistrations.DeleteVendorRegistrationDocument;
using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.Questionnaires;

namespace EBVL.BackEnd.WebApi.Modules.Main.VendorRegistrations.DeleteVendorRegistrationDocument;

public sealed class DeleteVendorRegistrationDocumentEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app.MapDelete(QuestionnaireRuntimeRoutes.Document, Handle).AllowAnonymous().WithTags(RouteConfig.Tag).WithName("VendorQuestionnaire.DeleteDocument");
    }

    private static async Task<IResult> Handle(Guid registrationId, Guid documentId, [FromQuery] string resumeToken, ISender sender, CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteVendorRegistrationDocumentCommand(registrationId, documentId, resumeToken), cancellationToken);
        return Results.NoContent();
    }
}
