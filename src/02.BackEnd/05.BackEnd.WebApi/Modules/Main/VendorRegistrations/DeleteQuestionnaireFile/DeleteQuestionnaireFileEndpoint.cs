using EBVL.BackEnd.Logics.Modules.Main.VendorRegistrations.DeleteQuestionnaireFile;
using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.Questionnaires;

namespace EBVL.BackEnd.WebApi.Modules.Main.VendorRegistrations.DeleteQuestionnaireFile;

public sealed class DeleteQuestionnaireFileEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app.MapDelete(QuestionnaireRuntimeRoutes.File, Handle).AllowAnonymous().WithTags(RouteConfig.Tag).WithName("VendorQuestionnaire.DeleteFile");
    }

    private static async Task<IResult> Handle(Guid registrationId, Guid fileId, [FromQuery] string resumeToken, ISender sender, CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteQuestionnaireFileCommand(registrationId, fileId, resumeToken), cancellationToken);
        return Results.NoContent();
    }
}
