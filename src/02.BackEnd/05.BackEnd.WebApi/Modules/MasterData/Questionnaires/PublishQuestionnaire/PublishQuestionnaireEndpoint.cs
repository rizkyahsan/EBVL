using EBVL.BackEnd.Logics.Modules.MasterData.Questionnaires.PublishQuestionnaire;
using EBVL.Shared.Dto.Modules.MasterData.Questionnaires;

namespace EBVL.BackEnd.WebApi.Modules.MasterData.Questionnaires.PublishQuestionnaire;

public sealed class PublishQuestionnaireEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapPost(QuestionnaireRoutes.Publish, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName("Questionnaires.Publish")
            .Produces<GetQuestionnaireResponse>();
    }

    private static async Task<IResult> Handle(
        Guid questionnaireId, PublishQuestionnaireRequest body, ISender sender,
        CancellationToken cancellationToken)
    {
        return Results.Ok(await sender.Send(new PublishQuestionnaireCommand(questionnaireId, body.RowVersion), cancellationToken));
    }
}
