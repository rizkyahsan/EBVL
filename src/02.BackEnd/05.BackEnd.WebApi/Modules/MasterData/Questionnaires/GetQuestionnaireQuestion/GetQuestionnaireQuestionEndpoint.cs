using EBVL.BackEnd.Logics.Modules.MasterData.Questionnaires.GetQuestionnaireQuestion;
using EBVL.Shared.Dto.Modules.MasterData.Questionnaires;

namespace EBVL.BackEnd.WebApi.Modules.MasterData.Questionnaires.GetQuestionnaireQuestion;

public sealed class GetQuestionnaireQuestionEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapGet(QuestionnaireRoutes.QuestionDetail, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName("Questionnaires.QuestionDetail")
            .Produces<GetQuestionnaireQuestionResponse>();
    }

    private static async Task<IResult> Handle(
        Guid questionnaireId, Guid sectionId, Guid questionId, ISender sender,
        CancellationToken cancellationToken)
    {
        return Results.Ok(await sender.Send(new GetQuestionnaireQuestionQuery(questionnaireId, sectionId, questionId), cancellationToken));
    }
}
