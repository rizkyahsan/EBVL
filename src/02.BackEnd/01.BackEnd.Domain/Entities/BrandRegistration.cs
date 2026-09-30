namespace EBVL.BackEnd.Domain.Entities;

public sealed class BrandRegistration : ModifiableEntity
{
    public Guid WorkflowCaseId { get; set; }
    public required string OwnerUsername { get; set; }
    public required string BrandName { get; set; }
    public required string ProductName { get; set; }
    public required string Group { get; set; }
    public required string FactoryCountry { get; set; }
    public required string Category { get; set; }
    public required string ProductDescription { get; set; }
    public DateTimeOffset? SubmittedAt { get; set; }
    public WorkflowCase WorkflowCase { get; set; } = default!;
    public ICollection<BrandInvitation> Invitations { get; set; } = new HashSet<BrandInvitation>();
}

public sealed class BrandInvitation : CreatableEntity
{
    public Guid BrandRegistrationId { get; set; }
    public required string Subject { get; set; }
    public required string Recipient { get; set; }
    public required string Cc { get; set; }
    public required string Body { get; set; }
    public required string SentBy { get; set; }
    public DateTimeOffset SentAt { get; set; }
    public BrandInvitationResponse? Response { get; set; }
    public DateTimeOffset? RespondedAt { get; set; }
    public string? RespondedBy { get; set; }
    public BrandRegistration BrandRegistration { get; set; } = default!;
}
