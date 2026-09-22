using EBVL.BackEnd.Logics.Modules.MasterData.Questionnaires.DeleteQuestionnaireSection;
using EBVL.Shared.Dto.Modules.MasterData.Questionnaires;

namespace EBVL.BackEnd.WebApi.Modules.MasterData.Questionnaires.DeleteQuestionnaireSection;

public sealed class DeleteQuestionnaireSectionEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapDelete(QuestionnaireRoutes.UpdateSection, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName("Questionnaires.DeleteSection")
            .Produces<GetQuestionnaireResponse>();
    }

    private static async Task<IResult> Handle(
        Guid questionnaireId, Guid sectionId, [AsParameters] DeleteQuestionnaireChildRequest body, ISender sender,
        CancellationToken cancellationToken)
    {
        return Results.Ok(await sender.Send(new DeleteQuestionnaireSectionCommand(questionnaireId, sectionId, body.RowVersion), cancellationToken));
    }
}
