namespace EBVL.Shared.Dto.Modules.Main.Projects.GetProjects;

public record GetProjectsRequest
{
    public Guid LenderId { get; init; } = Guid.Empty;

    public string StatusCode { get; init; } = string.Empty;
}
