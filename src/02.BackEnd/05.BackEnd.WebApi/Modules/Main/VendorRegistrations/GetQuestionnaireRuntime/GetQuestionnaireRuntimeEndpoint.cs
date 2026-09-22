using EBVL.BackEnd.Logics.Modules.Main.VendorRegistrations.GetQuestionnaireRuntime;
using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.Questionnaires;

namespace EBVL.BackEnd.WebApi.Modules.Main.VendorRegistrations.GetQuestionnaireRuntime;

public sealed class GetQuestionnaireRuntimeEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app.MapPost(QuestionnaireRuntimeRoutes.Get, Handle).AllowAnonymous().WithTags(RouteConfig.Tag).WithName("VendorQuestionnaire.Get");
    }

    private static async Task<IResult> Handle(Guid registrationId, ResumeQuestionnaireRequest body, ISender sender, CancellationToken cancellationToken)
    {
        return Results.Ok(await sender.Send(new GetQuestionnaireRuntimeQuery(registrationId, body.ResumeToken), cancellationToken));
    }
}
