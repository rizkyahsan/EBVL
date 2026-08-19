using EBVL.Shared.Dto.Common.FileStorages;

namespace EBVL.Shared.Dto.Modules.Main.Projects.UpdateProjectStage;

public record UpdateProjectAttachmentRequest
{
    public required Guid Id { get; set; }

    public required string AttachmentName { get; set; }
    public required string AttachmentDesc { get; set; }
    public required int AttachmentSortNo { get; set; }
    public required Guid FileStorageId { get; set; }
    public required FileItem? File { get; set; }
}

public sealed class UpdateProjectAttachmentRequestValidator : AbstractValidatorBase<UpdateProjectAttachmentRequest>
{
    public UpdateProjectAttachmentRequestValidator()
    {
        _ = RuleFor(x => x.AttachmentName)
            .NotEmpty()
            .MinimumLength(ProjectAttachmentsMinimumLengthFor.Name)
            .MaximumLength(ProjectAttachmentsMaximumLengthFor.Name);

        _ = RuleFor(x => x.AttachmentDesc)
            .NotEmpty();

        _ = RuleFor(x => x.AttachmentSortNo)
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
