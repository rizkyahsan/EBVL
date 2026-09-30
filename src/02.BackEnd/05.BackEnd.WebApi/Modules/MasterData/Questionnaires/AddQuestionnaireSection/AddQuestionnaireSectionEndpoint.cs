using EBVL.BackEnd.Logics.Modules.MasterData.Questionnaires.AddQuestionnaireSection;
using EBVL.Shared.Dto.Modules.MasterData.Questionnaires;

namespace EBVL.BackEnd.WebApi.Modules.MasterData.Questionnaires.AddQuestionnaireSection;

public sealed class AddQuestionnaireSectionEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapPost(AddQuestionnaireSectionRoute.Pattern, Handle)
            .RequireAuthorization(QuestionnairePermissions.Manage)
            .WithTags(RouteConfig.Tag)
            .WithName("Questionnaires.AddSection")
            .Produces<GetQuestionnaireResponse>();
    }

    private static async Task<IResult> Handle(
        Guid questionnaireId, AddQuestionnaireSectionRequest body, ISender sender,
        CancellationToken cancellationToken)
    {
        return Results.Created(GetQuestionnaireRoute.ResourceUri(questionnaireId), (object?)await sender.Send(new AddQuestionnaireSectionCommand(questionnaireId, body), cancellationToken));
    }
}
