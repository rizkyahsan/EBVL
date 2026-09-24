namespace EBVL.Shared.Dto.Modules.Main.Vendor.RequestRegistrations.GetRequestRegistrations;

public sealed record GetRequestRegistrationsResponse : PaginatedListResponse<RequestRegistrationItem>
{
    public int ProcessCount { get; init; }
    public int ApproveCount { get; init; }
    public int RejectCount { get; init; }
}
