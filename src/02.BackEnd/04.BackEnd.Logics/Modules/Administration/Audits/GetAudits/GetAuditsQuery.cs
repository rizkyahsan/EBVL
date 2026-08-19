using EBVL.Shared.Dto.Modules.Administration;
using EBVL.Shared.Dto.Modules.Administration.Audits.GetAudits;
using Pertamina.Common.Dto.Enums;

namespace EBVL.BackEnd.Logics.Modules.Administration.Audits.GetAudits;

[AuthorizeRequestByPermission(Permissions.AdministrationAuditsRead)]
public sealed record GetAuditsQuery : GetAuditsRequest, IRequest<GetAuditsResponse>
{
}

public sealed class GetAuditsQueryValidator : AbstractValidatorBase<GetAuditsQuery>
{
    public GetAuditsQueryValidator()
    {
        Include(new GetAuditsRequestValidator());
    }
}

public sealed class GetAuditsQueryHandler(
    IDatabaseService databaseService,
    IDateAndTimeService dateAndTimeService)
    : IRequestHandler<GetAuditsQuery, GetAuditsResponse>
{
    public async Task<GetAuditsResponse> Handle(GetAuditsQuery request, CancellationToken cancellationToken)
    {
        var from = request.From ?? dateAndTimeService.Now.AddDays(-7);
        var to = request.To ?? dateAndTimeService.Now;

        var query = databaseService.Audits
            .AsNoTracking()
            .Where(audit => audit.Created >= from && audit.Created <= to);

        var filteredQuery = Filter(query, request.SearchText);
        var orderedQuery = Order(filteredQuery, request.SortOrder, request.SortField);

        var page = request.Page is null ? 1 : request.Page.Value;
        var take = request.PageSize is null ? 10 : request.PageSize.Value;
        var skip = (page - 1) * take;

        var totalCount = await orderedQuery.CountAsync(cancellationToken);
        var items = await orderedQuery
            .Skip(skip)
            .Take(take)
            .Select(audit => new AuditItem
            {
                Id = audit.Id,
                Created = audit.Created,
                CreatedBy = audit.CreatedBy,
                ActionType = audit.ActionType,
                ActionName = audit.GetActionName(),
                EntityId = audit.EntityId,
                EntityName = audit.EntityName
            })
            .ToListAsync(cancellationToken);

        return new GetAuditsResponse
        {
            TotalCount = totalCount,
            Items = items
        };
    }

    private static IQueryable<Audit> Filter(IQueryable<Audit> query, string? keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
        {
            return query;
        }

        return query.Where(audit => audit.CreatedBy.Contains(keyword)
            || audit.ActionName.Contains(keyword)
            || audit.EntityName.Contains(keyword)
            || audit.EntityId.ToString().Contains(keyword));
    }

    private static IOrderedQueryable<Audit> Order(IQueryable<Audit> query, SortOrder? sortOrder, string? sortField)
    {
        var orderedQuery = query.OrderByDescending(x => x.Created);

        if (string.IsNullOrWhiteSpace(sortField))
        {
            return orderedQuery;
        }

        return sortOrder switch
        {
            SortOrder.Ascending => OrderAscending(orderedQuery, sortField),
            SortOrder.Descending => OrderDescending(orderedQuery, sortField),
            _ => orderedQuery
        };
    }

    private static IOrderedQueryable<Audit> OrderAscending(IOrderedQueryable<Audit> orderedQuery, string sortField)
    {
        return sortField switch
        {
            nameof(Audit.Created) => orderedQuery.OrderBy(audit => audit.Created),
            nameof(Audit.CreatedBy) => orderedQuery.OrderBy(audit => audit.CreatedBy),
            nameof(Audit.ActionName) => orderedQuery.OrderBy(audit => audit.ActionName),
            nameof(Audit.EntityName) => orderedQuery.OrderBy(audit => audit.EntityName),
            nameof(Audit.EntityId) => orderedQuery.OrderBy(audit => audit.EntityId),
            _ => orderedQuery
        };
    }

    private static IOrderedQueryable<Audit> OrderDescending(IOrderedQueryable<Audit> orderedQuery, string sortField)
    {
        return sortField switch
        {
            nameof(Audit.Created) => orderedQuery.OrderByDescending(audit => audit.Created),
            nameof(Audit.CreatedBy) => orderedQuery.OrderByDescending(audit => audit.CreatedBy),
            nameof(Audit.ActionName) => orderedQuery.OrderByDescending(audit => audit.ActionName),
            nameof(Audit.EntityName) => orderedQuery.OrderByDescending(audit => audit.EntityName),
            nameof(Audit.EntityId) => orderedQuery.OrderByDescending(audit => audit.EntityId),
            _ => orderedQuery
        };
    }
}
