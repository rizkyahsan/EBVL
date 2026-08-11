using EBVL.BackEnd.Logics.Modules.Examples.Documents.GetDocuments;
using EBVL.Shared.Dto.Modules.Examples.Documents;
using EBVL.Shared.Dto.Modules.Examples.Documents.GetDocuments;

namespace EBVL.BackEnd.WebApi.Modules.Examples.Documents.GetDocuments;

public sealed class GetDocumentsEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapGet(GetDocumentsRoute.Pattern, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName(GetDocumentsRoute.Name)
            .WithDescription(GetDocumentsRoute.Description)
            .Produces<GetDocumentsResponse>();
    }

    private static async Task<IResult> Handle(
        ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new GetDocumentsQuery();
        var response = await sender.Send(query, cancellationToken);

        return Results.Ok(response);
    }
}
