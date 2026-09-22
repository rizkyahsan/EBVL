using EBVL.BackEnd.Logics.Modules.MasterData.Documents.DeleteDocument;
using EBVL.Shared.Dto.Modules.MasterData.Documents;

namespace EBVL.BackEnd.WebApi.Modules.MasterData.Documents.DeleteDocument;

public sealed class DeleteDocumentEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app.MapDelete(DocumentRoutes.Requirement, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName("Documents.Delete")
            .Produces<GetDocumentResponse>();
    }

    private static async Task<IResult> Handle(Guid documentRequirementSetId, Guid documentId, string rowVersion, ISender sender, CancellationToken ct)
    {
        return Results.Ok(await sender.Send(new DeleteDocumentCommand(documentRequirementSetId, documentId, rowVersion), ct));
    }
}
