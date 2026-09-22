using EBVL.BackEnd.Logics.Modules.Main.VendorRegistrations.SubmitQuestionnaire;
using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.Questionnaires;

namespace EBVL.BackEnd.WebApi.Modules.Main.VendorRegistrations.SubmitQuestionnaire;

public sealed class SubmitQuestionnaireEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app.MapPost(QuestionnaireRuntimeRoutes.Submit, Handle).AllowAnonymous().WithTags(RouteConfig.Tag).WithName("VendorQuestionnaire.Submit");
    }

    private static async Task<IResult> Handle(Guid registrationId, SubmitQuestionnaireRequest body, ISender sender, CancellationToken cancellationToken)
    {
        return Results.Ok(await sender.Send(new SubmitQuestionnaireCommand(registrationId, body), cancellationToken));
    }
}
