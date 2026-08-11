using EBVL.BackEnd.Logics.Modules.Examples.Documents.DownloadDocument;
using EBVL.Shared.Dto.Modules.Examples.Documents;
using EBVL.Shared.Dto.Modules.Examples.Documents.DownloadDocument;

namespace EBVL.BackEnd.WebApi.Modules.Examples.Documents.DownloadDocument;

public sealed class DownloadDocumentEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapGet(DownloadDocumentRoute.Pattern, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName(DownloadDocumentRoute.Name)
            .WithDescription(DownloadDocumentRoute.Description)
            .Produces<DownloadDocumentResponse>();
    }

    private static async Task<IResult> Handle(
        [FromRoute] Guid documentId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new DownloadDocumentQuery
        {
            DocumentId = documentId
        };

        var response = await sender.Send(query, cancellationToken);

        return Results.Ok(response);
    }
}
