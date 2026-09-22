using EBVL.BackEnd.Logics.Modules.Main.VendorRegistrations.DownloadVendorRegistrationDocument;
using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.Questionnaires;

namespace EBVL.BackEnd.WebApi.Modules.Main.VendorRegistrations.DownloadVendorRegistrationDocument;

public sealed class DownloadVendorRegistrationDocumentEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app.MapPost(QuestionnaireRuntimeRoutes.Document + "/download", Handle).AllowAnonymous().WithTags(RouteConfig.Tag).WithName("VendorQuestionnaire.DownloadDocument");
    }

    private static async Task<IResult> Handle(Guid registrationId, Guid documentId, FileAuthorizationRequest body, ISender sender, CancellationToken cancellationToken)
    {
        return Results.Ok(await sender.Send(new DownloadVendorRegistrationDocumentQuery(registrationId, documentId, body.ResumeToken), cancellationToken));
    }
}
