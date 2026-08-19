namespace EBVL.Shared.Dto.Modules.Main.Projects.UpdateProjectStage;

public record UpdateProjectReqRequest
{
    public required Guid Id { get; set; }

    public required string ReqName { get; set; }
    public required string ReqDesc { get; set; }
    public required int ReqSortNo { get; set; }
    public required bool IsRequired { get; set; }
}

public sealed class UpdateProjectReqRequestValidator : AbstractValidatorBase<UpdateProjectReqRequest>
{
    public UpdateProjectReqRequestValidator()
    {
        _ = RuleFor(x => x.ReqName)
            .NotEmpty()
            .MinimumLength(ProjectReqsMinimumLengthFor.Name)
            .MaximumLength(ProjectReqsMaximumLengthFor.Name);

        _ = RuleFor(x => x.ReqDesc)
            .NotEmpty();

        _ = RuleFor(x => x.ReqSortNo)
            .NotEmpty();
    }
}

