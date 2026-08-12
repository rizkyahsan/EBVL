using EBVL.BackEnd.Logics.Modules.Administration.VendorRegistrations.RegisterVendor;
using EBVL.Shared.Dto.Modules.Administration.VendorRegistrations;
using EBVL.Shared.Dto.Modules.Administration.VendorRegistrations.RegisterVendor;

namespace EBVL.BackEnd.WebApi.Modules.Administration.VendorRegistrations.RegisterVendor;

public sealed class RegisterVendorEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapPost(RegisterVendorRoute.Pattern, Handle)
            .AllowAnonymous()
            .WithTags(RouteConfig.Tag)
            .WithName(RegisterVendorRoute.Name)
            .WithDescription(RegisterVendorRoute.Description)
            .Produces<RegisterVendorResponse>();
    }

    private static async Task<IResult> Handle(RegisterVendorRequest request, ISender sender, CancellationToken cancellationToken)
    {
        return Results.Ok(await sender.Send(new RegisterVendorCommand(request), cancellationToken));
    }
}
