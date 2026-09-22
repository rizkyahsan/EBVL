using EBVL.BackEnd.Logics.Modules.MasterData.Documents.GetDocumentHistory;
using EBVL.Shared.Dto.Modules.MasterData.Documents;

namespace EBVL.BackEnd.WebApi.Modules.MasterData.Documents.GetDocumentHistory;

public sealed class GetDocumentHistoryEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app.MapGet(DocumentRoutes.History, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName("Documents.History")
            .Produces<GetDocumentHistoryResponse>();
    }

    private static async Task<IResult> Handle(Guid seriesId, ISender sender, CancellationToken ct)
    {
        return Results.Ok(await sender.Send(new GetDocumentHistoryQuery(seriesId), ct));
    }
}
