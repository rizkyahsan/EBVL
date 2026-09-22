using EBVL.BackEnd.Logics.Modules.Main.VendorRegistrations.UploadQuestionnaireFile;
using EBVL.Shared.Dto.Common.FileStorages;
using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.Questionnaires;

namespace EBVL.BackEnd.WebApi.Modules.Main.VendorRegistrations.UploadQuestionnaireFile;

public sealed class UploadQuestionnaireFileEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app.MapPost(QuestionnaireRuntimeRoutes.Files, Handle).AllowAnonymous().DisableAntiforgery().WithTags(RouteConfig.Tag).WithName("VendorQuestionnaire.UploadFile");
    }

    [RequestSizeLimit((50L * 1024 * 1024) + (1024 * 1024))]
    private static async Task<IResult> Handle(Guid registrationId, Guid questionId, [FromForm] string resumeToken, IFormFile file, ISender sender, CancellationToken cancellationToken)
    {
        if (file.Length > 50L * 1024 * 1024)
        {
            return Results.BadRequest("PDF files cannot exceed 50 MB.");
        }

        var response = await sender.Send(new UploadQuestionnaireFileCommand(registrationId, questionId, resumeToken, new FileItem { FileName = Path.GetFileName(file.FileName), ContentType = file.ContentType, FileContent = await file.ToBytesAsync(cancellationToken) }), cancellationToken);
        return Results.Ok(response);
    }
}
