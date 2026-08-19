namespace EBVL.Shared.Dto.Modules.Main.Projects.GetProjectVerifies;

public record GetProjectVerifiesRequest
{
    public Guid LenderId { get; init; } = Guid.Empty;
}
