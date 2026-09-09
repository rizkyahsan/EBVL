using EBVL.BackEnd.Logics.Modules.MasterData.Questionnaires;
using EBVL.Shared.Dto.Modules.MasterData.Questionnaires;

namespace EBVL.BackEnd.WebApi.Modules.MasterData.Questionnaires;

public sealed class QuestionnaireEndpoints : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        // TODO: Restore RequireAuthorization after IdAMan API authentication setup is complete.
        _ = app.MapGet(QuestionnaireRoutes.List, async (ISender sender, CancellationToken ct) => Results.Ok(await sender.Send(new GetQuestionnairesQuery(), ct))).AllowAnonymous().WithTags(RouteConfig.Tag).WithName("Questionnaires.List");
        _ = app.MapGet(QuestionnaireRoutes.Detail, async (Guid questionnaireId, ISender sender, CancellationToken ct) => Results.Ok(await sender.Send(new GetQuestionnaireQuery(questionnaireId), ct))).AllowAnonymous().WithTags(RouteConfig.Tag).WithName("Questionnaires.Detail");
        _ = app.MapPost(QuestionnaireRoutes.Add, async (AddQuestionnaireRequest body, ISender sender, CancellationToken ct) => Results.Created(QuestionnaireRoutes.List, (object?)await sender.Send(new AddQuestionnaireCommand { BusinessProcess = body.BusinessProcess, VendorType = body.VendorType, Section = body.Section, IsActive = body.IsActive }, ct))).AllowAnonymous().WithTags(RouteConfig.Tag).WithName("Questionnaires.Add");
        _ = app.MapPost(QuestionnaireRoutes.AddQuestion, async (Guid questionnaireId, Guid sectionId, AddQuestionnaireQuestionRequest body, ISender sender, CancellationToken ct) => Results.Created(QuestionnaireRoutes.Detail, (object?)await sender.Send(new AddQuestionnaireQuestionCommand(questionnaireId, sectionId, body), ct))).AllowAnonymous().WithTags(RouteConfig.Tag).WithName("Questionnaires.AddQuestion");
        return app.MapPut(QuestionnaireRoutes.Update, async (Guid questionnaireId, UpdateQuestionnaireRequest body, ISender sender, CancellationToken ct) => Results.Ok(await sender.Send(new UpdateQuestionnaireCommand(questionnaireId, body), ct))).AllowAnonymous().WithTags(RouteConfig.Tag).WithName("Questionnaires.Update");
    }
}
