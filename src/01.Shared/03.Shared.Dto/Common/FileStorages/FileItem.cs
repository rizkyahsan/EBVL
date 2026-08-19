namespace EBVL.Shared.Dto.Common.FileStorages;

public sealed record FileItem : SendFileRequest
{
}

public sealed class FileItemValidator : AbstractValidatorBase<FileItem>
{
    public FileItemValidator()
    {
        Include(new SendFileRequestValidator());
    }
}
