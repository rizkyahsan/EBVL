using EBVL.BackEnd.Logics.Modules.MasterData.Documents.AddDocument;
using EBVL.Shared.Dto.Modules.MasterData.Documents;

namespace EBVL.BackEnd.WebApi.Modules.MasterData.Documents.AddDocument;

public sealed class AddDocumentEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app.MapPost(DocumentRoutes.Add, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName("Documents.Add")
            .Produces<GetDocumentResponse>(StatusCodes.Status201Created);
    }

    private static async Task<IResult> Handle(Guid documentRequirementSetId, AddDocumentRequest body, ISender sender, CancellationToken ct)
    {
        return Results.Created(DocumentRoutes.Detail, (object?)await sender.Send(new AddDocumentCommand(documentRequirementSetId, body), ct));
    }
}
