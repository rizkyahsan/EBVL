using EBVL.BackEnd.Logics.Common.Builder;
using EBVL.BackEnd.Logics.Common.Services.FileStorageDb;
using EBVL.BackEnd.Services.AppConfigBackEnd;
using EBVL.BackEnd.Services.EmailBlast2;
using EBVL.BackEnd.Services.EmailBlast2.Model;
using EBVL.Shared.Dto.Modules.Main.MyProjects.UpdateMyProjectStage;
using EBVL.Shared.Statics;
using EBVL.Shared.Statics.Configurations;
using Microsoft.Extensions.Options;
using Pertamina.Services.CurrentUser;

namespace EBVL.BackEnd.Logics.Modules.Main.MyProjects.UpdateMyProjectStage;

[AuthorizeRequest]
public sealed record UpdateMyProjectStageCommand : UpdateMyProjectStageRequest, IRequest { }

public sealed class UpdateMyProjectStageCommandValidator : AbstractValidatorBase<UpdateMyProjectStageCommand>
{
    public UpdateMyProjectStageCommandValidator()
    {
        Include(new UpdateMyProjectStageRequestValidator());
    }
}

public sealed class UpdateMyProjectStageCommandHandler(IDatabaseService databaseService,
    ICurrentUserService currentUserService,
    IFileStorageDbService fileStorageDbService,
    IEmailBlast2Service emailService,
    IOptions<AppConfigBackEndOptions> appConfigBackEndOptions)
    : IRequestHandler<UpdateMyProjectStageCommand>
{
    private readonly AppConfigBackEndOptions _appConfigBackEndOptions = appConfigBackEndOptions.Value;
    public async Task Handle(UpdateMyProjectStageCommand request, CancellationToken cancellationToken)
    {
        var username = currentUserService.Username
            ?? throw ExceptionFor.NotAuthenticated();

        var createdFileStorageIds = new List<Guid>();
        var fileStorageIdsToDelete = new List<Guid>();

        await using var transaction =
            await databaseService.BeginTransactionAsync(cancellationToken);

        try
        {
            #region Set Status Used
            var statuses = await databaseService.Statuses
                .AsNoTracking()
                .ToDictionaryAsync(x => $"{x.Table}-{x.Code}", x => x.Id, cancellationToken);

            var status = new
            {
                ReqSubmit = statuses[$"{StatusesTableFor.ProjectLenderReq}-{StatusesStatusCodeFor.ProjectLenderReqSubmit}"]
            };
            #endregion

            var projectLenderReq = await databaseService.ProjectLenderReqs
                .Include(x => x.Project)
                .Include(x => x.ProjectStage)
                .Include(x => x.ProjectLender).ThenInclude(x => x.Lender)
                .Where(x => !x.IsDeleted && x.Id == request.Id)
                .SingleOrDefaultAsync(cancellationToken)
                ?? throw ExceptionFor.EntityNotFound(ProjectLenderReqsDisplayTextFor.ProjectLenderReq, CommonDisplayTextFor.Id, request.Id);

            var existingReqs = await databaseService.ProjectLenderReqFiles
                .Where(x => !x.IsDeleted && x.ProjectLenderReqId == projectLenderReq.Id)
                .ToListAsync(cancellationToken);

            var requestIds = request.ProjectLenderReqFiles
                .Where(x => x.Id != Guid.Empty)
                .Select(x => x.Id).ToHashSet();

            #region Validate Required Documents On Submit
            var remainingExistingReqIds = existingReqs
                .Where(x => requestIds.Contains(x.Id))
                .Select(x => x.ProjectReqId);

            var newReqIds = request.ProjectLenderReqFiles
                .Where(x => x.Id == Guid.Empty)
                .Select(x => x.ProjectReqId).Distinct();

            var finalUploadedReqIds = remainingExistingReqIds.Union(newReqIds).ToHashSet();

            if (request.IsSubmitted)
            {
                var requiredReqs = await databaseService.ProjectReqs
                    .AsNoTracking()
                    .Where(x => x.ProjectStageId == projectLenderReq.ProjectStageId && x.IsRequired)
                    .Select(x => new { x.Id, x.Name })
                    .ToListAsync(cancellationToken);

                var missingReqs = requiredReqs
                    .Where(x => !finalUploadedReqIds.Contains(x.Id))
                    .ToList();

                if (missingReqs.Count > 0)
                {
                    throw ExceptionFor.ModelValidation($"Required documents missing: {string.Join(", ", missingReqs.Select(x => x.Name))}");
                }

                projectLenderReq.StatusId = status.ReqSubmit;
            }
            #endregion

            #region Project Lender Req Files
            foreach (var existing in existingReqs)
            {
                if (requestIds.Contains(existing.Id))
                {
                    continue;
                }

                existing.IsDeleted = true;

                if (existing.FileStorageId != Guid.Empty)
                {
                    fileStorageIdsToDelete.Add(existing.FileStorageId);
                }
            }

            var newReqs = new List<ProjectLenderReqFile>();

            foreach (var requestItem in request.ProjectLenderReqFiles.Where(x => x.Id == Guid.Empty))
            {
                if (requestItem.File is null)
                {
                    throw new InvalidOperationException("New attachment must upload a file.");
                }

                var fileStorage = await fileStorageDbService.CreateAsync(requestItem.File, cancellationToken);

                createdFileStorageIds.Add(fileStorage.Id);

                newReqs.Add(
                    new ProjectLenderReqFile
                    {
                        ProjectId = projectLenderReq.ProjectId,
                        ProjectReqId = requestItem.ProjectReqId,
                        ProjectLenderReqId = projectLenderReq.Id,
                        FileStorageId = fileStorage.Id
                    });
            }

            await databaseService.ProjectLenderReqFiles.AddRangeAsync(newReqs, cancellationToken);
            #endregion

            #region Project Lender Req History
            if (!string.IsNullOrWhiteSpace(request.Remarks))
            {
                var projectLenderHistory = new ProjectLenderHistory
                {
                    ProjectId = projectLenderReq.ProjectId,
                    ProjectLenderReqId = projectLenderReq.Id,
                    Remarks = request.Remarks
                };

                _ = await databaseService.ProjectLenderHistories.AddAsync(projectLenderHistory, cancellationToken);
            }
            #endregion

            #region Add Logs
            var logs = new LogTransaction
            {
                ProjectId = projectLenderReq.ProjectId,
                ProjectStageId = projectLenderReq.ProjectStageId,
                ProjectLenderId = projectLenderReq.ProjectLenderId,
                Action = request.IsSubmitted ? CommonActionFor.MainMyProjectsSubmitMyProjectStage : CommonActionFor.MainMyProjectsUpdateMyProjectStage,
                Role = RoleNameFor.Lender
            };

            _ = await databaseService.LogTransactions.AddAsync(logs, cancellationToken);
            #endregion

            _ = await databaseService.SaveAsync(nameof(UpdateMyProjectStage), cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            if (request.IsSubmitted)
            {
                #region send notification email
                var keys = new[]
                {
                    KeyFor.CompanyName,
                    KeyFor.DefaultFrom
                };

                var configuration = await databaseService.Configurations
                    .Where(x => !x.IsDeleted)
                    .Where(x => keys.Contains(x.Key))
                    .ToDictionaryAsync(x => x.Key, x => x.Value, cancellationToken);

                var companyName = configuration[KeyFor.CompanyName];
                var defaultFrom = configuration[KeyFor.DefaultFrom];

                var tos = configuration
                    .Where(x => x.Key == KeyFor.DefaultAdminEmail)
                    .Select(x => new EmailContact2
                    {
                        Address = x.Value,
                        Name = x.Value,
                    })
                    .ToList();

                if (tos.Count == 0)
                {
                    return;
                }

                try
                {
                    var parametersBodyEmail = new Dictionary<string, string>
                    {
                        ["AdminName"] = $"Admin {_appConfigBackEndOptions.AppNickName}",
                        ["ProjectName"] = projectLenderReq.Project.Title,
                        ["ProjectStageName"] = projectLenderReq.ProjectStage.Name,
                        ["LenderName"] = projectLenderReq.ProjectLender.Lender.Name,
                        ["Note"] = $"{request.Remarks}",
                        ["FrontendLink"] = $"<a href=\"{_appConfigBackEndOptions.FrontEndBaseUrl}\">{_appConfigBackEndOptions.AppNickName}</a> - {companyName}"
                    };

                    var internalTos = new List<EmailContact2>();
                    var externalTos = new List<EmailContact2>();

                    foreach (var to in tos)
                    {
                        if (to.Address.Contains("pertamina", StringComparison.OrdinalIgnoreCase))
                        {
                            internalTos.Add(to);
                        }
                        else
                        {
                            externalTos.Add(to);
                        }
                    }

                    if (internalTos.Count > 0)
                    {
                        var emailTemplate = await EmailInputBuilder.BuildTheTemplate(databaseService, EmailTemplatesEmailWith.EmailBlast, CommonModuleFor.MyProjects,
                            CommonActionFor.MainMyProjectsUpdateMyProjectStage, parametersBodyEmail, defaultFrom: defaultFrom, explicitTos: internalTos);

                        emailService.SendEmails(emailTemplate);
                    }

                    if (externalTos.Count > 0)
                    {
                        var emailTemplate = await EmailInputBuilder.BuildTheTemplate(databaseService, EmailTemplatesEmailWith.TwilioSendGrid, CommonModuleFor.MyProjects,
                            CommonActionFor.MainMyProjectsUpdateMyProjectStage, parametersBodyEmail, defaultFrom: defaultFrom, explicitTos: externalTos);

                        emailService.SendEmails(emailTemplate);
                    }
                }
                catch (Exception)
                {
                    // log warning only
                }
                #endregion
            }

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
