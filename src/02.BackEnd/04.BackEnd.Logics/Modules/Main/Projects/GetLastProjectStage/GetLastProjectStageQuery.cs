using EBVL.Shared.Dto.Modules.Main.Projects.GetLastProjectStage;

namespace EBVL.BackEnd.Logics.Modules.Main.Projects.GetLastProjectStage;

[AuthorizeRequest]
public sealed record GetLastProjectStageQuery : GetLastProjectStageRequest, IRequest<GetLastProjectStageResponse> { }

public sealed class GetLastProjectStageQueryValidator : AbstractValidatorBase<GetLastProjectStageQuery>
{
    public GetLastProjectStageQueryValidator()
    {
        Include(new GetLastProjectStageRequestValidator());
    }
}

public sealed class GetLastProjectStageQueryHandler(IDatabaseService databaseService)
    : IRequestHandler<GetLastProjectStageQuery, GetLastProjectStageResponse>
{
    public async Task<GetLastProjectStageResponse> Handle(GetLastProjectStageQuery request, CancellationToken cancellationToken)
    {
        var audits = await databaseService.Audits
            .Where(audit => audit.EntityName == nameof(Project) && audit.EntityId == request.Id)
            .OrderBy(audit => audit.Created)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var statuses = await databaseService.Statuses
            .AsNoTracking()
            .ToDictionaryAsync(x => $"{x.Table}-{x.Code}", x => x.Id, cancellationToken);

        var status = await databaseService.Statuses
            .AsNoTracking()
            .ToDictionaryAsync(x => x.Id, x => x, cancellationToken);

        var projectStage = await databaseService.ProjectStages
            .AsNoTracking()
            .Where(x => !x.IsDeleted && x.ProjectId == request.Id
                && x.StatusId == statuses[$"{StatusesTableFor.ProjectStage}-{StatusesStatusCodeFor.ProjectStageComplete}"])
            .OrderByDescending(x => x.Level)
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
                ProjectAttachments = x.ProjectAttachments.Select(pa => new ProjectAttachmentItem()
                {
                    Id = pa.Id,
                    Name = pa.Name,
                    Desc = pa.Desc,
                    SortNo = pa.SortNo,
                    FileStorageId = pa.FileStorageId,
                    FileStorageName = string.Empty,
                }).OrderBy(x => x.SortNo).ToList(),
                ProjectReqs = x.ProjectReqs.Select(pr => new ProjectReqItem()
                {
                    Id = pr.Id,
                    Name = pr.Name,
                    Desc = pr.Desc,
                    SortNo = pr.SortNo,
                    IsRequired = pr.IsRequired
                }).OrderBy(x => x.SortNo).ToList(),
                Audits = audits.ToAuditItems<AuditItem>()
            })
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw ExceptionFor.EntityNotFound(ProjectStagesDisplayTextFor.ProjectStage, CommonDisplayTextFor.Id, request.Id);

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

            foreach (var attachment in projectStage.ProjectAttachments)
            {
                attachment.FileStorageName = fileStorage.GetValueOrDefault(attachment.FileStorageId, string.Empty);
            }
        }
        #endregion

        return new GetLastProjectStageResponse
        {
            Item = projectStage
        };
    }
}
