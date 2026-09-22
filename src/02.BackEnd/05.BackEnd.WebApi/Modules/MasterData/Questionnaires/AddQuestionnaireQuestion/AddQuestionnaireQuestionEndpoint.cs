using EBVL.BackEnd.Logics.Modules.MasterData.Questionnaires.AddQuestionnaireQuestion;
using EBVL.Shared.Dto.Modules.MasterData.Questionnaires;

namespace EBVL.BackEnd.WebApi.Modules.MasterData.Questionnaires.AddQuestionnaireQuestion;

public sealed class AddQuestionnaireQuestionEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapPost(QuestionnaireRoutes.AddQuestion, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName("Questionnaires.AddQuestion")
            .Produces<GetQuestionnaireResponse>();
    }

    private static async Task<IResult> Handle(
        Guid questionnaireId, Guid sectionId, AddQuestionnaireQuestionRequest body, ISender sender,
        CancellationToken cancellationToken)
    {
        return Results.Created(QuestionnaireRoutes.Detail, (object?)await sender.Send(new AddQuestionnaireQuestionCommand(questionnaireId, sectionId, body), cancellationToken));
    }
}
