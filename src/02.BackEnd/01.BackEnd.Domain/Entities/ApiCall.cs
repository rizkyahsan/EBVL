namespace EBVL.BackEnd.Domain.Entities;

[ExcludeFromAudit]
public sealed class ApiCall : CreatableEntity
{
    public required string ServiceName { get; set; }
    public required string ServiceProvider { get; set; }
    public required string ServiceCategory { get; set; }
    public required string RequestUrl { get; set; }
    public required string RequestMethod { get; set; }
    public required string? RequestParameters { get; set; }
    public required ushort ResponseStatusCode { get; set; }
    public required string? ResponseHeaders { get; set; }
    public required string? ResponseContent { get; set; }
    public required string? ErrorMessage { get; set; }
}
