using EBVL.Shared.Dto.Modules.Main.MyProjects.GetMyProject;
using Pertamina.Services.CurrentUser;

namespace EBVL.BackEnd.Logics.Modules.Main.MyProjects.GetMyProject;

[AuthorizeRequest]
public sealed record GetMyProjectQuery : GetMyProjectRequest, IRequest<GetMyProjectResponse> { }

public sealed class GetMyProjectValidator : AbstractValidatorBase<GetMyProjectQuery>
{
    public GetMyProjectValidator()
    {
        Include(new GetMyProjectRequestValidator());
    }
}

public sealed class GetMyProjectHandler(IDatabaseService databaseService,
    ICurrentUserService currentUserService)
    : IRequestHandler<GetMyProjectQuery, GetMyProjectResponse>
{
    public async Task<GetMyProjectResponse> Handle(GetMyProjectQuery request, CancellationToken cancellationToken)
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

        var audits = await databaseService.Audits
            .Where(audit => audit.EntityName == nameof(Project) && audit.EntityId == request.Id)
            .OrderBy(audit => audit.Created)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var statuses = await databaseService.Statuses
            .AsNoTracking()
            .ToDictionaryAsync(x => x.Id, cancellationToken);

        var projectStageDraftStatusId = statuses.Values
            .Single(x => x.Table == StatusesTableFor.ProjectStage && x.Code == StatusesStatusCodeFor.ProjectStageDraft)
            .Id;

        var projects = await databaseService.Projects
            .AsNoTracking()
            .Where(x => !x.IsDeleted && x.Id == request.Id)
            .Where(x => x.ProjectLenders.Any(pl => pl.LenderId == user.LenderId))
            .Where(x => x.ProjectStages.Any(ps => ps.StatusId != projectStageDraftStatusId))
            .Select(x => new
            {
                x.Id,
                x.Title,
                x.Desc,
                x.Objective,
                x.FinanceType,
                x.StatusId,
                ProjectLender = x.ProjectLenders.Where(pl => pl.LenderId == user.LenderId)
                .Select(pl => new
                {
                    pl.Id,
                    pl.StatusId,
                    pl.Note,
                    pl.FileStorageId
                })
                .Single(),
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
                        .FirstOrDefault()
                })
            })
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw ExceptionFor.EntityNotFound(ProjectsDisplayTextFor.Project, CommonDisplayTextFor.Id, request.Id);

        var item = new MyProjectItem()
        {
            Id = projects.Id,
            Title = projects.Title,
            Desc = projects.Desc,
            Objective = projects.Objective,
            FinanceType = projects.FinanceType,
            StatusId = projects.StatusId,
            StatusName = statuses[projects.StatusId].Name,
            StatusCode = statuses[projects.StatusId].Code,
            ProjectLenderId = projects.ProjectLender.Id,
            ProjectLenderStatusId = projects.ProjectLender.StatusId,
            ProjectLenderNote = projects.ProjectLender.Note,
            FileStorageId = projects.ProjectLender.FileStorageId,
            ProjectLenderStatusCode = statuses[projects.ProjectLender.StatusId].Code,
            ProjectLenderStatusName = statuses[projects.ProjectLender.StatusId].Name,
            ProjectStages = [.. projects.Stages.Select(stage => new MyProjectStageItem
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
            })],
            Audits = audits.ToAuditItems<AuditItem>()
        };

        return new GetMyProjectResponse
        {
            Item = item
        };
    }
}
