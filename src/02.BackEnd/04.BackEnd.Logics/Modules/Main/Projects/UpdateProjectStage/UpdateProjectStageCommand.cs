using EBVL.BackEnd.Logics.Common.Services.FileStorageDb;
using EBVL.Shared.Dto.Modules.Main.Projects.UpdateProjectStage;
using EBVL.Shared.Statics;
using EBVL.Shared.Statics.Common;
using Pertamina.Services.CurrentUser;

namespace EBVL.BackEnd.Logics.Modules.Main.Projects.UpdateProjectStage;

[AuthorizeRequest]
public sealed record UpdateProjectStageCommand : UpdateProjectStageRequest, IRequest { }

public sealed class UpdateProjectStageCommandValidator : AbstractValidatorBase<UpdateProjectStageCommand>
{
    public UpdateProjectStageCommandValidator()
    {
        Include(new UpdateProjectStageRequestValidator());
    }
}

public sealed class UpdateProjectStageCommandHandler(IDatabaseService databaseService,
    ICurrentUserService currentUserService,
    IFileStorageDbService fileStorageDbService)
    : IRequestHandler<UpdateProjectStageCommand>
{
    public async Task Handle(UpdateProjectStageCommand request, CancellationToken cancellationToken)
    {
        var username = currentUserService.Username
            ?? throw ExceptionFor.NotAuthenticated();

        var createdFileStorageIds = new List<Guid>();
        var fileStorageIdsToDelete = new List<Guid>();

        await using var transaction =
            await databaseService.BeginTransactionAsync(cancellationToken);

        try
        {
            var projectStage = await databaseService.ProjectStages
                .Where(x => !x.IsDeleted && x.Id == request.Id)
                .SingleOrDefaultAsync(cancellationToken)
                ?? throw ExceptionFor.EntityNotFound(ProjectStagesDisplayTextFor.ProjectStage, CommonDisplayTextFor.Id, request.Id);

            var dueDate = TimeZoneInfo.ConvertTime((DateTimeOffset)request.DueDate!, TimezoneFor.WibTimeZone);
            projectStage.Name = request.Name;
            projectStage.Desc = request.Desc;
            projectStage.DueDate = dueDate;

            #region Project Attachment
            var existingAttachments = await databaseService
                .ProjectAttachments.Where(x => !x.IsDeleted && x.ProjectStageId == projectStage.Id)
                .ToListAsync(cancellationToken);

            var attachmentMap = request.ProjectAttachments
                .Where(x => x.Id != Guid.Empty)
                .ToDictionary(x => x.Id);

            foreach (var attachment in existingAttachments)
            {
                if (!attachmentMap.TryGetValue(attachment.Id, out var requestItem))
                {
                    attachment.IsDeleted = true;

                    if (attachment.FileStorageId != Guid.Empty)
                    {
                        fileStorageIdsToDelete.Add(attachment.FileStorageId);
                    }

                    continue;
                }

                attachment.Name = requestItem.AttachmentName;
                attachment.Desc = requestItem.AttachmentDesc;
                attachment.SortNo = requestItem.AttachmentSortNo;

                // upload replacement file
                if (requestItem.FileStorageId == Guid.Empty && requestItem.File is not null)
                {
                    var oldFileStorageId = attachment.FileStorageId;

                    var fileStorage = await fileStorageDbService.CreateAsync(requestItem.File, cancellationToken);
                    createdFileStorageIds.Add(fileStorage.Id);

                    attachment.FileStorageId = fileStorage.Id;

                    if (oldFileStorageId != Guid.Empty)
                    {
                        fileStorageIdsToDelete.Add(oldFileStorageId);
                    }
                }
            }

            var newAttachments = new List<ProjectAttachment>();

            foreach (var requestItem in request.ProjectAttachments.Where(x => x.Id == Guid.Empty))
            {
                var fileStorageId = Guid.Empty;

                if (requestItem.FileStorageId != Guid.Empty)
                {
                    fileStorageId = requestItem.FileStorageId;
                }
                else if (requestItem.File is not null)
                {
                    var fileStorage = await fileStorageDbService.CreateAsync(requestItem.File, cancellationToken);
                    createdFileStorageIds.Add(fileStorage.Id);

                    fileStorageId = fileStorage.Id;
                }

                newAttachments.Add(
                    new ProjectAttachment
                    {
                        ProjectId = projectStage.ProjectId,
                        ProjectStageId = projectStage.Id,
                        Name = requestItem.AttachmentName,
                        Desc = requestItem.AttachmentDesc,
                        SortNo = requestItem.AttachmentSortNo,
                        FileStorageId = fileStorageId
                    });
            }

            await databaseService.ProjectAttachments.AddRangeAsync(newAttachments, cancellationToken);
            #endregion

            #region Project Reqs
            var existingProjectReqs = await databaseService.ProjectReqs
                .Where(x => !x.IsDeleted && x.ProjectStageId == projectStage.Id)
                .ToListAsync(cancellationToken);

            var reqMap = request.ProjectReqs
                .Where(x => x.Id != Guid.Empty)
                .ToDictionary(x => x.Id);

            foreach (var req in existingProjectReqs)
            {
                if (!reqMap.TryGetValue(req.Id, out var requestItem))
                {
                    req.IsDeleted = true;
                    continue;
                }

                req.Name = requestItem.ReqName;
                req.Desc = requestItem.ReqDesc;
                req.SortNo = requestItem.ReqSortNo;
                req.IsRequired = requestItem.IsRequired;
            }

            var newReqs = request.ProjectReqs
                .Where(x => x.Id == Guid.Empty)
                .Select(x => new ProjectReq
                {
                    ProjectId = projectStage.ProjectId,
                    ProjectStageId = projectStage.Id,
                    Name = x.ReqName,
                    Desc = x.ReqDesc,
                    SortNo = x.ReqSortNo,
                    IsRequired = x.IsRequired,
                })
                .ToList();

            await databaseService.ProjectReqs.AddRangeAsync(newReqs, cancellationToken);
            #endregion

            #region Add Logs
            var logs = new LogTransaction
            {
                ProjectId = projectStage.ProjectId,
                ProjectStageId = projectStage.Id,
                Action = CommonActionFor.MainProjectsUpdateProjectStage,
                Role = RoleNameFor.Admin
            };
            _ = await databaseService.LogTransactions.AddAsync(logs, cancellationToken);
            #endregion

            _ = await databaseService.SaveAsync(nameof(UpdateProjectStage), cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            #region Delete all old file storage
            try
            {
                foreach (var id in fileStorageIdsToDelete.Distinct())
                {
                    await fileStorageDbService.DeleteAsync(id, cancellationToken);
                }
            }
            catch (Exception)
            {
                // log warning only
            }
            #endregion
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
