using EBVL.BackEnd.Logics.Modules.MasterData.EmailTemplates.UpdateEmailTemplate;
using EBVL.Shared.Dto.Modules.MasterData.EmailTemplates;
using EBVL.Shared.Dto.Modules.MasterData.EmailTemplates.UpdateEmailTemplate;

namespace EBVL.BackEnd.WebApi.Modules.MasterData.EmailTemplates.UpdateEmailTemplate;

public sealed class UpdateEmailTemplateEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapPatch(UpdateEmailTemplateRoute.Pattern, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName(UpdateEmailTemplateRoute.Name)
            .WithDescription(UpdateEmailTemplateRoute.Description)
            .Produces(StatusCodes.Status204NoContent);
    }

    private static async Task<IResult> Handle(
        [FromRoute] Guid emailTemplateId,
        UpdateEmailTemplateCommand command,
        ISender sender,
        CancellationToken cancellationToken)
    {
        if (emailTemplateId != command.EmailTemplateId)
        {
            throw ExceptionFor.Mismatch(nameof(emailTemplateId), emailTemplateId, nameof(command.EmailTemplateId), command.EmailTemplateId);
        }

        await sender.Send(command, cancellationToken);

        return Results.NoContent();
    }
}
