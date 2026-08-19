namespace EBVL.BackEnd.Domain.Entities;

public sealed class Lender : ModifiableEntity
{
    public required string Name { get; set; }
    public required string Address { get; set; }
    public required Guid CountryId { get; set; }
    [Encrypted]
    [ExcludeFromAudit]
    public required string PhoneNumber { get; set; }
    public string FullPhoneNumber => $"{Country.PhoneCode} {PhoneNumber}";
    public required string EmailAddress { get; set; }
    public required string Website { get; set; }

    public Country Country { get; set; } = default!;

    public ICollection<User> Users { get; set; } = new HashSet<User>();
    public ICollection<ProjectLender> ProjectLenders { get; set; } = new HashSet<ProjectLender>();

}
