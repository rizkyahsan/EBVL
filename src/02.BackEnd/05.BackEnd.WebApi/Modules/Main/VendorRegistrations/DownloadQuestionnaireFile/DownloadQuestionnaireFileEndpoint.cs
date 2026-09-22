using EBVL.BackEnd.Logics.Modules.Main.VendorRegistrations.DownloadQuestionnaireFile;
using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.Questionnaires;

namespace EBVL.BackEnd.WebApi.Modules.Main.VendorRegistrations.DownloadQuestionnaireFile;

public sealed class DownloadQuestionnaireFileEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app.MapPost(QuestionnaireRuntimeRoutes.File + "/download", Handle).AllowAnonymous().WithTags(RouteConfig.Tag).WithName("VendorQuestionnaire.DownloadFile");
    }

    private static async Task<IResult> Handle(Guid registrationId, Guid fileId, FileAuthorizationRequest body, ISender sender, CancellationToken cancellationToken)
    {
        return Results.Ok(await sender.Send(new DownloadQuestionnaireFileQuery(registrationId, fileId, body.ResumeToken), cancellationToken));
    }
}
