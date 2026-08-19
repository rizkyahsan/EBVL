using EBVL.BackEnd.Logics.Modules.MasterData.Lenders.GetLender;
using EBVL.Shared.Dto.Modules.MasterData.Lenders;
using EBVL.Shared.Dto.Modules.MasterData.Lenders.GetLender;

namespace EBVL.BackEnd.WebApi.Modules.MasterData.Lenders.GetLender;

public sealed class GetLenderEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapGet(GetLenderRoute.Pattern, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName(GetLenderRoute.Name)
            .WithDescription(GetLenderRoute.Description)
            .Produces<GetLenderResponse>();
    }

    private static async Task<IResult> Handle(
        [FromRoute] Guid lenderId,
        HttpContext context,
        ISender sender,
        CancellationToken cancellationToken)
    {
        if (!context.User.HasPermission(Permissions.MasterDataLendersRead))
        {
            //return Results.Forbid();
        }

        var query = new GetLenderQuery
        {
            LenderId = lenderId
        };

        var response = await sender.Send(query, cancellationToken);

        return Results.Ok(response);
    }
}
