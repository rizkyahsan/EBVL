using EBVL.BackEnd.Logics.Modules.Main.Vendor.RequestRegistrations.GetRequestRegistrations;
using EBVL.Shared.Dto.Modules.Main.Vendor.RequestRegistrations;
using EBVL.Shared.Dto.Modules.Main.Vendor.RequestRegistrations.GetRequestRegistrations;

namespace EBVL.BackEnd.WebApi.Modules.Main.Vendor.RequestRegistrations.GetRequestRegistrations;

public sealed class GetRequestRegistrationsEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapGet(GetRequestRegistrationsRoute.Pattern, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName(GetRequestRegistrationsRoute.Name)
            .WithDescription(GetRequestRegistrationsRoute.Description)
            .Produces<GetRequestRegistrationsResponse>();
    }

    private static async Task<IResult> Handle(
        ISender sender,
        [AsParameters] GetRequestRegistrationsQuery query,
        CancellationToken cancellationToken)
    {
        var response = await sender.Send(query, cancellationToken);
        return Results.Ok(response);
    }
}
