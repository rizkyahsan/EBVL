using EBVL.Shared.Dto.Modules.Common.FileStorages.DownloadFileStorage;

namespace EBVL.FrontEnd.Logics.Modules.Common.FileStorages.DownloadFileStorage;

public sealed record DownloadFileStorageQuery : DownloadFileStorageRequest, IRequest<DownloadFileStorageResponse>
{
}

public sealed class DownloadFileStorageQueryValidator : AbstractValidatorBase<DownloadFileStorageQuery>
{
    public DownloadFileStorageQueryValidator()
    {
        Include(new DownloadFileStorageRequestValidator());
    }
}

public sealed class DownloadFileStorageQueryHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<DownloadFileStorageQuery, DownloadFileStorageResponse>
{
    public async Task<DownloadFileStorageResponse> Handle(DownloadFileStorageQuery request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(DownloadFileStorageRoute.ResourceUri(request.FileStorageId), Method.Get);

        return await backEndApiService.SendRequestAsync<DownloadFileStorageResponse>(restRequest, cancellationToken);
    }
}
