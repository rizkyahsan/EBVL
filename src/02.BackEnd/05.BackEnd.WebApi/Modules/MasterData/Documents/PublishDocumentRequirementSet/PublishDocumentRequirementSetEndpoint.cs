using EBVL.BackEnd.Logics.Modules.MasterData.Documents.PublishDocumentRequirementSet;
using EBVL.Shared.Dto.Modules.MasterData.Documents;

namespace EBVL.BackEnd.WebApi.Modules.MasterData.Documents.PublishDocumentRequirementSet;

public sealed class PublishDocumentRequirementSetEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app.MapPost(DocumentRoutes.Publish, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName("Documents.Publish")
            .Produces<GetDocumentResponse>();
    }

    private static async Task<IResult> Handle(Guid documentRequirementSetId, PublishDocumentRequirementSetRequest body, ISender sender, CancellationToken ct)
    {
        return Results.Ok(await sender.Send(new PublishDocumentRequirementSetCommand(documentRequirementSetId, body.RowVersion), ct));
    }
}
