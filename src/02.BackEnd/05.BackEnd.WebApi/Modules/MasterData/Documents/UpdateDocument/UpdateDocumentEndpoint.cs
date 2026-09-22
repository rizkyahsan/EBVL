using EBVL.BackEnd.Logics.Modules.MasterData.Documents.UpdateDocument;
using EBVL.Shared.Dto.Modules.MasterData.Documents;

namespace EBVL.BackEnd.WebApi.Modules.MasterData.Documents.UpdateDocument;

public sealed class UpdateDocumentEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app.MapPut(DocumentRoutes.Requirement, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName("Documents.Update")
            .Produces<GetDocumentResponse>();
    }

    private static async Task<IResult> Handle(Guid documentRequirementSetId, Guid documentId, UpdateDocumentRequest body, ISender sender, CancellationToken ct)
    {
        return Results.Ok(await sender.Send(new UpdateDocumentCommand(documentRequirementSetId, documentId, body), ct));
    }
}
