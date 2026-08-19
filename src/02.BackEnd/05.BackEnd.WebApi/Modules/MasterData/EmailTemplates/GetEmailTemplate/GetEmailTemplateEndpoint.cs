using EBVL.BackEnd.Logics.Modules.MasterData.EmailTemplates.GetEmailTemplates;
using EBVL.Shared.Dto.Modules.MasterData.EmailTemplates;
using EBVL.Shared.Dto.Modules.MasterData.EmailTemplates.GetEmailTemplates;

namespace EBVL.BackEnd.WebApi.Modules.MasterData.EmailTemplates.GetEmailTemplate;

public sealed class GetEmailTemplatesEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapGet(GetEmailTemplatesRoute.Pattern, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName(GetEmailTemplatesRoute.Name)
            .WithDescription(GetEmailTemplatesRoute.Description)
            .Produces<GetEmailTemplatesResponse>();
    }

    private static async Task<IResult> Handle(
        ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new GetEmailTemplatesQuery();
        var response = await sender.Send(query, cancellationToken);

        return Results.Ok(response);
    }
}
