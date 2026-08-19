namespace EBVL.Shared.Dto.Modules.Main.Projects.GetProject;

public record GetProjectRequest
{
    public required Guid Id { get; init; }
}

public sealed class GetProjectRequestValidator : AbstractValidatorBase<GetProjectRequest>
{
    public GetProjectRequestValidator()
    {
        _ = RuleFor(x => x.Id)
            .NotEmpty();
    }
}
