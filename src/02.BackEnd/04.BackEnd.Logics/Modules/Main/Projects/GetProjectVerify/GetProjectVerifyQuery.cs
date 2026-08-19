using EBVL.Shared.Dto.Modules.Main.Projects.GetProjectVerify;
using Pertamina.Services.CurrentUser;

namespace EBVL.BackEnd.Logics.Modules.Main.Projects.GetProjectVerify;

[AuthorizeRequest]
public sealed record GetProjectVerifyQuery : GetProjectVerifyRequest, IRequest<GetProjectVerifyResponse> { }

public sealed class GetProjectVerifyQueryValidator : AbstractValidatorBase<GetProjectVerifyQuery>
{
    public GetProjectVerifyQueryValidator()
    {
        Include(new GetProjectVerifyRequestValidator());
    }
}

public sealed class GetProjectVerifyQueryHandler(IDatabaseService databaseService,
    ICurrentUserService currentUserService)
    : IRequestHandler<GetProjectVerifyQuery, GetProjectVerifyResponse>
{
    public async Task<GetProjectVerifyResponse> Handle(GetProjectVerifyQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId
            ?? throw ExceptionFor.NotAuthenticated();

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
            .Select(x => new ProjectStageItem
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
                ProjectLenderReqs = x.ProjectLenderReqs
                .Select(pl => new ProjectLenderReqItem()
                {
                    Id = pl.Id,
                    ProjectLenderId = pl.ProjectLenderId,
                    ProjectLenderName = pl.ProjectLender.Lender.Name,
                    ProjectLenderEmails = pl.ProjectLender.Lender.Users.Select(x => x.Username).ToList(),
                    ProjectLenderStatusCode = status[pl.ProjectLender.StatusId].Code,
                    StatusId = pl.StatusId,
                    StatusCode = status[pl.StatusId].Code,
                    StatusName = status[pl.StatusId].Name,
                    ProjectReqItems = pl.ProjectStage.ProjectReqs.OrderBy(pr => pr.SortNo)
                    .Select(pr => new ProjectReqItem
                    {
                        Id = pr.Id,
                        Name = pr.Name,
                        Desc = pr.Desc,
                        SortNo = pr.SortNo,
                        IsRequired = pr.IsRequired,
                        ProjectLenderReqFiles = pl.ProjectLenderReqFiles
                        .Where(pf => pf.ProjectReqId == pr.Id)
                        .Select(pf => new ProjectLenderReqFileItem
                        {
                            Id = pf.Id,
                            FileStorageId = pf.FileStorageId,
                            FileStorageName = string.Empty
                        })
                        .ToList()
                    })
                    .ToList(),
                    ProjectLenderHistories = pl.ProjectLenderHistories.Select(ph => new ProjectLenderHistoryItem()
                    {
                        Id = ph.Id,
                        Remarks = ph.Remarks,
                        Created = ph.Created,
                        CreatedBy = ph.CreatedBy
                    }).OrderByDescending(x => x.Created).ToList(),
                }).ToList(),
                Audits = audits.ToAuditItems<AuditItem>()
            })
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw ExceptionFor.EntityNotFound(ProjectsDisplayTextFor.Project, CommonDisplayTextFor.Id, request.Id);

        #region Get File Storage Name
        var lenderReqFiles = projectStage.ProjectLenderReqs
            .SelectMany(x => x.ProjectReqItems)
            .SelectMany(x => x.ProjectLenderReqFiles)
            .ToList();

        var fileIds = lenderReqFiles
            .Select(x => x.FileStorageId)
            .Where(x => x != Guid.Empty)
            .Distinct().ToList();

        if (fileIds.Count > 0)
        {
            var fileStorageLookup = await databaseService.FileStorages
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .Where(x => fileIds.Contains(x.Id))
                .ToDictionaryAsync(x => x.Id, x => x.OriginalFileName, cancellationToken);

            foreach (var file in lenderReqFiles)
            {
                file.FileStorageName = fileStorageLookup.GetValueOrDefault(file.FileStorageId, string.Empty);
            }
        }
        #endregion

        return new GetProjectVerifyResponse
        {
            Item = projectStage
        };
    }
}
