using EBVL.BackEnd.Logics.Modules.MasterData.Questionnaires.AddQuestionnaire;
using EBVL.Shared.Dto.Modules.MasterData.Questionnaires;

namespace EBVL.BackEnd.WebApi.Modules.MasterData.Questionnaires.AddQuestionnaire;

public sealed class AddQuestionnaireEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapPost(QuestionnaireRoutes.Add, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName("Questionnaires.Add")
            .Produces<GetQuestionnaireResponse>();
    }

    private static async Task<IResult> Handle(
        AddQuestionnaireRequest body, ISender sender,
        CancellationToken cancellationToken)
    {
        return Results.Created(QuestionnaireRoutes.List, (object?)await sender.Send(new AddQuestionnaireCommand { Code = body.Code, BusinessProcess = body.BusinessProcess, IsActive = body.IsActive }, cancellationToken));
    }
}
