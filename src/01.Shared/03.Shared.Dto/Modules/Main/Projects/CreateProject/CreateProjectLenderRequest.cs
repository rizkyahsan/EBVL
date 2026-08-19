namespace EBVL.Shared.Dto.Modules.Main.Projects.CreateProject;

public record CreateProjectLenderRequest
{
    public required Guid LenderId { get; set; }
}

public sealed class CreateProjectLenderRequestValidator : AbstractValidatorBase<CreateProjectLenderRequest>
{
    public CreateProjectLenderRequestValidator()
    {
        _ = RuleFor(x => x.LenderId)
            .NotEmpty();
    }
}
