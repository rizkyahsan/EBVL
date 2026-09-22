using EBVL.BackEnd.Logics.Modules.MasterData.Questionnaires.GetQuestionnaireQuestions;
using EBVL.Shared.Dto.Modules.MasterData.Questionnaires;

namespace EBVL.BackEnd.WebApi.Modules.MasterData.Questionnaires.GetQuestionnaireQuestions;

public sealed class GetQuestionnaireQuestionsEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapGet(QuestionnaireRoutes.Questions, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName("Questionnaires.Questions")
            .Produces<GetQuestionnaireQuestionsResponse>();
    }

    private static async Task<IResult> Handle(
        Guid questionnaireId, Guid sectionId, [AsParameters] GetQuestionnaireQuestionsRequest request, ISender sender,
        CancellationToken cancellationToken)
    {
        return Results.Ok(await sender.Send(new GetQuestionnaireQuestionsQuery(questionnaireId, sectionId) { Page = request.Page, PageSize = request.PageSize, SearchText = request.SearchText, SortField = request.SortField, SortOrder = request.SortOrder }, cancellationToken));
    }
}
