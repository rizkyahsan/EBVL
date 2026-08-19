using EBVL.BackEnd.Logics.Modules.Common.FileStorages.DownloadFileStorage;
using EBVL.Shared.Dto.Modules.Common.FileStorages;
using EBVL.Shared.Dto.Modules.Common.FileStorages.DownloadFileStorage;

namespace EBVL.BackEnd.WebApi.Modules.Common.FileStorages.DownloadFileStorage;

public sealed class DownloadFileStorageEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapGet(DownloadFileStorageRoute.Pattern, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName(DownloadFileStorageRoute.Name)
            .WithDescription(DownloadFileStorageRoute.Description)
            .Produces<DownloadFileStorageResponse>();
    }

    private static async Task<IResult> Handle(
        [FromRoute] Guid fileStorageId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new DownloadFileStorageQuery
        {
            FileStorageId = fileStorageId
        };

        var response = await sender.Send(query, cancellationToken);

        return Results.Ok(response);
    }
}
