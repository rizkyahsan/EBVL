using EBVL.BackEnd.Logics.Modules.MasterData.Questionnaires.GetQuestionnaireSections;
using EBVL.Shared.Dto.Modules.MasterData.Questionnaires;

namespace EBVL.BackEnd.WebApi.Modules.MasterData.Questionnaires.GetQuestionnaireSections;

public sealed class GetQuestionnaireSectionsEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapGet(QuestionnaireRoutes.Sections, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName("Questionnaires.Sections")
            .Produces<GetQuestionnaireSectionsResponse>();
    }

    private static async Task<IResult> Handle(
        Guid questionnaireId, [AsParameters] GetQuestionnaireSectionsRequest request, ISender sender,
        CancellationToken cancellationToken)
    {
        return Results.Ok(await sender.Send(new GetQuestionnaireSectionsQuery(questionnaireId) { Page = request.Page, PageSize = request.PageSize, SearchText = request.SearchText, SortField = request.SortField, SortOrder = request.SortOrder }, cancellationToken));
    }
}
