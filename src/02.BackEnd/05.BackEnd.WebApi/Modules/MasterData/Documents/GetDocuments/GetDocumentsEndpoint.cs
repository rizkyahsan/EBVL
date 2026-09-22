using EBVL.BackEnd.Logics.Modules.MasterData.Documents.GetDocuments;
using EBVL.Shared.Dto.Modules.MasterData.Documents;

namespace EBVL.BackEnd.WebApi.Modules.MasterData.Documents.GetDocuments;

public sealed class GetDocumentsEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app.MapGet(DocumentRoutes.List, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName("Documents.List")
            .Produces<GetDocumentsResponse>();
    }

    private static async Task<IResult> Handle(ISender sender, CancellationToken ct)
    {
        return Results.Ok(await sender.Send(new GetDocumentsQuery(), ct));
    }
}
