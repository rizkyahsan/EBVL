using EBVL.Shared.Dto.Modules.Main.Projects.GetProjectCompletes;
using Pertamina.Services.CurrentUser;

namespace EBVL.BackEnd.Logics.Modules.Main.Projects.GetProjectCompletes;

[AuthorizeRequest]
public sealed record GetProjectCompletesQuery : GetProjectCompletesRequest, IRequest<GetProjectCompletesResponse> { }

public sealed class GetProjectCompletesQueryHandler(IDatabaseService databaseService,
    ICurrentUserService currentUserService)
    : IRequestHandler<GetProjectCompletesQuery, GetProjectCompletesResponse>
{
    public async Task<GetProjectCompletesResponse> Handle(GetProjectCompletesQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId
            ?? throw ExceptionFor.NotAuthenticated();

        #region Set Status Used        
        var statuses = await databaseService.Statuses
            .AsNoTracking()
            .ToDictionaryAsync(x => x.Id, cancellationToken);

        var statusId = new
        {
            Complete = statuses.Values
                .Single(x => x.Table == StatusesTableFor.Project && x.Code == StatusesStatusCodeFor.ProjectComplete)
                .Id,
        };
        #endregion

        var projects = await databaseService.Projects
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .Where(x => x.StatusId == statusId.Complete)
            .Where(x => request.LenderId == Guid.Empty
                || x.ProjectLenders.Any(y => y.LenderId == request.LenderId))
            .OrderByDescending(x => x.Modified ?? x.Created)
            .Select(x => new ProjectItem
            {
                Id = x.Id,
                Title = x.Title,
                StatusId = x.StatusId,
                StatusName = statuses[x.StatusId].Name,
                StatusCode = statuses[x.StatusId].Code,
                ProjectStages = x.ProjectStages.OrderBy(x => x.Level).Select(ps => new ProjectStageItem()
                {
                    Id = ps.Id,
                    Level = ps.Level,
                    Name = ps.Name,
                    Desc = ps.Desc,
                    DueDate = ps.DueDate,
                    StatusId = ps.StatusId,
                    StatusName = statuses[ps.StatusId].Name,
                    StatusCode = statuses[ps.StatusId].Code,
                }).ToList(),
                ProjectFiles = x.ProjectFiles.Select(pf => new ProjectFileItem()
                {
                    Id = pf.Id,
                    FileStorageId = pf.FileStorageId,
                    FileStorageName = string.Empty,
                }).ToList()
            })
            .ToListAsync(cancellationToken);

        #region Get File Storage Name
        var fileIds = projects
            .SelectMany(x => x.ProjectFiles)
            .Select(x => x.FileStorageId)
            .Distinct()
            .ToList();

        var fileStorageLookup = await databaseService.FileStorages
            .AsNoTracking()
            .Where(x => fileIds.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id, x => x.OriginalFileName, cancellationToken);

        foreach (var project in projects)
        {
            foreach (var file in project.ProjectFiles)
            {
                file.FileStorageName =
                    fileStorageLookup.GetValueOrDefault(file.FileStorageId, "");
            }
        }
        #endregion

        return new GetProjectCompletesResponse
        {
            Items = projects
        };
    }
}
