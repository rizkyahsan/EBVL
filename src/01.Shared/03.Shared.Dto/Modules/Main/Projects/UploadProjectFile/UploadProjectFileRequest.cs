using EBVL.Shared.Dto.Common.FileStorages;

namespace EBVL.Shared.Dto.Modules.Main.Projects.UploadProjectFile;

public record UploadProjectFileRequest
{
    public required Guid Id { get; set; }

    public required FileItem? File { get; set; }
}

public sealed class UploadProjectFileRequestValidator : AbstractValidatorBase<UploadProjectFileRequest>
{
    public UploadProjectFileRequestValidator()
    {
        _ = RuleFor(x => x.Id)
            .NotEmpty();

        _ = RuleFor(x => x.File)
            .NotEmpty();

        // Validate uploaded file
        _ = When(x => x.File is not null, () =>
        {
            _ = RuleFor(x => x.File!)
                .SetValidator(new FileItemValidator());
        });
    }
}
