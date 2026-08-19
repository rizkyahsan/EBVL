using EBVL.BackEnd.Logics.Modules.MasterData.EmailTemplates.DeleteEmailTemplate;
using EBVL.Shared.Dto.Modules.MasterData.EmailTemplates;
using EBVL.Shared.Dto.Modules.MasterData.EmailTemplates.DeleteEmailTemplate;

namespace EBVL.BackEnd.WebApi.Modules.MasterData.EmailTemplates.DeleteEmailTemplate;

public sealed class DeleteEmailTemplateEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapDelete(DeleteEmailTemplateRoute.Pattern, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName(DeleteEmailTemplateRoute.Name)
            .WithDescription(DeleteEmailTemplateRoute.Description)
            .Produces(StatusCodes.Status204NoContent);
    }

    private static async Task<IResult> Handle(
        [FromRoute] Guid emailTemplateId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new DeleteEmailTemplateCommand
        {
            EmailTemplateId = emailTemplateId
        };

        await sender.Send(command, cancellationToken);

        return Results.NoContent();
    }
}
