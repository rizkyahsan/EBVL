using EBVL.Shared.Enums;

namespace EBVL.Shared.Dto.Modules.Main.BrandRegistrations.GetBrandRegistrations;

public sealed record BrandRegistrationListItem(Guid Id, string BrandName, string ProductName, string Group,
    BrandRegistrationStatus Status, DateTimeOffset? SubmittedAt, DateTimeOffset LastUpdatedAt);

public sealed record GetBrandRegistrationsResponse(IReadOnlyList<BrandRegistrationListItem> Items);
