using EBVL.BackEnd.Logics.Modules.MasterData.Users.CheckVerificationUser;
using EBVL.Shared.Dto.Modules.MasterData.Users;
using EBVL.Shared.Dto.Modules.MasterData.Users.CheckVerificationUser;

namespace EBVL.BackEnd.WebApi.Modules.MasterData.Users.CheckVerificationUser;

public sealed class CheckVerificationUserEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapGet(CheckVerificationUserRoute.Pattern, Handle)
            .AllowAnonymous()
            .WithTags(RouteConfig.Tag)
            .WithName(CheckVerificationUserRoute.Name)
            .WithDescription(CheckVerificationUserRoute.Description)
            .Produces<CheckVerificationUserResponse>();
    }

    private static async Task<IResult> Handle(
        [FromRoute] Guid userId,
        [FromQuery] string token,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new CheckVerificationUserQuery
        {
            UserId = userId,
            Token = token
        };

        var response = await sender.Send(query, cancellationToken);

        return Results.Ok(response);
    }
}

