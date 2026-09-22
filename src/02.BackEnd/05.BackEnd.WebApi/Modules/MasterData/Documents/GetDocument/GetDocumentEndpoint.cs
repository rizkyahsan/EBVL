using EBVL.BackEnd.Logics.Modules.MasterData.Documents.GetDocument;
using EBVL.Shared.Dto.Modules.MasterData.Documents;

namespace EBVL.BackEnd.WebApi.Modules.MasterData.Documents.GetDocument;

public sealed class GetDocumentEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app.MapGet(DocumentRoutes.Detail, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName("Documents.Detail")
            .Produces<GetDocumentResponse>();
    }

    private static async Task<IResult> Handle(Guid documentRequirementSetId, ISender sender, CancellationToken ct)
    {
        return Results.Ok(await sender.Send(new GetDocumentQuery(documentRequirementSetId), ct));
    }
}
