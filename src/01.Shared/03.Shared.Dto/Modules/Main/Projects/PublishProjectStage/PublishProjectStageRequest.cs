namespace EBVL.Shared.Dto.Modules.Main.Projects.PublishProjectStage;

public record PublishProjectStageRequest
{
    public required Guid Id { get; set; }
}

public sealed class PublishProjectStageRequestValidator : AbstractValidatorBase<PublishProjectStageRequest>
{
    public PublishProjectStageRequestValidator()
    {
        _ = RuleFor(x => x.Id)
            .NotEmpty();
    }
}
