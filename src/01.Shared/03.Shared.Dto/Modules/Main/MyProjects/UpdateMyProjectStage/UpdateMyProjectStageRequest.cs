namespace EBVL.Shared.Dto.Modules.Main.MyProjects.UpdateMyProjectStage;

public record UpdateMyProjectStageRequest
{
    public required Guid Id { get; set; }

    public required Guid ProjectId { get; init; }
    public required Guid ProjectLenderId { get; set; }
    public required Guid ProjectStageId { get; set; }

    public required bool IsSubmitted { get; set; } = false;
    public string Remarks { get; set; } = string.Empty;

    public required List<UpdateMyProjectLenderReqFileRequest> ProjectLenderReqFiles { get; set; }
}

public sealed class UpdateMyProjectStageRequestValidator : AbstractValidatorBase<UpdateMyProjectStageRequest>
{
    public UpdateMyProjectStageRequestValidator()
    {
        _ = RuleFor(x => x.Id)
            .NotEmpty();

        _ = RuleFor(x => x.ProjectId)
            .NotEmpty();

        _ = RuleFor(x => x.ProjectLenderId)
            .NotEmpty();

        _ = RuleFor(x => x.ProjectStageId)
            .NotEmpty();

        _ = When(x => !string.IsNullOrEmpty(x.Remarks), () =>
        {
            _ = RuleFor(x => x.Remarks)
                .NotEmpty()
                .MinimumLength(CommonMinimumLengthFor.Notes);
        });

        _ = RuleFor(x => x.ProjectLenderReqFiles)
            .Must(files =>
                files.All(x =>
                    x.FileStorageId != Guid.Empty ||
                    x.File is not null));

        _ = RuleForEach(m => m.ProjectLenderReqFiles)
                .SetValidator(new UpdateMyProjectLenderReqFileRequestValidator());
    }
}
