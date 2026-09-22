using EBVL.BackEnd.Logics.Modules.MasterData.Questionnaires.UpdateQuestionnaireSection;
using EBVL.Shared.Dto.Modules.MasterData.Questionnaires;

namespace EBVL.BackEnd.WebApi.Modules.MasterData.Questionnaires.UpdateQuestionnaireSection;

public sealed class UpdateQuestionnaireSectionEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapPut(QuestionnaireRoutes.UpdateSection, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName("Questionnaires.UpdateSection")
            .Produces<GetQuestionnaireResponse>();
    }

    private static async Task<IResult> Handle(
        Guid questionnaireId, Guid sectionId, UpdateQuestionnaireSectionRequest body, ISender sender,
        CancellationToken cancellationToken)
    {
        return Results.Ok(await sender.Send(new UpdateQuestionnaireSectionCommand(questionnaireId, sectionId, body), cancellationToken));
    }
}
