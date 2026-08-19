namespace EBVL.Shared.Dto.Modules.Common.FileStorages.DownloadFileStorage;

public record DownloadFileStorageRequest
{
    public required Guid FileStorageId { get; init; }
}

public sealed class DownloadFileStorageRequestValidator : AbstractValidatorBase<DownloadFileStorageRequest>
{
    public DownloadFileStorageRequestValidator()
    {
        _ = RuleFor(x => x.FileStorageId)
            .NotEmpty();
    }
}
