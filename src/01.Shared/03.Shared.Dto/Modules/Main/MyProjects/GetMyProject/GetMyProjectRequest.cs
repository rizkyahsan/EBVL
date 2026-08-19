namespace EBVL.Shared.Dto.Modules.Main.MyProjects.GetMyProject;

public record GetMyProjectRequest
{
    public required Guid Id { get; init; }
}

public sealed class GetMyProjectRequestValidator : AbstractValidatorBase<GetMyProjectRequest>
{
    public GetMyProjectRequestValidator()
    {
        _ = RuleFor(x => x.Id)
            .NotEmpty();
    }
}
