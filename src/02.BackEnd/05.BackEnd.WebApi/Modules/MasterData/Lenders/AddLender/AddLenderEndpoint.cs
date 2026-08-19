using EBVL.BackEnd.Logics.Modules.MasterData.Lenders.AddLender;
using EBVL.Shared.Dto.Modules.MasterData.Lenders;
using EBVL.Shared.Dto.Modules.MasterData.Lenders.AddLender;
using EBVL.Shared.Dto.Modules.MasterData.Lenders.GetLender;

namespace EBVL.BackEnd.WebApi.Modules.MasterData.Lenders.AddLender;

public sealed class AddLenderEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapPost(AddLenderRoute.Pattern, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName(AddLenderRoute.Name)
            .WithDescription(AddLenderRoute.Description)
            .Produces<AddLenderResponse>(StatusCodes.Status201Created);
    }

    private static async Task<IResult> Handle(
        HttpContext context,
        AddLenderCommand command,
        ISender sender,
        CancellationToken cancellationToken)
    {
        if (!context.User.HasPermission(Permissions.MasterDataLendersWrite))
        {
            //return Results.Forbid();
        }

        var response = await sender.Send(command, cancellationToken);

        return Results.Created(GetLenderRoute.ResourceUri(response.Item.Id), response);
    }
}
