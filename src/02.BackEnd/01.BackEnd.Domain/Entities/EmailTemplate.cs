namespace EBVL.BackEnd.Domain.Entities;

public sealed class EmailTemplate : ModifiableEntity
{
    public required string Module { get; set; }

    public required string Action { get; set; }

    public string DefaultTo { get; set; } = string.Empty;

    public string DefaultCc { get; set; } = string.Empty;

    public required string Subject { get; set; }

    public required string Content { get; set; }
}
