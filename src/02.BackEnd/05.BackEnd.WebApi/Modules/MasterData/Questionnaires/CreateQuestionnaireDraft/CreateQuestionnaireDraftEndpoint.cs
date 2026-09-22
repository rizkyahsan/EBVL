using EBVL.BackEnd.Logics.Modules.MasterData.Questionnaires.CreateQuestionnaireDraft;
using EBVL.Shared.Dto.Modules.MasterData.Questionnaires;

namespace EBVL.BackEnd.WebApi.Modules.MasterData.Questionnaires.CreateQuestionnaireDraft;

public sealed class CreateQuestionnaireDraftEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapPost(QuestionnaireRoutes.Draft, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName("Questionnaires.CreateDraft")
            .Produces<GetQuestionnaireResponse>();
    }

    private static async Task<IResult> Handle(
        Guid questionnaireId, ISender sender,
        CancellationToken cancellationToken)
    {
        return Results.Ok(await sender.Send(new CreateQuestionnaireDraftCommand(questionnaireId), cancellationToken));
    }
}
