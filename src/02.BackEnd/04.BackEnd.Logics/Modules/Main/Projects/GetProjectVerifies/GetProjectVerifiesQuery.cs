using EBVL.Shared.Dto.Modules.Main.Projects.GetProjectVerifies;
using Pertamina.Services.CurrentUser;

namespace EBVL.BackEnd.Logics.Modules.Main.Projects.GetProjectVerifies;

[AuthorizeRequest]
public sealed record GetProjectVerifiesQuery : GetProjectVerifiesRequest, IRequest<GetProjectVerifiesResponse> { }

public sealed class GetProjectVerifiesQueryHandler(IDatabaseService databaseService,
    ICurrentUserService currentUserService)
    : IRequestHandler<GetProjectVerifiesQuery, GetProjectVerifiesResponse>
{
    public async Task<GetProjectVerifiesResponse> Handle(GetProjectVerifiesQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId
            ?? throw ExceptionFor.NotAuthenticated();

        #region Set Status Used        
        var statuses = await databaseService.Statuses
            .AsNoTracking()
            .ToDictionaryAsync(x => x.Id, cancellationToken);

        var statusId = new
        {
            StageOnProgress = statuses.Values
                .Single(x => x.Table == StatusesTableFor.ProjectStage && x.Code == StatusesStatusCodeFor.ProjectStageOnProgress)
                .Id,
            StageReview = statuses.Values
                .Single(x => x.Table == StatusesTableFor.ProjectStage && x.Code == StatusesStatusCodeFor.ProjectStageOnReview)
                .Id,
            ReqOnProgress = statuses.Values
                .Single(x => x.Table == StatusesTableFor.ProjectLenderReq && x.Code == StatusesStatusCodeFor.ProjectLenderReqOnProgress)
                .Id,
            ReqSubmit = statuses.Values
                .Single(x => x.Table == StatusesTableFor.ProjectLenderReq && x.Code == StatusesStatusCodeFor.ProjectLenderReqSubmit)
                .Id,
            ReqRevision = statuses.Values
                .Single(x => x.Table == StatusesTableFor.ProjectLenderReq && x.Code == StatusesStatusCodeFor.ProjectLenderReqRevision)
                .Id
        };
        #endregion

        var projects = await databaseService.Projects
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .Where(x => x.ProjectStages.Any(ps =>
                (ps.Level == 0 && ((ps.StatusId == statusId.StageOnProgress && ps.ProjectLenderReqs.Any(pr => pr.StatusId == statusId.ReqSubmit || pr.StatusId == statusId.ReqRevision))
                    || ps.StatusId == statusId.StageReview))
                || (ps.Level > 0 &&
                    ((ps.StatusId == statusId.StageReview && ps.ProjectLenderReqs.Any(pr => pr.StatusId == statusId.ReqSubmit))
                    || (ps.StatusId == statusId.StageOnProgress && ps.ProjectLenderReqs.Any(pr => pr.StatusId == statusId.ReqOnProgress))
                    )
                )
            ))
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
                    DueDate = ps.DueDate.DateTime,
                    StatusId = ps.StatusId,
                    StatusName = statuses[ps.StatusId].Name,
                    StatusCode = statuses[ps.StatusId].Code,
                }).ToList()
            })
            .ToListAsync(cancellationToken);

        return new GetProjectVerifiesResponse
        {
            Items = projects
        };
    }
}
