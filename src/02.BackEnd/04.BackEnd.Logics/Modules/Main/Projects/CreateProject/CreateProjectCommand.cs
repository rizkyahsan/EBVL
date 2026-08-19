using EBVL.BackEnd.Logics.Common.Services.FileStorageDb;
using EBVL.Shared.Dto.Modules.Main.Projects.CreateProject;
using EBVL.Shared.Statics;
using EBVL.Shared.Statics.Common;
using Pertamina.Services.CurrentUser;

namespace EBVL.BackEnd.Logics.Modules.Main.Projects.CreateProject;

[AuthorizeRequest]
public sealed record CreateProjectCommand : CreateProjectRequest, IRequest<CreateProjectResponse> { }

public sealed class CreateProjectCommandValidator : AbstractValidatorBase<CreateProjectCommand>
{
    public CreateProjectCommandValidator()
    {
        Include(new CreateProjectRequestValidator());
    }
}

public sealed class CreateProjectCommandHandler(IDatabaseService databaseService,
    ICurrentUserService currentUserService,
    IFileStorageDbService fileStorageDbService)
    : IRequestHandler<CreateProjectCommand, CreateProjectResponse>
{
    public async Task<CreateProjectResponse> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
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

            #region Project
            var project = new Project
            {
                Title = request.Title,
                Desc = request.Desc,
                Objective = request.Objective,
                FinanceType = request.FinanceType,
                StatusId = statuses[$"{StatusesTableFor.Project}-{StatusesStatusCodeFor.ProjectDraft}"]
            };

            _ = await databaseService.Projects.AddAsync(project, cancellationToken);
            #endregion

            #region Project Stage
            var dueDate = TimeZoneInfo.ConvertTime((DateTimeOffset)request.ProjectStage.DueDate!, TimezoneFor.WibTimeZone);
            var projectStage = new ProjectStage
            {
                ProjectId = project.Id,
                Level = 0, //Preparation Stage
                Name = request.ProjectStage.StageName,
                Desc = request.ProjectStage.StageDesc,
                DueDate = dueDate,
                StatusId = statuses[$"{StatusesTableFor.ProjectStage}-{StatusesStatusCodeFor.ProjectStageDraft}"]
            };

            _ = await databaseService.ProjectStages.AddAsync(projectStage, cancellationToken);
            #endregion

            #region Project Lender
            var duplicateLenders = request.ProjectLenders.GroupBy(x => x.LenderId).Any(x => x.Count() > 1);

            if (duplicateLenders)
            {
                throw new InvalidOperationException("Duplicate lenders are not allowed.");
            }

            var projectLender = request.ProjectLenders.Select(x => new ProjectLender()
            {
                ProjectId = project.Id,
                LenderId = x.LenderId,
                Note = string.Empty,
                StatusId = statuses[$"{StatusesTableFor.ProjectLender}-{StatusesStatusCodeFor.ProjectLenderDraft}"]
            }).ToList();

            await databaseService.ProjectLenders.AddRangeAsync(projectLender, cancellationToken);
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
                ProjectId = project.Id,
                Action = CommonActionFor.MainProjectsCreateProject,
                Role = RoleNameFor.Admin
            };
            _ = await databaseService.LogTransactions.AddAsync(logs, cancellationToken);
            #endregion

            _ = await databaseService.SaveAsync(nameof(CreateProject), cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return new CreateProjectResponse
            {
                Item = new ProjectItem
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
