using EBVL.Shared.Dto.Modules.Main.Projects.GetProjects;

namespace EBVL.BackEnd.Logics.Modules.Main.Projects.GetProjects;

[AuthorizeRequest]
public sealed record GetProjectsQuery : GetProjectsRequest, IRequest<GetProjectsResponse> { }

public sealed class GetProjectsQueryHandler(IDatabaseService databaseService)
    : IRequestHandler<GetProjectsQuery, GetProjectsResponse>
{
    public async Task<GetProjectsResponse> Handle(GetProjectsQuery request, CancellationToken cancellationToken)
    {
        #region Set Status Used        
        var statuses = await databaseService.Statuses
            .AsNoTracking()
            .ToDictionaryAsync(x => x.Id, cancellationToken);

        var statusId = new
        {
            Draft = statuses.Values
                .Single(x => x.Table == StatusesTableFor.Project && x.Code == StatusesStatusCodeFor.ProjectDraft)
                .Id,
            OnProgress = statuses.Values
                .Single(x => x.Table == StatusesTableFor.Project && x.Code == StatusesStatusCodeFor.ProjectOnProgress)
                .Id,
            Complete = statuses.Values
                .Single(x => x.Table == StatusesTableFor.Project && x.Code == StatusesStatusCodeFor.ProjectComplete)
                .Id,
        };
        #endregion

        var filterStatus = request.StatusCode switch
        {
            StatusesStatusCodeFor.ProjectDraft => statusId.Draft,
            StatusesStatusCodeFor.ProjectOnProgress => statusId.OnProgress,
            _ => Guid.Empty
        };

        var projects = await databaseService.Projects
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .Where(x => x.StatusId != statusId.Complete)
            .Where(x => request.LenderId == Guid.Empty
                || x.ProjectLenders.Any(y => y.LenderId == request.LenderId))
            .Where(x => filterStatus == Guid.Empty
                || x.StatusId == filterStatus)
            .OrderByDescending(x => x.Modified ?? x.Created)
            .Select(x => new ProjectItem
            {
                Id = x.Id,
                Title = x.Title,
                StatusId = x.StatusId,
                StatusName = statuses[x.StatusId].Name,
                StatusCode = statuses[x.StatusId].Code,
                ProjectStages = x.ProjectStages.OrderBy(x => x.Level)
                .Select(ps => new ProjectStageItem()
                {
                    Id = ps.Id,
                    Level = ps.Level,
                    Name = ps.Name,
                    Desc = ps.Desc,
                    DueDate = ps.DueDate.DateTime,
                    StatusId = ps.StatusId,
                    StatusName = statuses[ps.StatusId].Name,
                    StatusCode = statuses[ps.StatusId].Code,
                }).ToList()
            })
            .ToListAsync(cancellationToken);

        return new GetProjectsResponse
        {
            Items = projects
        };
    }
}
