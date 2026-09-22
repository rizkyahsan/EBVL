using EBVL.BackEnd.Logics.Modules.MasterData.Questionnaires.DeleteQuestionnaireQuestion;
using EBVL.Shared.Dto.Modules.MasterData.Questionnaires;

namespace EBVL.BackEnd.WebApi.Modules.MasterData.Questionnaires.DeleteQuestionnaireQuestion;

public sealed class DeleteQuestionnaireQuestionEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapDelete(QuestionnaireRoutes.QuestionDetail, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName("Questionnaires.DeleteQuestion")
            .Produces<GetQuestionnaireResponse>();
    }

    private static async Task<IResult> Handle(
        Guid questionnaireId, Guid sectionId, Guid questionId, [AsParameters] DeleteQuestionnaireChildRequest body, ISender sender,
        CancellationToken cancellationToken)
    {
        return Results.Ok(await sender.Send(new DeleteQuestionnaireQuestionCommand(questionnaireId, sectionId, questionId, body.RowVersion), cancellationToken));
    }
}
