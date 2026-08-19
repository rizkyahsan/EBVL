using System.Security.Cryptography;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using EBVL.Shared.Dto.Common.FileStorages;
using Microsoft.Extensions.Configuration;
using Pertamina.Services.FileStorage;
namespace EBVL.BackEnd.Logics.Common.Services.FileStorageDb;

public sealed class FileStorageDbService(
    IConfiguration configuration,
    IDatabaseService databaseService,
    IFileStorageService fileStorageService)
    : IFileStorageDbService
{

    private readonly BlobServiceClient _blobServiceClient =
         new(configuration["AzureBlob:Connection"]);

    private BlobContainerClient GetContainerClient()
    {
        return _blobServiceClient.GetBlobContainerClient(configuration["AzureBlob:BlobContainerName"]);
    }

    // 🆕 Ensure container exists
    public async Task EnsureContainerExistsAsync(PublicAccessType accessType = PublicAccessType.None, CancellationToken cancellationToken = default)
    {
        var containerClient = GetContainerClient();
        _ = await containerClient.CreateIfNotExistsAsync(accessType, cancellationToken: cancellationToken);
        #region test create container
        //// 2. Generate your unique, lowercase name for a new runtime container
        //var containerName = "container-" + Guid.NewGuid().ToString().ToLower();
        //BlobContainerClient randomContainerClient;

        //try
        //{
        //    // 3. Create the random container directly from your class's private client instance
        //    randomContainerClient = await _blobServiceClient.CreateBlobContainerAsync(containerName, accessType, cancellationToken: cancellationToken);

        //    Console.WriteLine("Created container {0}", randomContainerClient.Name);
        //}
        //catch (RequestFailedException e)
        //{
        //    Console.WriteLine("HTTP error code {0}: {1}", e.Status, e.ErrorCode);
        //    Console.WriteLine(e.Message);
        //    throw;
        //}
        #endregion
        #region test upload dummy.text
        //// Get a blob reference inside the container
        //var blobClient = containerClient.GetBlobClient("dummy.txt");

        //// Upload some dummy content
        //using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("This is a dummy file."));
        //_ = await blobClient.UploadAsync(stream, overwrite: true, cancellationToken);

        //// List all blobs in the container
        //await foreach (var blobItem in containerClient.GetBlobsAsync(cancellationToken: cancellationToken))
        //{
        //    Console.WriteLine($"Blob name: {blobItem.Name}");
        //}

        //// Delete only dummy.txt
        //_ = await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
        //Console.WriteLine("Deleted blob: dummy.txt");
        #endregion
    }

    public async Task<FileStorage> CreateAsync(FileItem file, CancellationToken cancellationToken = default)
    {
        return await CreateAsync(file.FileName, file.ContentType, file.FileContent, cancellationToken);
    }

    public async Task<byte[]> ReadAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var fileStorage = await GetAsync(id, cancellationToken);

        switch (fileStorage.StorageType)
        {
            case FileStorageType.Database:
                if (fileStorage.FileData is null)
                {
                    throw new InvalidOperationException("FileData not found.");
                }

                return fileStorage.FileData;

            case FileStorageType.LocalStorage:
                return await fileStorageService.ReadAsync(fileStorage.StoredFileName, cancellationToken);

            case FileStorageType.AzureBlob:
                return await ReadBlobAsync(fileStorage.BlobName!, cancellationToken);

            case FileStorageType.AwsS3:
                throw new NotSupportedException("AWS S3 storage not yet implemented.");

            default:
                throw new NotSupportedException($"Unsupported storage type: {fileStorage.StorageType}");
        }
    }

    public async Task UpdateAsync(FileStorage fileStorage, CancellationToken cancellationToken = default)
    {
        _ = databaseService.FileStorages.Update(fileStorage);
        _ = await databaseService.SaveAsync(nameof(FileStorage), cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var fileStorage = await GetAsync(id, cancellationToken);

        switch (fileStorage.StorageType)
        {
            case FileStorageType.LocalStorage:
                if (!string.IsNullOrWhiteSpace(fileStorage.StoredFileName))
                {
                    await fileStorageService.DeleteAsync(fileStorage.StoredFileName, cancellationToken);
                }

                break;

            case FileStorageType.AzureBlob:
                if (!string.IsNullOrWhiteSpace(fileStorage.BlobName))
                {
                    var blobClient = GetContainerClient().GetBlobClient(fileStorage.BlobName);
                    _ = await blobClient.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots, cancellationToken: cancellationToken);
                }

                break;

            case FileStorageType.Database:
                // Nothing to physically delete, just mark as deleted
                break;
            case FileStorageType.AwsS3:
                break;
            default:
                throw new NotSupportedException($"Unsupported storage type: {fileStorage.StorageType}");
        }

        fileStorage.IsDeleted = true;
        _ = await databaseService.SaveAsync(nameof(FileStorage), cancellationToken);
    }

    public async Task<FileStorage> CopyAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var source = await GetAsync(id, cancellationToken);
        var bytes = await ReadAsync(id, cancellationToken);

        var file = new FileItem
        {
            FileName = source.OriginalFileName,
            ContentType = source.FileContentType,
            FileContent = bytes
        };

        return await CreateAsync(file, cancellationToken);
    }

    public string GetRealUrl(string blobName)
    {
        var blobClient = GetContainerClient().GetBlobClient(blobName);
        return blobClient.Uri.ToString();
    }

    public string GetSecureUrl(string blobName, int expiryMinutes = 5)
    {
        var blobClient = GetContainerClient().GetBlobClient(blobName);

        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = GetContainerClient().Name,
            BlobName = blobName,
            Resource = FileStoragesResourceFor.B.ToLower(),
            ExpiresOn = DateTimeOffset.Now.AddMinutes(expiryMinutes)
        };

        sasBuilder.SetPermissions(BlobSasPermissions.Read);

        return blobClient.GenerateSasUri(sasBuilder).ToString();
    }

    private async Task<FileStorage> CreateAsync(string originalFileName, string contentType, byte[] fileData,
        CancellationToken cancellationToken = default)
    {
        var fileSize = fileData.Length;
        //var storageType = FileStorageType.LocalStorage; // adjust logic if needed
        var storageType = FileStorageType.AzureBlob; // adjust logic if needed

        string? storedFileName = null;
        string? blobName = null;
        byte[]? databaseFileData = null;

        switch (storageType)
        {
            case FileStorageType.Database:
                databaseFileData = fileData;
                break;

            case FileStorageType.AzureBlob:
                blobName = $"{Guid.NewGuid()}{Path.GetExtension(originalFileName)}";
                var blobClient = GetContainerClient().GetBlobClient(blobName);
                using (var stream = new MemoryStream(fileData))
                {
                    _ = await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = contentType }, cancellationToken: cancellationToken);
                }

                #region get all files on azure bloobs
                //var listitems = GetContainerClient().GetBlobsAsync(cancellationToken: cancellationToken);
                //await foreach (var blobItem in listitems)
                //{
                //    Console.WriteLine($"Blob name: {blobItem.Name}");
                //}
                #endregion
                break;

            case FileStorageType.LocalStorage:
                storedFileName = $"{Guid.NewGuid()}{Path.GetExtension(originalFileName)}";
                await fileStorageService.CreateAsync(storedFileName, fileData, cancellationToken);
                break;
            case FileStorageType.AwsS3:
                break;
            default:
                throw new NotSupportedException($"Unsupported storage type: {storageType}");
        }

        var fileStorage = new FileStorage
        {
            OriginalFileName = originalFileName,
            StoredFileName = storedFileName ?? string.Empty,
            BlobName = blobName,
            ContainerName = GetContainerClient().Name,
            FileContentType = contentType,
            FileSize = fileSize,
            FileData = databaseFileData,
            FileExtension = Path.GetExtension(originalFileName),
            StorageType = storageType,
            FileHash = Convert.ToHexString(SHA256.HashData(fileData))
        };

        _ = await databaseService.FileStorages.AddAsync(fileStorage, cancellationToken);
        return fileStorage;
    }

    private async Task<FileStorage> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await databaseService.FileStorages.SingleAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
    }

    private async Task<byte[]> ReadBlobAsync(string blobName, CancellationToken cancellationToken)
    {
        var blobClient = GetContainerClient().GetBlobClient(blobName);
        var download = await blobClient.DownloadContentAsync(cancellationToken);
        return download.Value.Content.ToArray();
    }
}
