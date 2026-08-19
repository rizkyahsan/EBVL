namespace EBVL.Shared.Dto.Modules.Main.MyProjects.GetMyProjects;

public record GetMyProjectsRequest
{
    public required Guid LenderId { get; init; }
}

public sealed class GetMyProjectsRequestValidator : AbstractValidatorBase<GetMyProjectsRequest>
{
    public GetMyProjectsRequestValidator()
    {
        _ = RuleFor(x => x.LenderId)
            .NotEmpty();
    }
}
