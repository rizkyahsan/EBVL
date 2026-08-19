using EBVL.BackEnd.Logics.Modules.MasterData.EmailTemplates.AddEmailTemplate;
using EBVL.Shared.Dto.Modules.MasterData.EmailTemplates;
using EBVL.Shared.Dto.Modules.MasterData.EmailTemplates.AddEmailTemplate;
using EBVL.Shared.Dto.Modules.MasterData.EmailTemplates.GetEmailTemplate;

namespace EBVL.BackEnd.WebApi.Modules.MasterData.EmailTemplates.AddEmailTemplate;

public sealed class AddEmailTemplateEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapPost(AddEmailTemplateRoute.Pattern, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName(AddEmailTemplateRoute.Name)
            .WithDescription(AddEmailTemplateRoute.Description)
            .Produces<AddEmailTemplateResponse>(StatusCodes.Status201Created);
    }

    private static async Task<IResult> Handle(
        AddEmailTemplateCommand command,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var response = await sender.Send(command, cancellationToken);

        return Results.Created(GetEmailTemplateRoute.ResourceUri(response.Item.Id), response);
    }
}
