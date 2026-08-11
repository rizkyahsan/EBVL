using EBVL.BackEnd.Logics.Modules.Administration.ApiCalls.GetApiCall;
using EBVL.Shared.Dto.Modules.Administration.ApiCalls;
using EBVL.Shared.Dto.Modules.Administration.ApiCalls.GetApiCall;

namespace EBVL.BackEnd.WebApi.Modules.Administration.ApiCalls.GetApiCall;

public sealed class GetApiCallEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapGet(GetApiCallRoute.Pattern, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName(GetApiCallRoute.Name)
            .WithDescription(GetApiCallRoute.Description)
            .Produces<GetApiCallResponse>();
    }

    private static async Task<IResult> Handle(
        [FromRoute] Guid apiCallId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new GetApiCallQuery
        {
            ApiCallId = apiCallId
        };

        var response = await sender.Send(query, cancellationToken);

        return Results.Ok(response);
    }
}
