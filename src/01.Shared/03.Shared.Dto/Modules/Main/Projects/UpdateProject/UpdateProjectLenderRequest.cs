using EBVL.Shared.Dto.Common.FileStorages;

namespace EBVL.Shared.Dto.Modules.Main.Projects.UpdateProject;

public record UpdateProjectLenderRequest
{
    public required Guid Id { get; set; }

    public required Guid LenderId { get; set; }
    public string? Note { get; set; } = null;
    public required string StatusCode { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public Guid? FileStorageId { get; set; } = null;
    public FileItem? File { get; set; } = null;
}

public sealed class UpdateProjectLenderRequestValidator : AbstractValidatorBase<UpdateProjectLenderRequest>
{
    public UpdateProjectLenderRequestValidator()
    {
        _ = RuleFor(x => x.LenderId)
            .NotEmpty();

        _ = RuleFor(x => x.StatusCode)
            .NotEmpty();

        _ = When(x => x.StatusCode is StatusesCodeFor.ProjectLenderLose, () =>
        {
            _ = RuleFor(x => x.Note)
                .NotEmpty()
                .MinimumLength(CommonMinimumLengthFor.Notes);

            _ = When(x => x.File is not null, () =>
            {
                _ = RuleFor(x => x.File!)
                    .SetValidator(new FileItemValidator());
            });
        });
    }
}
