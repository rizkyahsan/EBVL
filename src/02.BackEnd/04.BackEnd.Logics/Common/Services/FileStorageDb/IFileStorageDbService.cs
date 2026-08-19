using Azure.Storage.Blobs.Models;
using EBVL.Shared.Dto.Common.FileStorages;

namespace EBVL.BackEnd.Logics.Common.Services.FileStorageDb;

public interface IFileStorageDbService
{
    public Task EnsureContainerExistsAsync(PublicAccessType accessType = PublicAccessType.None, CancellationToken cancellationToken = default);
    public Task<FileStorage> CreateAsync(FileItem file, CancellationToken cancellationToken = default);

    public Task<byte[]> ReadAsync(Guid id, CancellationToken cancellationToken = default);

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    public Task UpdateAsync(FileStorage fileStorage, CancellationToken cancellationToken = default);

    public Task<FileStorage> CopyAsync(Guid id, CancellationToken cancellationToken = default);
}

