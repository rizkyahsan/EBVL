using EBVL.BackEnd.Logics.Modules.Examples.Cryptography.ConvertToCiphertext;
using EBVL.Shared.Dto.Modules.Examples.Cryptography;
using EBVL.Shared.Dto.Modules.Examples.Cryptography.ConvertToCiphertext;

namespace EBVL.BackEnd.WebApi.Modules.Examples.Cryptography.ConvertToCiphertext;

public sealed class ConvertToCiphertextEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapPost(ConvertToCiphertextRoute.Pattern, Handle)
            .AllowAnonymous()
            .WithTags(RouteConfig.Tag)
            .WithName(ConvertToCiphertextRoute.Name)
            .WithDescription(ConvertToCiphertextRoute.Description)
            .Produces<ConvertToCiphertextResponse>();
    }

    private static IResult Handle(
        ConvertToCiphertextCommand command,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var response = sender.Send(command, cancellationToken);

        return Results.Ok(response);
    }
}
