using EBVL.BackEnd.Logics.Modules.Administration.ApiCalls.GetApiCalls;
using EBVL.Shared.Dto.Modules.Administration.ApiCalls;
using EBVL.Shared.Dto.Modules.Administration.ApiCalls.GetApiCalls;

namespace EBVL.BackEnd.WebApi.Modules.Administration.ApiCalls.GetApiCalls;

public sealed class GetApiCallsEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapGet(GetApiCallsRoute.Pattern, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName(GetApiCallsRoute.Name)
            .WithDescription(GetApiCallsRoute.Description)
            .Produces<GetApiCallsResponse>();
    }

    private static async Task<IResult> Handle(
        ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new GetApiCallsQuery();
        var response = await sender.Send(query, cancellationToken);

        return Results.Ok(response);
    }
}
