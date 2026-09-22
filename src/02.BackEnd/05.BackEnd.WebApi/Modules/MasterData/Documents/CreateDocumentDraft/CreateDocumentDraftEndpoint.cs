using EBVL.BackEnd.Logics.Modules.MasterData.Documents.CreateDocumentDraft;
using EBVL.Shared.Dto.Modules.MasterData.Documents;

namespace EBVL.BackEnd.WebApi.Modules.MasterData.Documents.CreateDocumentDraft;

public sealed class CreateDocumentDraftEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app.MapPost(DocumentRoutes.Draft, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName("Documents.Draft")
            .Produces<GetDocumentResponse>();
    }

    private static async Task<IResult> Handle(Guid documentRequirementSetId, ISender sender, CancellationToken ct)
    {
        return Results.Ok(await sender.Send(new CreateDocumentDraftCommand(documentRequirementSetId), ct));
    }
}
