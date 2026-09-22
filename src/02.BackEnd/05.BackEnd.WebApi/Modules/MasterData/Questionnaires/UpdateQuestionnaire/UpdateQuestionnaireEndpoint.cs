using EBVL.BackEnd.Logics.Modules.MasterData.Questionnaires.UpdateQuestionnaire;
using EBVL.Shared.Dto.Modules.MasterData.Questionnaires;

namespace EBVL.BackEnd.WebApi.Modules.MasterData.Questionnaires.UpdateQuestionnaire;

public sealed class UpdateQuestionnaireEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapPut(QuestionnaireRoutes.Update, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName("Questionnaires.Update")
            .Produces<GetQuestionnaireResponse>();
    }

    private static async Task<IResult> Handle(
        Guid questionnaireId, UpdateQuestionnaireRequest body, ISender sender,
        CancellationToken cancellationToken)
    {
        return Results.Ok(await sender.Send(new UpdateQuestionnaireCommand(questionnaireId, body), cancellationToken));
    }
}
