using EBVL.BackEnd.Logics.Modules.MasterData.Questionnaires.GetQuestionnaire;
using EBVL.Shared.Dto.Modules.MasterData.Questionnaires;

namespace EBVL.BackEnd.WebApi.Modules.MasterData.Questionnaires.GetQuestionnaire;

public sealed class GetQuestionnaireEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapGet(QuestionnaireRoutes.Detail, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName("Questionnaires.Detail")
            .Produces<GetQuestionnaireResponse>();
    }

    private static async Task<IResult> Handle(
        Guid questionnaireId, ISender sender,
        CancellationToken cancellationToken)
    {
        return Results.Ok(await sender.Send(new GetQuestionnaireQuery(questionnaireId), cancellationToken));
    }
}
