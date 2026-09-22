using EBVL.BackEnd.Logics.Modules.MasterData.Questionnaires.GetQuestionnaires;
using EBVL.Shared.Dto.Modules.MasterData.Questionnaires;

namespace EBVL.BackEnd.WebApi.Modules.MasterData.Questionnaires.GetQuestionnaires;

public sealed class GetQuestionnairesEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapGet(QuestionnaireRoutes.List, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName("Questionnaires.List")
            .Produces<GetQuestionnairesResponse>();
    }

    private static async Task<IResult> Handle(
        ISender sender,
        CancellationToken cancellationToken)
    {
        return Results.Ok(await sender.Send(new GetQuestionnairesQuery(), cancellationToken));
    }
}
