using EBVL.Shared.Enums;

namespace EBVL.Shared.Dto.Modules.Main.BrandRegistrations.Common;

public sealed record BrandRegistrationItem(
    Guid Id,
    string BrandName,
    string ProductName,
    string Group,
    string FactoryCountry,
    string Category,
    string ProductDescription,
    BrandRegistrationStatus Status,
    DateTimeOffset? SubmittedAt,
    DateTimeOffset LastUpdatedAt,
    string RowVersion,
    IReadOnlyList<WorkflowHistoryItem> History,
    BrandInvitationItem? Invitation);

public sealed record WorkflowHistoryItem(Guid Id, BrandRegistrationStatus FromStatus, BrandRegistrationStatus ToStatus,
    BrandRegistrationAction Action, string ActorUsername, string ActorRole, string? Note, DateTimeOffset OccurredAt);

public sealed record BrandInvitationItem(Guid Id, string Subject, string Recipient, string Cc, string Body,
    DateTimeOffset SentAt, string SentBy, BrandInvitationResponse? Response, DateTimeOffset? RespondedAt, string? RespondedBy);
