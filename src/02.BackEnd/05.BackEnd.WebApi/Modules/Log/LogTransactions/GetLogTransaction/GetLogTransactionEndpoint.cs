using EBVL.BackEnd.Logics.Modules.Log.LogTransactions.GetLogTransaction;
using EBVL.Shared.Dto.Modules.Log.LogTransactions;
using EBVL.Shared.Dto.Modules.Log.LogTransactions.GetLogTransaction;

namespace EBVL.BackEnd.WebApi.Modules.Log.LogTransactions.GetLogTransaction;

public sealed class GetLogTransactionEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapGet(GetLogTransactionRoute.Pattern, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName(GetLogTransactionRoute.Name)
            .WithDescription(GetLogTransactionRoute.Description)
            .Produces<GetLogTransactionResponse>();
    }

    private static async Task<IResult> Handle(
        [FromRoute] Guid id,
        HttpContext context,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new GetLogTransactionQuery
        {
            Id = id
        };

        var response = await sender.Send(query, cancellationToken);

        return Results.Ok(response);
    }
}
