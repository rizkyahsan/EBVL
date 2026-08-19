namespace EBVL.Shared.Dto.Modules.Main.Projects.CompleteProjectStage;

public record CompleteProjectLenderReqRequest
{
    public required Guid Id { get; init; }

    public required string StatusCode { get; set; }
}

public sealed class CompleteProjectLenderReqRequestValidator : AbstractValidatorBase<CompleteProjectLenderReqRequest>
{
    public CompleteProjectLenderReqRequestValidator()
    {
        _ = RuleFor(x => x.Id)
            .NotEmpty();

        _ = RuleFor(x => x.StatusCode)
            .Must(x =>
                x is StatusesCodeFor.ProjectLenderReqAccept or
                StatusesCodeFor.ProjectLenderReqReject);
    }
}
