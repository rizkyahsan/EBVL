using EBVL.BackEnd.Logics.Modules.MasterData.Questionnaires.UpdateQuestionnaireQuestion;
using EBVL.Shared.Dto.Modules.MasterData.Questionnaires;

namespace EBVL.BackEnd.WebApi.Modules.MasterData.Questionnaires.UpdateQuestionnaireQuestion;

public sealed class UpdateQuestionnaireQuestionEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapPut(QuestionnaireRoutes.QuestionDetail, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName("Questionnaires.UpdateQuestion")
            .Produces<GetQuestionnaireResponse>();
    }

    private static async Task<IResult> Handle(
        Guid questionnaireId, Guid sectionId, Guid questionId, UpdateQuestionnaireQuestionRequest body, ISender sender,
        CancellationToken cancellationToken)
    {
        return Results.Ok(await sender.Send(new UpdateQuestionnaireQuestionCommand(questionnaireId, sectionId, questionId, body), cancellationToken));
    }
}
