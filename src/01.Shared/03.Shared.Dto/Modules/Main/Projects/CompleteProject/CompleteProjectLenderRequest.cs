using EBVL.Shared.Dto.Common.FileStorages;

namespace EBVL.Shared.Dto.Modules.Main.Projects.CompleteProject;

public record CompleteProjectLenderRequest
{
    public required Guid Id { get; set; }

    public string? Note { get; set; }
    public required string StatusCode { get; set; }
    public required Guid FileStorageId { get; set; }
    public required FileItem? File { get; set; }
}

public sealed class CompleteProjectLenderRequestValidator : AbstractValidatorBase<CompleteProjectLenderRequest>
{
    public CompleteProjectLenderRequestValidator()
    {
        _ = RuleFor(x => x.Id)
            .NotEmpty();

        _ = RuleFor(x => x.Note)
            .NotEmpty()
            .MinimumLength(CommonMinimumLengthFor.Notes);

        _ = RuleFor(x => x.StatusCode)
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
