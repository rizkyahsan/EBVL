using EBVL.BackEnd.Logics.Modules.Main.VendorRegistrations.SaveQuestionnaireAnswers;
using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.Questionnaires;

namespace EBVL.BackEnd.WebApi.Modules.Main.VendorRegistrations.SaveQuestionnaireAnswers;

public sealed class SaveQuestionnaireAnswersEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app.MapPut(QuestionnaireRuntimeRoutes.Answers, Handle).AllowAnonymous().WithTags(RouteConfig.Tag).WithName("VendorQuestionnaire.SaveAnswers");
    }

    private static async Task<IResult> Handle(Guid registrationId, SaveAnswersRequest body, ISender sender, CancellationToken cancellationToken)
    {
        return Results.Ok(await sender.Send(new SaveQuestionnaireAnswersCommand(registrationId, body), cancellationToken));
    }
}
