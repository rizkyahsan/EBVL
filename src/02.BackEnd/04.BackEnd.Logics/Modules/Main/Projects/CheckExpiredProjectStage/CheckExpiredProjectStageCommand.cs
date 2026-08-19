using EBVL.BackEnd.Logics.Common.Builder;
using EBVL.BackEnd.Services.AppConfigBackEnd;
using EBVL.BackEnd.Services.EmailBlast2;
using EBVL.BackEnd.Services.EmailBlast2.Model;
using EBVL.Shared.Statics;
using EBVL.Shared.Statics.Common;
using EBVL.Shared.Statics.Configurations;
using Microsoft.Extensions.Options;

namespace EBVL.BackEnd.Logics.Modules.Main.Projects.CheckExpiredProjectStage;

public sealed record CheckExpiredProjectStageCommand : IRequest { }

public sealed class CheckExpiredProjectStageCommandHandler(IDatabaseService databaseService,
    IEmailBlast2Service emailService,
    IOptions<AppConfigBackEndOptions> appConfigBackEndOptions)
    : IRequestHandler<CheckExpiredProjectStageCommand>
{
    private readonly AppConfigBackEndOptions _appConfigBackEndOptions = appConfigBackEndOptions.Value;
    public async Task Handle(CheckExpiredProjectStageCommand request, CancellationToken cancellationToken)
    {
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
                LenderLose = statuses[$"{StatusesTableFor.ProjectLender}-{StatusesStatusCodeFor.ProjectLenderLose}"],
                StageOnProgress = statuses[$"{StatusesTableFor.ProjectStage}-{StatusesStatusCodeFor.ProjectStageOnProgress}"],
                StageReview = statuses[$"{StatusesTableFor.ProjectStage}-{StatusesStatusCodeFor.ProjectStageOnReview}"],
                ReqSubmit = statuses[$"{StatusesTableFor.ProjectLenderReq}-{StatusesStatusCodeFor.ProjectLenderReqSubmit}"],
                ReqRevision = statuses[$"{StatusesTableFor.ProjectLenderReq}-{StatusesStatusCodeFor.ProjectLenderReqRevision}"],
                ReqReject = statuses[$"{StatusesTableFor.ProjectLenderReq}-{StatusesStatusCodeFor.ProjectLenderReqReject}"]
            };
            #endregion

            var now = TimeZoneInfo.ConvertTime(DateTimeOffset.Now, TimezoneFor.WibTimeZone);
            var projectStages = await databaseService.ProjectStages
                .Include(x => x.Project)
                .Include(x => x.ProjectReqs)
                .Include(x => x.ProjectLenderReqs)
                    .ThenInclude(x => x.ProjectLender)
                        .ThenInclude(x => x.Lender)
                .Include(x => x.ProjectLenderReqs)
                    .ThenInclude(x => x.ProjectLenderReqFiles)
                .Where(x => !x.IsDeleted)
                .Where(x => x.StatusId == status.StageOnProgress)
                .Where(x => x.DueDate <= now)
                .ToListAsync(cancellationToken);

            if (projectStages.Count == 0)
            {
                return;
            }

            var projectLenderIds = projectStages
                .SelectMany(x => x.ProjectLenderReqs)
                .Where(x => !x.IsDeleted)
                .Select(x => x.ProjectLenderId)
                .Distinct()
                .ToHashSet();

            var projectLenders = await databaseService.ProjectLenders
                .Where(x => !x.IsDeleted)
                .Where(x => projectLenderIds.Contains(x.Id))
                .ToDictionaryAsync(x => x.Id, cancellationToken);

            foreach (var projectStage in projectStages)
            {
                if (projectStage.StatusId != status.StageOnProgress)
                {
                    throw new InvalidOperationException("Only on progress project stages can be Reviewed.");
                }

                projectStage.StatusId = status.StageReview;

                #region Check Requirement Project Lender Req
                var projectLenderReq = projectStage.ProjectLenderReqs
                    .Where(x => !x.IsDeleted)
                    .ToList();

                var requiredReqIds = projectStage.ProjectReqs
                    .Where(x => !x.IsDeleted && x.IsRequired)
                    .Select(x => x.Id)
                    .ToHashSet();

                foreach (var lenderReq in projectLenderReq)
                {
                    var uploadedReqIds = lenderReq.ProjectLenderReqFiles
                        .Where(x => !x.IsDeleted)
                        .Select(x => x.ProjectReqId)
                        .ToHashSet();

                    var hasMissingRequiredFile = !requiredReqIds.IsSubsetOf(uploadedReqIds);

                    if (!hasMissingRequiredFile)
                    {
                        if (lenderReq.StatusId != status.ReqRevision)
                        {
                            lenderReq.StatusId = status.ReqSubmit;
                        }
                    }
                    else
                    {
                        lenderReq.StatusId = status.ReqReject;
                        var noteRejected = projectStage.Level == 0
                            ? "The required documents were not submitted before the review due date."
                            : "The required documents were not updated before the review due date.";

                        var projectLender = projectLenders[lenderReq.ProjectLenderId];
                        projectLender.StatusId = status.LenderLose;
                        projectLender.Note = noteRejected;

                        //Add Project Lender History Rejected
                        var projectLenderHistory = new ProjectLenderHistory()
                        {
                            ProjectId = lenderReq.ProjectId,
                            ProjectLenderReqId = lenderReq.Id,
                            Remarks = noteRejected
                        };

                        _ = await databaseService.ProjectLenderHistories.AddAsync(projectLenderHistory, cancellationToken);
                    }
                }
                #endregion

                #region Add Logs
                var logs = new LogTransaction
                {
                    ProjectId = projectStage.ProjectId,
                    ProjectStageId = projectStage.Id,
                    Action = CommonActionFor.MainProjectsReviewProjectStage,
                    Role = RoleNameFor.Admin
                };
                _ = await databaseService.LogTransactions.AddAsync(logs, cancellationToken);
                #endregion
            }

            _ = await databaseService.SaveAsync(nameof(CheckExpiredProjectStage), cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            #region Send notification email
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

            var lenderIds = projectStages
                .SelectMany(x => x.ProjectLenderReqs)
                .Select(x => x.ProjectLender.LenderId)
                .Distinct()
                .ToHashSet();

            var users = await databaseService.Users
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .Where(x => x.IsPicLender)
                .Where(x => lenderIds.Contains(x.LenderId))
                .Select(x => new
                {
                    x.LenderId,
                    Contact = new EmailContact2
                    {
                        Address = x.Username,
                        Name = x.Username
                    }
                })
                .ToListAsync(cancellationToken);

            var usersByLender = users
                .GroupBy(x => x.LenderId)
                .ToDictionary(x => x.Key, x => x.Select(y => y.Contact).ToList());

            foreach (var projectStage in projectStages)
            {
                foreach (var lenderReq in projectStage.ProjectLenderReqs.Where(x => !x.IsDeleted))
                {
                    if (!usersByLender.TryGetValue(lenderReq.ProjectLender.LenderId, out var tos) || tos.Count == 0)
                    {
                        continue;
                    }

                    try
                    {
                        var parametersBodyEmail = new Dictionary<string, string>
                        {
                            ["LenderName"] = $"{lenderReq.ProjectLender.Lender.Name}",
                            ["ProjectName"] = $"{projectStage.Project.Title}",
                            ["ProjectStageName"] = $"{projectStage.Name}",
                            ["FrontendLink"] = $"<a href=\"{_appConfigBackEndOptions.FrontEndBaseUrl}\">{_appConfigBackEndOptions.AppNickName}</a> - {companyName}"
                        };

                        var action = lenderReq.StatusId == status.ReqSubmit ? CommonActionFor.MainProjectsReviewProjectStage : CommonActionFor.MainProjectsCompleteProjectLose;

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
                            var emailTemplate = await EmailInputBuilder.BuildTheTemplate(databaseService, EmailTemplatesEmailWith.EmailBlast, CommonModuleFor.Projects,
                                action, parametersBodyEmail, defaultFrom: defaultFrom, explicitTos: internalTos);

                            emailService.SendEmails(emailTemplate);
                        }

                        if (externalTos.Count > 0)
                        {
                            var emailTemplate = await EmailInputBuilder.BuildTheTemplate(databaseService, EmailTemplatesEmailWith.TwilioSendGrid, CommonModuleFor.Projects,
                                action, parametersBodyEmail, defaultFrom: defaultFrom, explicitTos: externalTos);

                            emailService.SendEmails(emailTemplate);
                        }
                    }
                    catch (Exception)
                    {
                        // log warning only
                    }
                }
            }
            #endregion
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);

            throw;
        }
    }
}
