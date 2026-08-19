using EBVL.BackEnd.Logics.Modules.MasterData.Lenders.GetLenders;
using EBVL.Shared.Dto.Modules.MasterData.Lenders;
using EBVL.Shared.Dto.Modules.MasterData.Lenders.GetLenders;

namespace EBVL.BackEnd.WebApi.Modules.MasterData.Lenders.GetLenders;

public sealed class GetLendersEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapGet(GetLendersRoute.Pattern, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName(GetLendersRoute.Name)
            .WithDescription(GetLendersRoute.Description)
            .Produces<GetLendersResponse>();
    }

    private static async Task<IResult> Handle(
        HttpContext context,
        ISender sender,
        CancellationToken cancellationToken)
    {
        if (!context.User.HasPermission(Permissions.MasterDataLendersRead))
        {
            //return Results.Forbid();
        }

        var query = new GetLendersQuery();
        var response = await sender.Send(query, cancellationToken);

        return Results.Ok(response);
    }
}
