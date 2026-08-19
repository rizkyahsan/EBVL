using EBVL.BackEnd.Logics.Modules.MasterData.Lenders.DeleteLender;
using EBVL.Shared.Dto.Modules.MasterData.Lenders;
using EBVL.Shared.Dto.Modules.MasterData.Lenders.DeleteLender;

namespace EBVL.BackEnd.WebApi.Modules.MasterData.Lenders.DeleteLender;

public sealed class DeleteLenderEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapDelete(DeleteLenderRoute.Pattern, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName(DeleteLenderRoute.Name)
            .WithDescription(DeleteLenderRoute.Description)
            .Produces(StatusCodes.Status204NoContent);
    }

    private static async Task<IResult> Handle(
        [FromRoute] Guid lenderId,
        HttpContext context,
        ISender sender,
        CancellationToken cancellationToken)
    {
        if (!context.User.HasPermission(Permissions.MasterDataLendersWrite))
        {
            //return Results.Forbid();
        }

        var command = new DeleteLenderCommand
        {
            LenderId = lenderId
        };

        await sender.Send(command, cancellationToken);

        return Results.NoContent();
    }
}
