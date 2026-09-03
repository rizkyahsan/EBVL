using EBVL.BackEnd.Logics.Modules.Main.VendorRegistrations.Questionnaires;
using EBVL.Shared.Dto.Common.FileStorages;
using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.PreRegistration;
using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.Questionnaires;

namespace EBVL.BackEnd.WebApi.Modules.Main.VendorRegistrations.Questionnaires;

public sealed class QuestionnaireRuntimeEndpoints : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        _ = app.MapPost(QuestionnaireRuntimeRoutes.Start, async (PreRegistrationRequest body, ISender sender, CancellationToken ct) => Results.Ok(await sender.Send(new StartQuestionnaireCommand(body), ct))).AllowAnonymous().WithTags(RouteConfig.Tag).WithName("VendorQuestionnaire.Start");
        _ = app.MapPost(QuestionnaireRuntimeRoutes.Get, async (Guid registrationId, ResumeQuestionnaireRequest body, ISender sender, CancellationToken ct) => Results.Ok(await sender.Send(new GetQuestionnaireRuntimeQuery(registrationId, body.ResumeToken), ct))).AllowAnonymous().WithTags(RouteConfig.Tag).WithName("VendorQuestionnaire.Get");
        _ = app.MapPut(QuestionnaireRuntimeRoutes.Answers, async (Guid registrationId, SaveAnswersRequest body, ISender sender, CancellationToken ct) => Results.Ok(await sender.Send(new SaveQuestionnaireAnswersCommand(registrationId, body), ct))).AllowAnonymous().WithTags(RouteConfig.Tag).WithName("VendorQuestionnaire.SaveAnswers");
        _ = app.MapPut(QuestionnaireRuntimeRoutes.Profile, async (Guid registrationId, UpdateVendorRegistrationProfileRequest body, ISender sender, CancellationToken ct) => Results.Ok(await sender.Send(new UpdateVendorRegistrationProfileCommand(registrationId, body), ct))).AllowAnonymous().WithTags(RouteConfig.Tag).WithName("VendorQuestionnaire.UpdateProfile");
        _ = app.MapPost(QuestionnaireRuntimeRoutes.Submit, async (Guid registrationId, SubmitQuestionnaireRequest body, ISender sender, CancellationToken ct) => Results.Ok(await sender.Send(new SubmitQuestionnaireCommand(registrationId, body), ct))).AllowAnonymous().WithTags(RouteConfig.Tag).WithName("VendorQuestionnaire.Submit");
        _ = app.MapPost(QuestionnaireRuntimeRoutes.Files, Upload).AllowAnonymous().DisableAntiforgery().WithTags(RouteConfig.Tag).WithName("VendorQuestionnaire.UploadFile");
        _ = app.MapPost(QuestionnaireRuntimeRoutes.Documents, UploadDocument).AllowAnonymous().DisableAntiforgery().WithTags(RouteConfig.Tag).WithName("VendorQuestionnaire.UploadDocument");
        _ = app.MapPost(QuestionnaireRuntimeRoutes.Document + "/download", async (Guid registrationId, Guid documentId, FileAuthorizationRequest body, ISender sender, CancellationToken ct) =>
        {
            var file = await sender.Send(new DownloadVendorRegistrationDocumentQuery(registrationId, documentId, body.ResumeToken), ct);
            return Results.File(file.Content, file.ContentType, file.FileName);
        }).AllowAnonymous().WithTags(RouteConfig.Tag).WithName("VendorQuestionnaire.DownloadDocument");
        _ = app.MapDelete(QuestionnaireRuntimeRoutes.Document, async (Guid registrationId, Guid documentId, [FromQuery] string resumeToken, ISender sender, CancellationToken ct) =>
        {
            await sender.Send(new DeleteVendorRegistrationDocumentCommand(registrationId, documentId, resumeToken), ct);
            return Results.NoContent();
        }).AllowAnonymous().WithTags(RouteConfig.Tag).WithName("VendorQuestionnaire.DeleteDocument");
        _ = app.MapPost(QuestionnaireRuntimeRoutes.File + "/download", async (Guid registrationId, Guid fileId, FileAuthorizationRequest body, ISender sender, CancellationToken ct) =>
        {
            var file = await sender.Send(new DownloadQuestionnaireFileQuery(registrationId, fileId, body.ResumeToken), ct);
            return Results.File(file.Content, file.ContentType, file.FileName);
        }).AllowAnonymous().WithTags(RouteConfig.Tag).WithName("VendorQuestionnaire.DownloadFile");
        return app.MapDelete(QuestionnaireRuntimeRoutes.File, async (Guid registrationId, Guid fileId, [FromQuery] string resumeToken, ISender sender, CancellationToken ct) =>
        {
            await sender.Send(new DeleteQuestionnaireFileCommand(registrationId, fileId, resumeToken), ct);
            return Results.NoContent();
        }).AllowAnonymous().WithTags(RouteConfig.Tag).WithName("VendorQuestionnaire.DeleteFile");
    }

    [RequestSizeLimit((50L * 1024 * 1024) + (1024 * 1024))]
    private static async Task<IResult> UploadDocument(Guid registrationId, string definitionKey, [FromForm] string resumeToken, IFormFile file, ISender sender, CancellationToken ct)
    {
        if (file.Length > 50L * 1024 * 1024)
        {
            return Results.BadRequest("PDF files cannot exceed 50 MB.");
        }

        var response = await sender.Send(new UploadVendorRegistrationDocumentCommand(registrationId, definitionKey, resumeToken, new FileItem { FileName = Path.GetFileName(file.FileName), ContentType = file.ContentType, FileContent = await file.ToBytesAsync(ct) }), ct);
        return Results.Ok(response);
    }

    [RequestSizeLimit((50L * 1024 * 1024) + (1024 * 1024))]
    private static async Task<IResult> Upload(Guid registrationId, Guid questionId, [FromForm] string resumeToken, IFormFile file, ISender sender, CancellationToken ct)
    {
        if (file.Length > 50L * 1024 * 1024)
        {
            return Results.BadRequest("PDF files cannot exceed 50 MB.");
        }

        var response = await sender.Send(new UploadQuestionnaireFileCommand(registrationId, questionId, resumeToken, new FileItem { FileName = Path.GetFileName(file.FileName), ContentType = file.ContentType, FileContent = await file.ToBytesAsync(ct) }), ct);
        return Results.Ok(response);
    }
}
