using EBVL.Shared.Dto.Common.FileStorages;

namespace EBVL.Shared.Dto.Modules.Main.MyProjects.UpdateMyProjectStage;

public record UpdateMyProjectLenderReqFileRequest
{
    public required Guid Id { get; set; }

    public required Guid ProjectReqId { get; set; }
    public required Guid FileStorageId { get; set; }
    public required FileItem? File { get; set; }
}

public sealed class UpdateMyProjectLenderReqFileRequestValidator : AbstractValidatorBase<UpdateMyProjectLenderReqFileRequest>
{
    public UpdateMyProjectLenderReqFileRequestValidator()
    {
        _ = RuleFor(x => x.ProjectReqId)
            .NotEmpty();

        _ = RuleFor(x => x)
            .Must(x => (x.FileStorageId == Guid.Empty && x.File is not null)
            || (x.FileStorageId != Guid.Empty && x.File is null))
            .WithMessage("Either upload a file or provide FileStorageId, but not both.");

        // Validate uploaded file
        _ = When(x => x.File is not null, () =>
        {
            _ = RuleFor(x => x.File!)
                .SetValidator(new FileItemValidator());
        });
    }
}
