using EBVL.BackEnd.Logics.Common.Services.FileStorageDb;
using EBVL.Shared.Dto.Modules.Common.FileStorages.DownloadFileStorage;
using Pertamina.Services.CurrentUser;

namespace EBVL.BackEnd.Logics.Modules.Common.FileStorages.DownloadFileStorage;

[AuthorizeRequest]
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

public sealed class DownloadFileStorageQueryHandler(IDatabaseService databaseService,
    ICurrentUserService currentUserService,
    IFileStorageDbService fileStorageDbService)
    : IRequestHandler<DownloadFileStorageQuery, DownloadFileStorageResponse>
{
    public async Task<DownloadFileStorageResponse> Handle(DownloadFileStorageQuery request, CancellationToken cancellationToken)
    {
        var username = currentUserService.Username
            ?? throw ExceptionFor.NotAuthenticated();

        var fileStorage = await databaseService.FileStorages
            .Where(x => !x.IsDeleted && x.Id == request.FileStorageId)
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw ExceptionFor.EntityNotFound(FileStoragesDisplayTextFor.FileStorage, CommonDisplayTextFor.Id, request.FileStorageId);

        var fileContent = await fileStorageDbService.ReadAsync(request.FileStorageId, cancellationToken);

        return new DownloadFileStorageResponse
        {
            FileName = fileStorage.OriginalFileName,
            FileContentType = fileStorage.FileContentType,
            FileContent = fileContent,
        };
    }
}
