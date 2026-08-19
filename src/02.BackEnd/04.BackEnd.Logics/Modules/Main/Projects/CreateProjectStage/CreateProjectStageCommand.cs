using EBVL.BackEnd.Logics.Common.Services.FileStorageDb;
using EBVL.Shared.Dto.Modules.Main.Projects.CreateProjectStage;
using EBVL.Shared.Statics;
using EBVL.Shared.Statics.Common;
using Pertamina.Services.CurrentUser;

namespace EBVL.BackEnd.Logics.Modules.Main.Projects.CreateProjectStage;

[AuthorizeRequest]
public sealed record CreateProjectStageCommand : CreateProjectStageRequest, IRequest<CreateProjectStageResponse> { }

public sealed class CreateProjectStageCommandValidator : AbstractValidatorBase<CreateProjectStageCommand>
{
    public CreateProjectStageCommandValidator()
    {
        Include(new CreateProjectStageRequestValidator());
    }
}

public sealed class CreateProjectStageCommandHandler(IDatabaseService databaseService,
    ICurrentUserService currentUserService,
    IFileStorageDbService fileStorageDbService)
    : IRequestHandler<CreateProjectStageCommand, CreateProjectStageResponse>
{
    public async Task<CreateProjectStageResponse> Handle(CreateProjectStageCommand request, CancellationToken cancellationToken)
    {
        var username = currentUserService.Username
            ?? throw ExceptionFor.NotAuthenticated();

        var createdFileStorageIds = new List<Guid>();

        await using var transaction =
            await databaseService.BeginTransactionAsync(cancellationToken);

        try
        {
            var statuses = await databaseService.Statuses
                .AsNoTracking()
                .ToDictionaryAsync(x => $"{x.Table}-{x.Code}", x => x.Id, cancellationToken);

            var project = await databaseService.Projects
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Id == request.ProjectId)
                .SingleOrDefaultAsync(cancellationToken)
                ?? throw ExceptionFor.EntityNotFound(ProjectsDisplayTextFor.Project, CommonDisplayTextFor.Id, request.ProjectId);

            #region Project Stage
            var latestProjectStage = await databaseService.ProjectStages
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.ProjectId == request.ProjectId && x.StatusId == statuses[$"{StatusesTableFor.ProjectStage}-{StatusesStatusCodeFor.ProjectStageComplete}"])
                .OrderByDescending(x => x.Level)
                .FirstOrDefaultAsync(cancellationToken) ?? throw new InvalidOperationException("No latest complete project stage found.");

            var dueDate = TimeZoneInfo.ConvertTime((DateTimeOffset)request.DueDate!, TimezoneFor.WibTimeZone);
            var projectStage = new ProjectStage
            {
                ProjectId = project.Id,
                Level = latestProjectStage.Level + 1, //Next Stage
                Name = request.Name,
                Desc = request.Desc,
                DueDate = dueDate,
                StatusId = statuses[$"{StatusesTableFor.ProjectStage}-{StatusesStatusCodeFor.ProjectStageDraft}"]
            };

            _ = await databaseService.ProjectStages.AddAsync(projectStage, cancellationToken);
            #endregion

            #region Project Attachment
            var projectAttachments = new List<ProjectAttachment>();

            foreach (var attachment in request.ProjectAttachments)
            {
                if (attachment.File is not null)
                {
                    var fileStorage = await fileStorageDbService.CreateAsync(attachment.File, cancellationToken);
                    createdFileStorageIds.Add(fileStorage.Id);

                    projectAttachments.Add(
                        new ProjectAttachment
                        {
                            ProjectId = project.Id,
                            ProjectStageId = projectStage.Id,
                            Name = attachment.AttachmentName,
                            Desc = attachment.AttachmentDesc,
                            SortNo = attachment.AttachmentSortNo,
                            FileStorageId = fileStorage.Id
                        });
                }
                else if (attachment.FileStorageId != Guid.Empty)
                {
                    var copiedFileStorage = await fileStorageDbService.CopyAsync(attachment.FileStorageId, cancellationToken);
                    createdFileStorageIds.Add(copiedFileStorage.Id);

                    projectAttachments.Add(
                        new ProjectAttachment
                        {
                            ProjectId = project.Id,
                            ProjectStageId = projectStage.Id,
                            Name = attachment.AttachmentName,
                            Desc = attachment.AttachmentDesc,
                            SortNo = attachment.AttachmentSortNo,
                            FileStorageId = copiedFileStorage.Id
                        });
                }
            }

            await databaseService.ProjectAttachments.AddRangeAsync(projectAttachments, cancellationToken);
            #endregion

            #region Project Requirement
            var projectReqs = request.ProjectReqs
                .Select(req => new ProjectReq
                {
                    ProjectId = project.Id,
                    ProjectStageId = projectStage.Id,
                    Name = req.ReqName,
                    Desc = req.ReqDesc,
                    SortNo = req.ReqSortNo,
                    IsRequired = req.IsRequired
                });

            await databaseService.ProjectReqs.AddRangeAsync(projectReqs, cancellationToken);
            #endregion

            #region Add Logs
            var logs = new LogTransaction
            {
                ProjectId = projectStage.ProjectId,
                ProjectStageId = projectStage.Id,
                Action = CommonActionFor.MainProjectsCreateProjectStage,
                Role = RoleNameFor.Admin
            };
            _ = await databaseService.LogTransactions.AddAsync(logs, cancellationToken);
            #endregion

            _ = await databaseService.SaveAsync(nameof(CreateProjectStage), cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return new CreateProjectStageResponse
            {
                Item = new ProjectStageItem
                {
                    Id = project.Id,
                }
            };
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);

            #region Delete all created file storage
            foreach (var id in createdFileStorageIds.Distinct())
            {
                try
                {
                    await fileStorageDbService.DeleteAsync(id, cancellationToken);
                }
                catch
                {
                    // log only
                }
            }
            #endregion

            throw;
        }
    }
}
