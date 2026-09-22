using EBVL.BackEnd.Logics.Modules.Main.VendorRegistrations.StartQuestionnaire;
using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.PreRegistration;
using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.Questionnaires;

namespace EBVL.BackEnd.WebApi.Modules.Main.VendorRegistrations.StartQuestionnaire;

public sealed class StartQuestionnaireEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app.MapPost(QuestionnaireRuntimeRoutes.Start, Handle).AllowAnonymous().WithTags(RouteConfig.Tag).WithName("VendorQuestionnaire.Start");
    }

    private static async Task<IResult> Handle(PreRegistrationRequest body, ISender sender, CancellationToken cancellationToken)
    {
        return Results.Ok(await sender.Send(new StartQuestionnaireCommand(body), cancellationToken));
    }
}
