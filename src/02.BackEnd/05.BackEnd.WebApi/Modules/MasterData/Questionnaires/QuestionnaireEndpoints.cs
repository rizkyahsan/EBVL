using EBVL.BackEnd.Logics.Modules.MasterData.Questionnaires;
using EBVL.Shared.Dto.Modules.MasterData.Questionnaires;

namespace EBVL.BackEnd.WebApi.Modules.MasterData.Questionnaires;

public sealed class QuestionnaireEndpoints : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        _ = app.MapGet(QuestionnaireRoutes.List, async (ISender sender, CancellationToken ct) => Results.Ok(await sender.Send(new GetQuestionnairesQuery(), ct))).RequireAuthorization().WithTags(RouteConfig.Tag).WithName("Questionnaires.List");
        _ = app.MapGet(QuestionnaireRoutes.Detail, async (Guid id, ISender sender, CancellationToken ct) => Results.Ok(await sender.Send(new GetQuestionnaireQuery(id), ct))).RequireAuthorization().WithTags(RouteConfig.Tag).WithName("Questionnaires.Detail");
        _ = app.MapPost(QuestionnaireRoutes.Add, async (AddQuestionnaireRequest body, ISender sender, CancellationToken ct) => Results.Created(QuestionnaireRoutes.List, (object?)await sender.Send(new AddQuestionnaireCommand { Code = body.Code, BusinessProcess = body.BusinessProcess }, ct))).RequireAuthorization().WithTags(RouteConfig.Tag).WithName("Questionnaires.Add");
        _ = app.MapPut(QuestionnaireRoutes.Update, async (Guid id, UpdateQuestionnaireDraftRequest body, ISender sender, CancellationToken ct) => Results.Ok(await sender.Send(new UpdateQuestionnaireDraftCommand(id, body), ct))).RequireAuthorization().WithTags(RouteConfig.Tag).WithName("Questionnaires.UpdateDraft");
        _ = app.MapPost(QuestionnaireRoutes.Validate, async (Guid id, ISender sender, CancellationToken ct) => Results.Ok(await sender.Send(new ValidateQuestionnaireCommand(id), ct))).RequireAuthorization().WithTags(RouteConfig.Tag).WithName("Questionnaires.Validate");
        _ = app.MapPost(QuestionnaireRoutes.Publish, async (Guid id, ISender sender, CancellationToken ct) =>
        {
            await sender.Send(new PublishQuestionnaireCommand(id), ct);
            return Results.NoContent();
        }).RequireAuthorization().WithTags(RouteConfig.Tag).WithName("Questionnaires.Publish");
        _ = app.MapPost(QuestionnaireRoutes.Deactivate, async (Guid id, ISender sender, CancellationToken ct) =>
        {
            await sender.Send(new DeactivateQuestionnaireCommand(id), ct);
            return Results.NoContent();
        }).RequireAuthorization().WithTags(RouteConfig.Tag).WithName("Questionnaires.Deactivate");
        _ = app.MapPost(QuestionnaireRoutes.Archive, async (Guid id, ISender sender, CancellationToken ct) =>
        {
            await sender.Send(new ArchiveQuestionnaireCommand(id), ct);
            return Results.NoContent();
        }).RequireAuthorization().WithTags(RouteConfig.Tag).WithName("Questionnaires.Archive");
        return app.MapDelete(QuestionnaireRoutes.DeleteDraft, async (Guid id, ISender sender, CancellationToken ct) =>
        {
            await sender.Send(new DeleteQuestionnaireDraftCommand(id), ct);
            return Results.NoContent();
        }).RequireAuthorization().WithTags(RouteConfig.Tag).WithName("Questionnaires.DeleteDraft");
    }
}
