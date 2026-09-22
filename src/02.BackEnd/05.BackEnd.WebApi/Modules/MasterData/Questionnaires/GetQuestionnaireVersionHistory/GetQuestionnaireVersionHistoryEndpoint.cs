using EBVL.BackEnd.Logics.Modules.MasterData.Questionnaires.GetQuestionnaireVersionHistory;
using EBVL.Shared.Dto.Modules.MasterData.Questionnaires;

namespace EBVL.BackEnd.WebApi.Modules.MasterData.Questionnaires.GetQuestionnaireVersionHistory;

public sealed class GetQuestionnaireVersionHistoryEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapGet(QuestionnaireRoutes.History, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName("Questionnaires.History")
            .Produces<GetQuestionnaireVersionHistoryResponse>();
    }

    private static async Task<IResult> Handle(
        Guid seriesId, ISender sender,
        CancellationToken cancellationToken)
    {
        return Results.Ok(await sender.Send(new GetQuestionnaireVersionHistoryQuery(seriesId), cancellationToken));
    }
}
