using EBVL.BackEnd.Logics.Modules.MasterData.EmailTemplates.GetEmailTemplate;
using EBVL.Shared.Dto.Modules.MasterData.EmailTemplates;
using EBVL.Shared.Dto.Modules.MasterData.EmailTemplates.GetEmailTemplate;

namespace EBVL.BackEnd.WebApi.Modules.MasterData.EmailTemplates.GetEmailTemplates;

public sealed class GetEmailTemplateEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapGet(GetEmailTemplateRoute.Pattern, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName(GetEmailTemplateRoute.Name)
            .WithDescription(GetEmailTemplateRoute.Description)
            .Produces<GetEmailTemplateResponse>();
    }

    private static async Task<IResult> Handle(
        [FromRoute] Guid emailTemplateId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new GetEmailTemplateQuery
        {
            EmailTemplateId = emailTemplateId
        };

        var response = await sender.Send(query, cancellationToken);

        return Results.Ok(response);
    }
}
