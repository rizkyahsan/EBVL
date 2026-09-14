using EBVL.BackEnd.Logics.Modules.MasterData.Documents;
using EBVL.Shared.Dto.Modules.MasterData.Documents;

namespace EBVL.BackEnd.WebApi.Modules.MasterData.Documents;

public sealed class DocumentEndpoints : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        // TODO: Restore RequireAuthorization after IdAMan API authentication setup is complete.
        _ = app.MapGet(DocumentRoutes.List, async (ISender sender, CancellationToken ct) => Results.Ok(await sender.Send(new GetDocumentsQuery(), ct))).AllowAnonymous().WithTags(RouteConfig.Tag).WithName("Documents.List");
        _ = app.MapGet(DocumentRoutes.Detail, async (Guid documentId, ISender sender, CancellationToken ct) => Results.Ok(await sender.Send(new GetDocumentQuery(documentId), ct))).AllowAnonymous().WithTags(RouteConfig.Tag).WithName("Documents.Detail");
        _ = app.MapPost(DocumentRoutes.Add, async (AddDocumentRequest body, ISender sender, CancellationToken ct) => Results.Created(DocumentRoutes.List, (object?)await sender.Send(new AddDocumentCommand { BusinessProcess = body.BusinessProcess, Name = body.Name, MaxSizeMb = body.MaxSizeMb, IsMandatory = body.IsMandatory, IsActive = body.IsActive }, ct))).AllowAnonymous().WithTags(RouteConfig.Tag).WithName("Documents.Add");
        return app.MapPut(DocumentRoutes.Update, async (Guid documentId, UpdateDocumentRequest body, ISender sender, CancellationToken ct) => Results.Ok(await sender.Send(new UpdateDocumentCommand(documentId, body), ct))).AllowAnonymous().WithTags(RouteConfig.Tag).WithName("Documents.Update");
    }
}
