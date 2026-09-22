using EBVL.BackEnd.Logics.Modules.Main.VendorRegistrations.UploadVendorRegistrationDocument;
using EBVL.Shared.Dto.Common.FileStorages;
using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.Questionnaires;

namespace EBVL.BackEnd.WebApi.Modules.Main.VendorRegistrations.UploadVendorRegistrationDocument;

public sealed class UploadVendorRegistrationDocumentEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app.MapPost(QuestionnaireRuntimeRoutes.Documents, Handle).AllowAnonymous().DisableAntiforgery().WithTags(RouteConfig.Tag).WithName("VendorQuestionnaire.UploadDocument");
    }

    [RequestSizeLimit((100L * 1024 * 1024) + (1024 * 1024))]
    private static async Task<IResult> Handle(Guid registrationId, string definitionKey, [FromForm] string resumeToken, IFormFile file, ISender sender, CancellationToken cancellationToken)
    {
        if (file.Length > 100L * 1024 * 1024)
        {
            return Results.BadRequest("PDF files cannot exceed 100 MB.");
        }

        var response = await sender.Send(new UploadVendorRegistrationDocumentCommand(registrationId, definitionKey, resumeToken, new FileItem { FileName = Path.GetFileName(file.FileName), ContentType = file.ContentType, FileContent = await file.ToBytesAsync(cancellationToken) }), cancellationToken);
        return Results.Ok(response);
    }
}
