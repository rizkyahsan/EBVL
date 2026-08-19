using EBVL.BackEnd.Logics.Modules.MasterData.Lenders.UpdateLender;
using EBVL.Shared.Dto.Modules.MasterData.Lenders;
using EBVL.Shared.Dto.Modules.MasterData.Lenders.UpdateLender;

namespace EBVL.BackEnd.WebApi.Modules.MasterData.Lenders.UpdateLender;

public sealed class UpdateLenderEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapPatch(UpdateLenderRoute.Pattern, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName(UpdateLenderRoute.Name)
            .WithDescription(UpdateLenderRoute.Description)
            .Produces(StatusCodes.Status204NoContent);
    }

    private static async Task<IResult> Handle(
        [FromRoute] Guid lenderId,
        HttpContext context,
        UpdateLenderCommand command,
        ISender sender,
        CancellationToken cancellationToken)
    {
        if (!context.User.HasPermission(Permissions.MasterDataLendersWrite))
        {
            //return Results.Forbid();
        }

        if (lenderId != command.LenderId)
        {
            throw ExceptionFor.Mismatch(nameof(lenderId), lenderId, nameof(command.LenderId), command.LenderId);
        }

        await sender.Send(command, cancellationToken);

        return Results.NoContent();
    }
}
