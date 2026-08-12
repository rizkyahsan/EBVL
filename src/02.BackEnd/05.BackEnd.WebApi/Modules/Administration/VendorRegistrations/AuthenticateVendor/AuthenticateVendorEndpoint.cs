using EBVL.BackEnd.Logics.Modules.Administration.VendorRegistrations.AuthenticateVendor;
using EBVL.Shared.Dto.Modules.Administration.VendorRegistrations;
using EBVL.Shared.Dto.Modules.Administration.VendorRegistrations.AuthenticateVendor;

namespace EBVL.BackEnd.WebApi.Modules.Administration.VendorRegistrations.AuthenticateVendor;

public sealed class AuthenticateVendorEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapPost(AuthenticateVendorRoute.Pattern, Handle)
            .AllowAnonymous()
            .WithTags(RouteConfig.Tag)
            .WithName(AuthenticateVendorRoute.Name)
            .WithDescription(AuthenticateVendorRoute.Description)
            .Produces<AuthenticateVendorResponse>()
            .Produces(StatusCodes.Status401Unauthorized);
    }

    private static async Task<IResult> Handle(AuthenticateVendorRequest request, ISender sender, CancellationToken cancellationToken)
    {
        var command = new AuthenticateVendorCommand
        {
            EmailAddress = request.EmailAddress,
            Password = request.Password
        };
        try
        {
            return Results.Ok(await sender.Send(command, cancellationToken));
        }
        catch (UnauthorizedAccessException)
        {
            return Results.Unauthorized();
        }
    }
}
