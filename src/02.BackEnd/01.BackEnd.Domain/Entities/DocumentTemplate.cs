namespace EBVL.BackEnd.Domain.Entities;

public sealed class DocumentTemplate : ModifiableEntity
{
    public required string Name { get; set; }
    public string? Alias { get; set; }
    public bool IsMandatory { get; set; }
    public ICollection<VendorDocument> VendorDocuments { get; set; } = [];
}
