using EBVL.Shared.Dto.Modules.Main.MyProjects.GetMyProjectStage;
using Pertamina.Services.CurrentUser;

namespace EBVL.BackEnd.Logics.Modules.Main.MyProjects.GetMyProjectStage;

[AuthorizeRequest]
public sealed record GetMyProjectStageQuery : GetMyProjectStageRequest, IRequest<GetMyProjectStageResponse> { }

public sealed class GetMyProjectStageQueryValidator : AbstractValidatorBase<GetMyProjectStageQuery>
{
    public GetMyProjectStageQueryValidator()
    {
        Include(new GetMyProjectStageRequestValidator());
    }
}

public sealed class GetMyProjectStageQueryHandler(IDatabaseService databaseService,
    ICurrentUserService currentUserService)
    : IRequestHandler<GetMyProjectStageQuery, GetMyProjectStageResponse>
{
    public async Task<GetMyProjectStageResponse> Handle(GetMyProjectStageQuery request, CancellationToken cancellationToken)
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
            .Where(audit => audit.EntityName == nameof(ProjectStage) && audit.EntityId == request.Id)
            .OrderBy(audit => audit.Created)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var status = await databaseService.Statuses
            .AsNoTracking()
            .ToDictionaryAsync(x => x.Id, x => x, cancellationToken);

        var projectStage = await databaseService.ProjectStages
            .AsNoTracking()
            .Where(x => !x.IsDeleted && x.Id == request.Id)
            .Select(x => new MyProjectStageItem
            {
                Id = x.Id,
                ProjectId = x.Project.Id,
                ProjectTitle = x.Project.Title,
                ProjectDesc = x.Project.Desc,
                ProjectObjective = x.Project.Objective,
                ProjectFinanceType = x.Project.FinanceType,
                ProjectStatusId = x.Project.StatusId,
                ProjectStatusCode = status[x.Project.StatusId].Code,
                ProjectStatusName = status[x.Project.StatusId].Name,
                Level = x.Level,
                Name = x.Name,
                Desc = x.Desc,
                DueDate = x.DueDate.DateTime,
                StatusId = x.StatusId,
                StatusCode = status[x.StatusId].Code,
                StatusName = status[x.StatusId].Name,
                IsPicLender = user.IsPicLender,
                IsAllowUpdate = countPicLender >= 2,
                ProjectAttachments = x.ProjectAttachments.Select(pa => new MyProjectAttachmentItem()
                {
                    Id = pa.Id,
                    Name = pa.Name,
                    Desc = pa.Desc,
                    SortNo = pa.SortNo,
                    FileStorageId = pa.FileStorageId,
                    FileStorageName = string.Empty,
                }).OrderBy(x => x.SortNo).ToList(),
                ProjectLenderReq = x.ProjectLenderReqs
                .Where(x => x.ProjectLender.LenderId == user.LenderId)
                .Select(pl => new MyProjectLenderReqItem()
                {
                    Id = pl.Id,
                    ProjectLenderId = pl.ProjectLenderId,
                    ProjectLenderName = pl.ProjectLender.Lender.Name,
                    StatusId = pl.StatusId,
                    StatusCode = status[pl.StatusId].Code,
                    StatusName = status[pl.StatusId].Name,
                    ProjectReqItems = pl.ProjectStage.ProjectReqs.OrderBy(pr => pr.SortNo)
                    .Select(pr => new MyProjectReqItem
                    {
                        Id = pr.Id,
                        Name = pr.Name,
                        Desc = pr.Desc,
                        SortNo = pr.SortNo,
                        IsRequired = pr.IsRequired,
                        ProjectLenderReqFiles = pl.ProjectLenderReqFiles
                        .Where(pf => !pf.IsDeleted)
                        .Where(pf => pf.ProjectReqId == pr.Id)
                        .Select(pf => new MyProjectLenderReqFileItem
                        {
                            Id = pf.Id,
                            FileStorageId = pf.FileStorageId,
                            FileStorageName = string.Empty
                        })
                        .ToList()
                    })
                    .ToList(),
                    ProjectLenderHistories = pl.ProjectLenderHistories.Select(ph => new MyProjectLenderHistoryItem()
                    {
                        Id = ph.Id,
                        Remarks = ph.Remarks,
                        Created = ph.Created,
                        CreatedBy = ph.CreatedBy
                    }).OrderByDescending(x => x.Created).ToList(),
                }).Single(),
                Audits = audits.ToAuditItems<AuditItem>()
            })
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw ExceptionFor.EntityNotFound(ProjectsDisplayTextFor.MyProject, CommonDisplayTextFor.Id, request.Id);

        #region Get File Storage Name
        var lenderReqFiles = projectStage.ProjectLenderReq.ProjectReqItems
            .SelectMany(x => x.ProjectLenderReqFiles)
            .ToList();

        var fileIds = projectStage.ProjectAttachments
            .Select(x => x.FileStorageId)
            .Concat(lenderReqFiles.Select(x => x.FileStorageId))
            .Distinct()
            .ToList();

        if (fileIds.Count > 0)
        {
            var fileStorage = await databaseService.FileStorages
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .Where(x => fileIds.Contains(x.Id))
                .ToDictionaryAsync(x => x.Id, x => x.OriginalFileName, cancellationToken);

            foreach (var attachment in projectStage.ProjectAttachments)
            {
                attachment.FileStorageName = fileStorage.GetValueOrDefault(attachment.FileStorageId, string.Empty);
            }

            foreach (var lenderReqFile in lenderReqFiles)
            {
                lenderReqFile.FileStorageName = fileStorage.GetValueOrDefault(lenderReqFile.FileStorageId, string.Empty);
            }
        }
        #endregion

        return new GetMyProjectStageResponse
        {
            Item = projectStage
        };
    }
}
