using EBVL.BackEnd.Logics.Modules.MasterData.Documents;
using EBVL.Shared.Dto.Modules.MasterData.Documents;

namespace EBVL.BackEnd.WebApi.Modules.MasterData.Documents;

public sealed class DocumentEndpoints : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        // TODO: Restore RequireAuthorization after IdAMan API authentication setup is complete.
        _ = app.MapGet(DocumentRoutes.List, async (ISender sender, CancellationToken ct) => Results.Ok(await sender.Send(new GetDocumentsQuery(), ct))).AllowAnonymous().WithTags(RouteConfig.Tag).WithName("Documents.List");
        _ = app.MapGet(DocumentRoutes.Detail, async (Guid documentRequirementSetId, ISender sender, CancellationToken ct) => Results.Ok(await sender.Send(new GetDocumentQuery(documentRequirementSetId), ct))).AllowAnonymous().WithTags(RouteConfig.Tag).WithName("Documents.Detail");
        _ = app.MapGet(DocumentRoutes.History, async (Guid seriesId, ISender sender, CancellationToken ct) => Results.Ok(await sender.Send(new GetDocumentHistoryQuery(seriesId), ct))).AllowAnonymous().WithTags(RouteConfig.Tag).WithName("Documents.History");
        _ = app.MapPost(DocumentRoutes.Draft, async (Guid documentRequirementSetId, ISender sender, CancellationToken ct) => Results.Ok(await sender.Send(new CreateDocumentDraftCommand(documentRequirementSetId), ct))).AllowAnonymous().WithTags(RouteConfig.Tag).WithName("Documents.Draft");
        _ = app.MapPost(DocumentRoutes.Publish, async (Guid documentRequirementSetId, PublishDocumentRequirementSetRequest body, ISender sender, CancellationToken ct) => Results.Ok(await sender.Send(new PublishDocumentRequirementSetCommand(documentRequirementSetId, body.RowVersion), ct))).AllowAnonymous().WithTags(RouteConfig.Tag).WithName("Documents.Publish");
        _ = app.MapPost(DocumentRoutes.Add, async (Guid documentRequirementSetId, AddDocumentRequest body, ISender sender, CancellationToken ct) => Results.Created(DocumentRoutes.Detail, (object?)await sender.Send(new AddDocumentCommand(documentRequirementSetId, body), ct))).AllowAnonymous().WithTags(RouteConfig.Tag).WithName("Documents.Add");
        _ = app.MapPut(DocumentRoutes.Requirement, async (Guid documentRequirementSetId, Guid documentId, UpdateDocumentRequest body, ISender sender, CancellationToken ct) => Results.Ok(await sender.Send(new UpdateDocumentCommand(documentRequirementSetId, documentId, body), ct))).AllowAnonymous().WithTags(RouteConfig.Tag).WithName("Documents.Update");
        return app.MapDelete(DocumentRoutes.Requirement, async (Guid documentRequirementSetId, Guid documentId, string rowVersion, ISender sender, CancellationToken ct) => Results.Ok(await sender.Send(new DeleteDocumentCommand(documentRequirementSetId, documentId, rowVersion), ct))).AllowAnonymous().WithTags(RouteConfig.Tag).WithName("Documents.Delete");
    }
}
