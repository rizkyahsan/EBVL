using EBVL.BackEnd.Logics.Modules.Log.LogTransactions.GetLogTransactions;
using EBVL.Shared.Dto.Modules.Log.LogTransactions;
using EBVL.Shared.Dto.Modules.Log.LogTransactions.GetLogTransactions;

namespace EBVL.BackEnd.WebApi.Modules.Log.LogTransactions.GetLogTransactions;

public sealed class GetLogTransactionsEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapGet(GetLogTransactionsRoute.Pattern, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName(GetLogTransactionsRoute.Name)
            .WithDescription(GetLogTransactionsRoute.Description)
            .Produces<GetLogTransactionsResponse>();
    }

    private static async Task<IResult> Handle(
        [AsParameters] GetLogTransactionsQuery query,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var response = await sender.Send(query, cancellationToken);

        return Results.Ok(response);
    }
}
