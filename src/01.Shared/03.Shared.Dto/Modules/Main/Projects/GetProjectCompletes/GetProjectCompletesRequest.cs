namespace EBVL.Shared.Dto.Modules.Main.Projects.GetProjectCompletes;

public record GetProjectCompletesRequest
{
    public Guid LenderId { get; init; } = Guid.Empty;
}
