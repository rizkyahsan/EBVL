using EBVL.Shared.Dto.Modules.Main.MyProjects.GetMyProjects;
using Pertamina.Services.CurrentUser;

namespace EBVL.BackEnd.Logics.Modules.Main.MyProjects.GetMyProjects;

[AuthorizeRequest]
public sealed record GetMyProjectsQuery : IRequest<GetMyProjectsResponse> { }

public sealed class GetMyProjectsQueryHandler(IDatabaseService databaseService,
    ICurrentUserService currentUserService)
    : IRequestHandler<GetMyProjectsQuery, GetMyProjectsResponse>
{
    public async Task<GetMyProjectsResponse> Handle(GetMyProjectsQuery request, CancellationToken cancellationToken)
    {
        #region Check User Login
        var userId = currentUserService.UserId
            ?? throw ExceptionFor.NotAuthenticated();

        var user = await databaseService.Users
            .AsNoTracking()
            .Where(x => x.IdentityUserId == new Guid(userId))
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw ExceptionFor.EntityNotFound(UsersDisplayTextFor.User, CommonDisplayTextFor.Id, userId);

        var countPicLender = await databaseService.Users
            .AsNoTracking()
            .Where(x => !x.IsDeleted && x.LenderId == user.LenderId && x.IsPicLender)
            .CountAsync(cancellationToken);
        #endregion

        var statuses = await databaseService.Statuses
            .AsNoTracking()
            .ToDictionaryAsync(x => x.Id, cancellationToken);

        var projectStageDraftStatusId = statuses.Values
            .Single(x => x.Table == StatusesTableFor.ProjectStage && x.Code == StatusesStatusCodeFor.ProjectStageDraft)
            .Id;

        var projects = await databaseService.Projects
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .Where(x => x.ProjectLenders.Any(pl => pl.LenderId == user.LenderId))
            .Where(x => x.ProjectStages.Any(ps => ps.StatusId != projectStageDraftStatusId))
            .Select(x => new
            {
                x.Id,
                x.Title,
                x.StatusId,
                ProjectLender = x.ProjectLenders.Where(pl => pl.LenderId == user.LenderId)
                .Select(pl => new
                {
                    pl.Id,
                    pl.StatusId
                })
                .FirstOrDefault(),
                Stages = x.ProjectStages.Where(ps => ps.ProjectLenderReqs
                .Any(pr => pr.ProjectLender.LenderId == user.LenderId))
                .Select(ps => new
                {
                    ps.Id,
                    ps.Level,
                    ps.Name,
                    ps.Desc,
                    ps.DueDate,
                    ps.StatusId,
                    ProjectLenderReq = ps.ProjectLenderReqs
                        .Where(pr => pr.ProjectLender.LenderId == user.LenderId)
                        .Select(pr => new
                        {
                            pr.StatusId
                        })
                        .First()
                })
            })
            .OrderByDescending(x => x.Id)
            .ToListAsync(cancellationToken);

        var items = projects.Select(project => new MyProjectItem
        {
            Id = project.Id,
            Title = project.Title,
            StatusId = project.StatusId,
            StatusName = statuses[project.StatusId].Name,
            StatusCode = statuses[project.StatusId].Code,
            ProjectLenderId = project.ProjectLender?.Id ?? Guid.Empty,
            ProjectLenderStatusId = project.ProjectLender?.StatusId ?? Guid.Empty,
            ProjectLenderStatusCode = project.ProjectLender is null
                    ? string.Empty : statuses[project.ProjectLender.StatusId].Code,
            ProjectLenderStatusName = project.ProjectLender is null
                    ? string.Empty : statuses[project.ProjectLender.StatusId].Name,
            ProjectStages = [.. project.Stages.OrderBy(x => x.Level).Select(stage => new MyProjectStageItem
            {
                Id = stage.Id,
                Level = stage.Level,
                Name = stage.Name,
                Desc = stage.Desc,
                DueDate = stage.DueDate.DateTime,
                StatusId = stage.StatusId,
                StatusName = statuses[stage.StatusId].Name,
                StatusCode = statuses[stage.StatusId].Code,
                IsPicLender = user.IsPicLender,
                IsAllowUpdate = countPicLender >= 2,
                StatusProjectLenderReqId = stage.ProjectLenderReq?.StatusId ?? Guid.Empty,
                StatusProjectLenderReqName = stage.ProjectLenderReq is null
                        ? string.Empty : statuses[stage.ProjectLenderReq.StatusId].Name,
                StatusProjectLenderReqCode = stage.ProjectLenderReq is null
                        ? string.Empty : statuses[stage.ProjectLenderReq.StatusId].Code
            })]
        }).ToList();

        return new GetMyProjectsResponse
        {
            Items = items
        };
    }
}
