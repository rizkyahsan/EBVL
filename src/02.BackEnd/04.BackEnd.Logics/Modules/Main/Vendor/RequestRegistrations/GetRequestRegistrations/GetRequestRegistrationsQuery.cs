using EBVL.Shared.Dto.Modules.Main.Vendor.RequestRegistrations;
using EBVL.Shared.Dto.Modules.Main.Vendor.RequestRegistrations.GetRequestRegistrations;
using Pertamina.Common.Dto.Enums;

namespace EBVL.BackEnd.Logics.Modules.Main.Vendor.RequestRegistrations.GetRequestRegistrations;

[AuthorizeRequest]
public sealed record GetRequestRegistrationsQuery : GetRequestRegistrationsRequest, IRequest<GetRequestRegistrationsResponse>;

public sealed class GetRequestRegistrationsQueryValidator : AbstractValidatorBase<GetRequestRegistrationsQuery>
{
    public GetRequestRegistrationsQueryValidator()
    {
        Include(new GetRequestRegistrationsRequestValidator());
    }
}

public sealed class GetRequestRegistrationsQueryHandler(IDatabaseService databaseService)
    : IRequestHandler<GetRequestRegistrationsQuery, GetRequestRegistrationsResponse>
{
    public async Task<GetRequestRegistrationsResponse> Handle(GetRequestRegistrationsQuery request, CancellationToken cancellationToken)
    {
        var query = databaseService.VendorRegistrations
            .AsNoTracking()
            .Where(x => !x.IsDeleted
                && x.Status != VendorRegistrationStatus.Draft
                && x.SubmittedAt != null);

        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            query = query.Where(x => x.CompanyName.Contains(request.SearchText));
        }

        var processCount = await query.CountAsync(x => x.Status == VendorRegistrationStatus.Submitted
            || x.Status == VendorRegistrationStatus.OnReview
            || x.Status == VendorRegistrationStatus.AdminMSAI
            || x.Status == VendorRegistrationStatus.ApprovedByAdminMSAI
            || x.Status == VendorRegistrationStatus.ReviewBySeniorManagerMSAI, cancellationToken);
        var approveCount = await query.CountAsync(x => x.Status == VendorRegistrationStatus.ApprovedBySeniorManagerMSAI, cancellationToken);
        var rejectCount = await query.CountAsync(x => x.Status == VendorRegistrationStatus.Rejected, cancellationToken);

        query = request.Category switch
        {
            RequestRegistrationCategory.Process => query.Where(x => x.Status == VendorRegistrationStatus.Submitted
                || x.Status == VendorRegistrationStatus.OnReview
                || x.Status == VendorRegistrationStatus.AdminMSAI
                || x.Status == VendorRegistrationStatus.ApprovedByAdminMSAI
                || x.Status == VendorRegistrationStatus.ReviewBySeniorManagerMSAI),
            RequestRegistrationCategory.Approve => query.Where(x => x.Status == VendorRegistrationStatus.ApprovedBySeniorManagerMSAI),
            RequestRegistrationCategory.Reject => query.Where(x => x.Status == VendorRegistrationStatus.Rejected),
            _ => query
        };

        var totalCount = await query.CountAsync(cancellationToken);
        var page = request.Page ?? 1;
        var pageSize = request.PageSize ?? 10;
        var orderedQuery = Order(query, request.SortOrder, request.SortField);
        var items = await orderedQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new RequestRegistrationItem
            {
                Id = x.Id,
                VendorName = x.CompanyName,
                Status = x.Status,
                StatusDisplay = string.Empty,
                Category = x.Status == VendorRegistrationStatus.ApprovedBySeniorManagerMSAI
                    ? RequestRegistrationCategory.Approve
                    : x.Status == VendorRegistrationStatus.Rejected
                        ? RequestRegistrationCategory.Reject
                        : RequestRegistrationCategory.Process,
                SubmittedAt = x.SubmittedAt!.Value,
                LastUpdatedAt = x.Modified ?? x.Created
            })
            .ToListAsync(cancellationToken);
        items = items.Select(item => item with { StatusDisplay = DisplayStatus(item.Status) }).ToList();

        return new GetRequestRegistrationsResponse
        {
            Items = items,
            TotalCount = totalCount,
            ProcessCount = processCount,
            ApproveCount = approveCount,
            RejectCount = rejectCount
        };
    }

    private static string DisplayStatus(VendorRegistrationStatus status)
    {
        return status switch
        {
            VendorRegistrationStatus.Draft => "Draft",
            VendorRegistrationStatus.Submitted => "New",
            VendorRegistrationStatus.OnReview => "Review by Admin MSAir",
            VendorRegistrationStatus.AdminMSAI => "Request Approval Admin MSAir",
            VendorRegistrationStatus.ApprovedByAdminMSAI => "Request Approval Sr. Man MSAir",
            VendorRegistrationStatus.ReviewBySeniorManagerMSAI => "Review by Admin Sr. Man MSAir",
            VendorRegistrationStatus.ApprovedBySeniorManagerMSAI => "Approved",
            VendorRegistrationStatus.Rejected => "Reject",
            _ => status.ToString()
        };
    }

    private static IOrderedQueryable<VendorRegistration> Order(
        IQueryable<VendorRegistration> query,
        SortOrder? sortOrder,
        string? sortField)
    {
        var descending = sortOrder == SortOrder.Descending;

        return (sortField, descending) switch
        {
            (nameof(RequestRegistrationItem.VendorName), false) => query.OrderBy(x => x.CompanyName).ThenBy(x => x.Id),
            (nameof(RequestRegistrationItem.VendorName), true) => query.OrderByDescending(x => x.CompanyName).ThenBy(x => x.Id),
            (nameof(RequestRegistrationItem.Status), false) => query.OrderBy(x => x.Status).ThenBy(x => x.Id),
            (nameof(RequestRegistrationItem.Status), true) => query.OrderByDescending(x => x.Status).ThenBy(x => x.Id),
            (nameof(RequestRegistrationItem.SubmittedAt), false) => query.OrderBy(x => x.SubmittedAt).ThenBy(x => x.Id),
            (nameof(RequestRegistrationItem.SubmittedAt), true) => query.OrderByDescending(x => x.SubmittedAt).ThenBy(x => x.Id),
            (nameof(RequestRegistrationItem.LastUpdatedAt), false) => query.OrderBy(x => x.Modified ?? x.Created).ThenBy(x => x.Id),
            (nameof(RequestRegistrationItem.LastUpdatedAt), true) => query.OrderByDescending(x => x.Modified ?? x.Created).ThenBy(x => x.Id),
            _ => query.OrderByDescending(x => x.SubmittedAt).ThenBy(x => x.Id)
        };
    }
}
