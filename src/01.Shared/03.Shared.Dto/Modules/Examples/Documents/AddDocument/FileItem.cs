namespace EBVL.Shared.Dto.Modules.Examples.Documents.AddDocument;

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
