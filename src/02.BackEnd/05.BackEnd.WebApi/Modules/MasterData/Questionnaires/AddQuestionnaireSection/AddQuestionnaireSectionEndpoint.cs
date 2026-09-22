using EBVL.BackEnd.Logics.Modules.MasterData.Questionnaires.AddQuestionnaireSection;
using EBVL.Shared.Dto.Modules.MasterData.Questionnaires;

namespace EBVL.BackEnd.WebApi.Modules.MasterData.Questionnaires.AddQuestionnaireSection;

public sealed class AddQuestionnaireSectionEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapPost(QuestionnaireRoutes.Sections, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName("Questionnaires.AddSection")
            .Produces<GetQuestionnaireResponse>();
    }

    private static async Task<IResult> Handle(
        Guid questionnaireId, AddQuestionnaireSectionRequest body, ISender sender,
        CancellationToken cancellationToken)
    {
        return Results.Created(QuestionnaireRoutes.Detail, (object?)await sender.Send(new AddQuestionnaireSectionCommand(questionnaireId, body), cancellationToken));
    }
}
