using EBVL.Shared.Dto.Modules.Main.Projects.GetProject;

namespace EBVL.BackEnd.Logics.Modules.Main.Projects.GetProject;

[AuthorizeRequest]
public sealed record GetProjectQuery : GetProjectRequest, IRequest<GetProjectResponse> { }

public sealed class GetProjectQueryValidator : AbstractValidatorBase<GetProjectQuery>
{
    public GetProjectQueryValidator()
    {
        Include(new GetProjectRequestValidator());
    }
}

public sealed class GetProjectQueryHandler(IDatabaseService databaseService)
    : IRequestHandler<GetProjectQuery, GetProjectResponse>
{
    public async Task<GetProjectResponse> Handle(GetProjectQuery request, CancellationToken cancellationToken)
    {
        var audits = await databaseService.Audits
            .Where(audit => audit.EntityName == nameof(Project) && audit.EntityId == request.Id)
            .OrderBy(audit => audit.Created)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var status = await databaseService.Statuses
            .AsNoTracking()
            .ToDictionaryAsync(x => x.Id, x => x, cancellationToken);

        var fileStorage = await databaseService.FileStorages
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .ToDictionaryAsync(x => x.Id, x => x.OriginalFileName, cancellationToken);

        var project = await databaseService.Projects
            .Where(x => !x.IsDeleted && x.Id == request.Id)
            .Select(x => new ProjectItem
            {
                Id = x.Id,
                Title = x.Title,
                Desc = x.Desc,
                Objective = x.Objective,
                FinanceType = x.FinanceType,
                StatusId = x.StatusId,
                StatusName = status[x.StatusId].Name,
                StatusCode = status[x.StatusId].Code,
                ProjectStages = x.ProjectStages.Select(ps => new ProjectStageItem()
                {
                    Id = ps.Id,
                    Level = ps.Level,
                    Name = ps.Name,
                    Desc = ps.Desc,
                    DueDate = ps.DueDate.DateTime,
                    StatusId = ps.StatusId,
                    StatusName = status[ps.StatusId].Name,
                    StatusCode = status[ps.StatusId].Code,
                }).OrderBy(x => x.Level).ToList(),
                ProjectLenders = x.ProjectLenders.Select(pl => new ProjectLenderItem()
                {
                    Id = pl.Id,
                    LenderId = pl.LenderId,
                    LenderName = pl.Lender.Name,
                    Note = pl.Note,
                    FileStorageId = pl.FileStorageId,
                    FileStorageName = string.Empty,
                    StatusId = pl.StatusId,
                    StatusName = status[pl.StatusId].Name,
                    StatusCode = status[pl.StatusId].Code,
                }).ToList(),
                Audits = audits.ToAuditItems<AuditItem>()
            })
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw ExceptionFor.EntityNotFound(ProjectsDisplayTextFor.Project, CommonDisplayTextFor.Id, request.Id);

        foreach (var file in project.ProjectLenders)
        {
            file.FileStorageName = file.FileStorageId.HasValue
                ? fileStorage.GetValueOrDefault(file.FileStorageId.Value, string.Empty)
                : string.Empty;
        }

        return new GetProjectResponse
        {
            Item = project
        };
    }
}
