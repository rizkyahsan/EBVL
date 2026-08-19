using EBVL.Shared.Dto.Common.FileStorages;

namespace EBVL.Shared.Dto.Modules.Main.Projects.CreateProject;

public record CreateProjectAttachmentRequest
{
    public required string AttachmentName { get; set; }
    public required string AttachmentDesc { get; set; }
    public required int AttachmentSortNo { get; set; }
    public required FileItem? File { get; set; }
}

public sealed class CreateProjectAttachmentRequestValidator : AbstractValidatorBase<CreateProjectAttachmentRequest>
{
    public CreateProjectAttachmentRequestValidator()
    {
        _ = RuleFor(x => x.AttachmentName)
            .NotEmpty()
            .MinimumLength(ProjectAttachmentsMinimumLengthFor.Name)
            .MaximumLength(ProjectAttachmentsMaximumLengthFor.Name);

        _ = RuleFor(x => x.AttachmentDesc)
            .NotEmpty();

        _ = RuleFor(x => x.AttachmentSortNo)
            .NotEmpty();

        _ = RuleFor(x => x.File)
            .NotEmpty();

        _ = When(x => x.File is not null, () =>
        {
            _ = RuleFor(x => x.File!)
                .SetValidator(new FileItemValidator());
        });
    }
}
