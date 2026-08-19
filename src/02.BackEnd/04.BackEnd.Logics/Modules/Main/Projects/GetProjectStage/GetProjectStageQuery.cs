using EBVL.Shared.Dto.Modules.Main.Projects.GetProjectStage;

namespace EBVL.BackEnd.Logics.Modules.Main.Projects.GetProjectStage;

[AuthorizeRequest]
public sealed record GetProjectStageQuery : GetProjectStageRequest, IRequest<GetProjectStageResponse> { }

public sealed class GetProjectStageQueryValidator : AbstractValidatorBase<GetProjectStageQuery>
{
    public GetProjectStageQueryValidator()
    {
        Include(new GetProjectStageRequestValidator());
    }
}

public sealed class GetProjectStageQueryHandler(IDatabaseService databaseService)
    : IRequestHandler<GetProjectStageQuery, GetProjectStageResponse>
{
    public async Task<GetProjectStageResponse> Handle(GetProjectStageQuery request, CancellationToken cancellationToken)
    {
        var audits = await databaseService.Audits
            .Where(audit => audit.EntityName == nameof(ProjectStage) && audit.EntityId == request.Id)
            .OrderBy(audit => audit.Created)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        #region Set Status Used        
        var statuses = await databaseService.Statuses
            .AsNoTracking()
            .ToDictionaryAsync(x => x.Id, cancellationToken);

        var statusId = new
        {
            StageDraft = statuses.Values
                .Single(x => x.Table == StatusesTableFor.ProjectStage && x.Code == StatusesStatusCodeFor.ProjectStageDraft)
                .Id,
            StageOnProgress = statuses.Values
                .Single(x => x.Table == StatusesTableFor.ProjectStage && x.Code == StatusesStatusCodeFor.ProjectStageOnProgress)
                .Id,
            StageReview = statuses.Values
                .Single(x => x.Table == StatusesTableFor.ProjectStage && x.Code == StatusesStatusCodeFor.ProjectStageOnReview)
                .Id,
            StageComplete = statuses.Values
                .Single(x => x.Table == StatusesTableFor.ProjectStage && x.Code == StatusesStatusCodeFor.ProjectStageComplete)
                .Id,
            ReqSubmit = statuses.Values
                .Single(x => x.Table == StatusesTableFor.ProjectLenderReq && x.Code == StatusesStatusCodeFor.ProjectLenderReqSubmit)
                .Id
        };
        #endregion

        var projectStage = await databaseService.ProjectStages
            .AsNoTracking()
            .Include(x => x.Project)
            .Include(x => x.ProjectAttachments)
            .Include(x => x.ProjectReqs)
            .Include(x => x.ProjectLenderReqs).ThenInclude(x => x.ProjectLender).ThenInclude(x => x.Lender).ThenInclude(x => x.Users)
            .Include(x => x.ProjectLenderReqs).ThenInclude(x => x.ProjectLenderReqFiles)
            .Include(x => x.ProjectLenderReqs).ThenInclude(x => x.ProjectLenderHistories)
            .Where(x => !x.IsDeleted && x.Id == request.Id)
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw ExceptionFor.EntityNotFound(ProjectStagesDisplayTextFor.ProjectStage, CommonDisplayTextFor.Id, request.Id);

        //Data when Project Stage Draft, On Progress
        if (projectStage.StatusId == statusId.StageDraft || projectStage.StatusId == statusId.StageOnProgress)
        {
            var data = new ProjectStageItem
            {
                Id = projectStage.Id,
                ProjectId = projectStage.Project.Id,
                ProjectTitle = projectStage.Project.Title,
                ProjectDesc = projectStage.Project.Desc,
                ProjectObjective = projectStage.Project.Objective,
                ProjectFinanceType = projectStage.Project.FinanceType,
                ProjectStatusId = projectStage.Project.StatusId,
                ProjectStatusCode = statuses[projectStage.Project.StatusId].Code,
                ProjectStatusName = statuses[projectStage.Project.StatusId].Name,
                Level = projectStage.Level,
                Name = projectStage.Name,
                Desc = projectStage.Desc,
                DueDate = projectStage.DueDate.DateTime,
                StatusId = projectStage.StatusId,
                StatusCode = statuses[projectStage.StatusId].Code,
                StatusName = statuses[projectStage.StatusId].Name,
                ProjectAttachments = [.. projectStage.ProjectAttachments.Select(pa => new ProjectAttachmentItem()
                {
                    Id = pa.Id,
                    Name = pa.Name,
                    Desc = pa.Desc,
                    SortNo = pa.SortNo,
                    FileStorageId = pa.FileStorageId,
                    FileStorageName = string.Empty,
                }).OrderBy(x => x.SortNo)],
                ProjectReqs = [.. projectStage.ProjectReqs.Select(pr => new ProjectReqItem()
                {
                    Id = pr.Id,
                    Name = pr.Name,
                    Desc = pr.Desc,
                    SortNo = pr.SortNo,
                    IsRequired = pr.IsRequired
                }).OrderBy(x => x.SortNo)],
                ProjectLenderReqs = [],
                Audits = audits.ToAuditItems<AuditItem>()
            };

            #region Get File Storage Name
            var fileIds = projectStage.ProjectAttachments
                .Select(x => x.FileStorageId)
                .Distinct()
                .ToList();

            if (fileIds.Count > 0)
            {
                var fileStorage = await databaseService.FileStorages
                    .AsNoTracking()
                    .Where(x => !x.IsDeleted)
                    .Where(x => fileIds.Contains(x.Id))
                    .ToDictionaryAsync(x => x.Id, x => x.OriginalFileName, cancellationToken);

                foreach (var attachment in data.ProjectAttachments)
                {
                    attachment.FileStorageName = fileStorage.GetValueOrDefault(attachment.FileStorageId, string.Empty);
                }
            }
            #endregion

            return new GetProjectStageResponse
            {
                Item = data
            };
        }
        else
        {
            var data = new ProjectStageItem
            {
                Id = projectStage.Id,
                ProjectId = projectStage.Project.Id,
                ProjectTitle = projectStage.Project.Title,
                ProjectDesc = projectStage.Project.Desc,
                ProjectObjective = projectStage.Project.Objective,
                ProjectFinanceType = projectStage.Project.FinanceType,
                ProjectStatusId = projectStage.Project.StatusId,
                ProjectStatusCode = statuses[projectStage.Project.StatusId].Code,
                ProjectStatusName = statuses[projectStage.Project.StatusId].Name,
                Level = projectStage.Level,
                Name = projectStage.Name,
                Desc = projectStage.Desc,
                DueDate = projectStage.DueDate.DateTime,
                StatusId = projectStage.StatusId,
                StatusCode = statuses[projectStage.StatusId].Code,
                StatusName = statuses[projectStage.StatusId].Name,
                ProjectAttachments = [.. projectStage.ProjectAttachments.Select(pa => new ProjectAttachmentItem()
                {
                    Id = pa.Id,
                    Name = pa.Name,
                    Desc = pa.Desc,
                    SortNo = pa.SortNo,
                    FileStorageId = pa.FileStorageId,
                    FileStorageName = string.Empty,
                }).OrderBy(x => x.SortNo)],
                ProjectReqs = [],
                ProjectLenderReqs = [.. projectStage.ProjectLenderReqs
                .Select(pl => new ProjectLenderReqItem()
                {
                    Id = pl.Id,
                    ProjectLenderId = pl.ProjectLenderId,
                    ProjectLenderName = pl.ProjectLender.Lender.Name,
                    ProjectLenderEmails = [.. pl.ProjectLender.Lender.Users.Select(x => x.Username)],
                    StatusId = pl.StatusId,
                    StatusCode = statuses[pl.StatusId].Code,
                    StatusName = statuses[pl.StatusId].Name,
                    ProjectReqItems = [.. pl.ProjectStage.ProjectReqs.OrderBy(pr => pr.SortNo)
                    .Select(pr => new ProjectReqItem
                    {
                        Id = pr.Id,
                        Name = pr.Name,
                        Desc = pr.Desc,
                        SortNo = pr.SortNo,
                        IsRequired = pr.IsRequired,
                        ProjectLenderReqFiles = [.. pl.ProjectLenderReqFiles
                        .Where(pf => pf.ProjectReqId == pr.Id)
                        .Select(pf => new ProjectLenderReqFileItem
                        {
                            Id = pf.Id,
                            FileStorageId = pf.FileStorageId,
                            FileStorageName = string.Empty
                        })]
                    })],
                    ProjectLenderHistories = [.. pl.ProjectLenderHistories.Select(ph => new ProjectLenderHistoryItem()
                    {
                        Id = ph.Id,
                        Remarks = ph.Remarks,
                        Created = ph.Created,
                        CreatedBy = ph.CreatedBy
                    }).OrderByDescending(x => x.Created)],
                })],
                Audits = audits.ToAuditItems<AuditItem>()
            };

            #region Get File Storage Name
            var fileIds = data.ProjectAttachments
                .Select(x => x.FileStorageId)
                .Distinct()
                .ToList();

            if (fileIds.Count > 0)
            {
                var fileStorage = await databaseService.FileStorages
                    .AsNoTracking()
                    .Where(x => !x.IsDeleted)
                    .Where(x => fileIds.Contains(x.Id))
                    .ToDictionaryAsync(x => x.Id, x => x.OriginalFileName, cancellationToken);

                foreach (var attachment in data.ProjectAttachments)
                {
                    attachment.FileStorageName = fileStorage.GetValueOrDefault(attachment.FileStorageId, string.Empty);
                }
            }

            var lenderReqFiles = data.ProjectLenderReqs
                .SelectMany(x => x.ProjectReqItems)
                .SelectMany(x => x.ProjectLenderReqFiles)
                .ToList();

            var fileIds2 = lenderReqFiles
                .Select(x => x.FileStorageId)
                .Where(x => x != Guid.Empty)
                .Distinct().ToList();

            if (fileIds2.Count > 0)
            {
                var fileStorageLookup = await databaseService.FileStorages
                    .AsNoTracking()
                    .Where(x => !x.IsDeleted)
                    .Where(x => fileIds2.Contains(x.Id))
                    .ToDictionaryAsync(x => x.Id, x => x.OriginalFileName, cancellationToken);

                foreach (var file in lenderReqFiles)
                {
                    file.FileStorageName = fileStorageLookup.GetValueOrDefault(file.FileStorageId, string.Empty);
                }
            }
            #endregion

            return new GetProjectStageResponse
            {
                Item = data
            };
        }
    }
}
