using EBVL.BackEnd.Logics.Common.Builder;
using EBVL.BackEnd.Services.AppConfigBackEnd;
using EBVL.BackEnd.Services.EmailBlast2;
using EBVL.BackEnd.Services.EmailBlast2.Model;
using EBVL.Shared.Dto.Modules.Main.Projects.PublishProjectStage;
using EBVL.Shared.Statics;
using EBVL.Shared.Statics.Configurations;
using Microsoft.Extensions.Options;
using Pertamina.Services.CurrentUser;

namespace EBVL.BackEnd.Logics.Modules.Main.Projects.PublishProjectStage;

[AuthorizeRequest]
public sealed record PublishProjectStageCommand : PublishProjectStageRequest, IRequest { }

public sealed class PublishProjectStageCommandValidator : AbstractValidatorBase<PublishProjectStageCommand>
{
    public PublishProjectStageCommandValidator()
    {
        Include(new PublishProjectStageRequestValidator());
    }
}

public sealed class PublishProjectStageCommandHandler(IDatabaseService databaseService,
    ICurrentUserService currentUserService,
    IEmailBlast2Service emailService,
    IOptions<AppConfigBackEndOptions> appConfigBackEndOptions)
    : IRequestHandler<PublishProjectStageCommand>
{
    private readonly AppConfigBackEndOptions _appConfigBackEndOptions = appConfigBackEndOptions.Value;
    public async Task Handle(PublishProjectStageCommand request, CancellationToken cancellationToken)
    {
        var username = currentUserService.Username
            ?? throw ExceptionFor.NotAuthenticated();

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
                ProjectDraft = statuses[$"{StatusesTableFor.Project}-{StatusesStatusCodeFor.ProjectDraft}"],
                ProjectOnProgress = statuses[$"{StatusesTableFor.Project}-{StatusesStatusCodeFor.ProjectOnProgress}"],
                LenderDraft = statuses[$"{StatusesTableFor.ProjectLender}-{StatusesStatusCodeFor.ProjectLenderDraft}"],
                LenderOnProgress = statuses[$"{StatusesTableFor.ProjectLender}-{StatusesStatusCodeFor.ProjectLenderOnProgress}"],
                StageDraft = statuses[$"{StatusesTableFor.ProjectStage}-{StatusesStatusCodeFor.ProjectStageDraft}"],
                StageOnProgress = statuses[$"{StatusesTableFor.ProjectStage}-{StatusesStatusCodeFor.ProjectStageOnProgress}"],
                ReqOnProgress = statuses[$"{StatusesTableFor.ProjectLenderReq}-{StatusesStatusCodeFor.ProjectLenderReqOnProgress}"]
            };
            #endregion

            #region Project Stage
            var projectStage = await databaseService.ProjectStages
                .Include(x => x.Project)
                .Where(x => !x.IsDeleted && x.Id == request.Id)
                .SingleOrDefaultAsync(cancellationToken)
                ?? throw ExceptionFor.EntityNotFound(ProjectStagesDisplayTextFor.ProjectStage, CommonDisplayTextFor.Id, request.Id);

            if (projectStage.StatusId != status.StageDraft)
            {
                throw new InvalidOperationException("Only draft project stages can be published.");
            }

            projectStage.StatusId = status.StageOnProgress;
            #endregion

            #region Project
            var project = projectStage.Project
                ?? throw ExceptionFor.EntityNotFound(ProjectStagesDisplayTextFor.ProjectStage, CommonDisplayTextFor.Id, request.Id);

            //Update Status Project from Draft to OnProgress
            if (project.StatusId == status.ProjectDraft)
            {
                project.StatusId = status.ProjectOnProgress;
            }
            #endregion

            #region Project Lender

            var existingReqs = await databaseService.ProjectLenderReqs
                .AnyAsync(x => !x.IsDeleted && x.ProjectStageId == projectStage.Id, cancellationToken);

            if (existingReqs)
            {
                throw new InvalidOperationException("Project Lender Reqs has already been published.");
            }

            var projectLender = await databaseService.ProjectLenders
                .Include(x => x.Lender)
                .Where(x => !x.IsDeleted && x.ProjectId == projectStage.ProjectId)
                .ToListAsync(cancellationToken);

            foreach (var lender in projectLender.Where(x => x.StatusId == status.LenderDraft))
            {
                lender.StatusId = status.LenderOnProgress;
            }
            #endregion

            #region Project Lender Req
            var activeLenders = projectLender
                .Where(x => x.StatusId == status.LenderOnProgress)
                .ToList();

            if (activeLenders.Count == 0)
            {
                throw new InvalidOperationException("Project must have at least one active lender before publishing.");
            }

            var projectLenderReqs = activeLenders
                .Select(x => new ProjectLenderReq
                {
                    ProjectId = projectStage.ProjectId,
                    ProjectLenderId = x.Id,
                    ProjectStageId = projectStage.Id,
                    StatusId = status.ReqOnProgress
                }).ToList();

            await databaseService.ProjectLenderReqs.AddRangeAsync(projectLenderReqs, cancellationToken);
            #endregion

            #region Add Logs
            var logs = new LogTransaction
            {
                ProjectId = project.Id,
                ProjectStageId = projectStage.Id,
                Action = CommonActionFor.MainProjectsPublishProjectStage,
                Role = RoleNameFor.Admin
            };
            _ = await databaseService.LogTransactions.AddAsync(logs, cancellationToken);
            #endregion

            _ = await databaseService.SaveAsync(nameof(PublishProjectStage), cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            #region Send notification email
            var keys = new[]
            {
                KeyFor.CompanyName,
                KeyFor.DefaultFrom,
                KeyFor.DefaultAdminEmail
            };

            var configuration = await databaseService.Configurations
                .Where(x => !x.IsDeleted)
                .Where(x => keys.Contains(x.Key))
                .ToDictionaryAsync(x => x.Key, x => x.Value, cancellationToken);

            var companyName = configuration[KeyFor.CompanyName];
            var defaultFrom = configuration[KeyFor.DefaultFrom];
            var defaultAdminEmail = configuration[KeyFor.DefaultAdminEmail];

            var listReqEmail = string.Empty;
            var projectReqs = await databaseService.ProjectReqs
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.ProjectId == projectStage.ProjectId && x.ProjectStageId == projectStage.Id)
                .ToListAsync();

            foreach (var projectReq in projectReqs.OrderBy(x => x.SortNo))
            {
                listReqEmail += $"<p>•\t{projectReq.Name}</p>";
            }

            #region Email to Admin
            var tosAdmin = new List<EmailContact2>()
            {
               new()
               {
                   Name = $"Admin {_appConfigBackEndOptions.AppNickName}",
                   Address = defaultAdminEmail
               }
            };

            var parametersBodyEmailAdmin = new Dictionary<string, string>
            {
                ["AdminName"] = $"Admin {_appConfigBackEndOptions.AppNickName}",
                ["ProjectName"] = $"{project.Title}",
                ["ProjectStageName"] = $"{projectStage.Name}",
                ["DueDate"] = $"{projectStage.DueDate}",
                ["ListReqProject"] = $"{listReqEmail}",
                ["FrontendLink"] = $"<a href=\"{_appConfigBackEndOptions.FrontEndBaseUrl}\">{_appConfigBackEndOptions.AppNickName}</a> - {companyName}"
            };

            var internalTosAdmin = new List<EmailContact2>();
            var externalTosAdmin = new List<EmailContact2>();

            foreach (var to in tosAdmin)
            {
                if (to.Address.Contains("pertamina", StringComparison.OrdinalIgnoreCase))
                {
                    internalTosAdmin.Add(to);
                }
                else
                {
                    externalTosAdmin.Add(to);
                }
            }

            if (internalTosAdmin.Count > 0)
            {
                var emailTemplate = await EmailInputBuilder.BuildTheTemplate(databaseService, EmailTemplatesEmailWith.EmailBlast, CommonModuleFor.Projects,
                    CommonActionFor.MainProjectsPublishProjectStageAdmin, parametersBodyEmailAdmin, defaultFrom: defaultFrom, explicitTos: internalTosAdmin);

                emailService.SendEmails(emailTemplate);
            }

            if (externalTosAdmin.Count > 0)
            {
                var emailTemplate = await EmailInputBuilder.BuildTheTemplate(databaseService, EmailTemplatesEmailWith.TwilioSendGrid, CommonModuleFor.Projects,
                    CommonActionFor.MainProjectsPublishProjectStageAdmin, parametersBodyEmailAdmin, defaultFrom: defaultFrom, explicitTos: externalTosAdmin);

                emailService.SendEmails(emailTemplate);
            }
            #endregion

            #region Email to Lender
            var lenderIds = activeLenders.Select(x => x.LenderId).Distinct().ToList();

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

            var parametersBodyEmailNewProject = new Dictionary<string, string>
            {
                ["ProjectName"] = $"{project.Title}",
                ["Objectives"] = $"{project.Objective}",
                ["FinancingType"] = $"{project.FinanceType}",
                ["EmailGroup"] = $"{defaultAdminEmail}",
                ["FrontendLink"] = $"<a href=\"{_appConfigBackEndOptions.FrontEndBaseUrl}\">{_appConfigBackEndOptions.AppNickName}</a> - {companyName}"
            };

            foreach (var lender in activeLenders)
            {
                var tos = users
                    .Where(x => x.LenderId == lender.LenderId)
                    .Select(x => x.Contact)
                    .ToList();

                if (tos.Count == 0)
                {
                    continue;
                }

                try
                {
                    var parametersBodyEmail = new Dictionary<string, string>
                    {
                        ["LenderName"] = $"{lender.Lender.Name}",
                        ["ProjectName"] = $"{project.Title}",
                        ["ProjectStageName"] = $"{projectStage.Name}",
                        ["DueDate"] = $"{projectStage.DueDate}",
                        ["ListReqProject"] = $"{listReqEmail}",
                        ["EmailGroup"] = $"{defaultAdminEmail}",
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
                        if (projectStage.Level == 0)
                        {
                            var emailTemplateNewProject = await EmailInputBuilder.BuildTheTemplate(databaseService, EmailTemplatesEmailWith.EmailBlast, CommonModuleFor.Projects,
                                CommonActionFor.MainProjectsCreateProject, parametersBodyEmailNewProject, defaultFrom: defaultFrom, explicitTos: internalTos);

                            emailService.SendEmails(emailTemplateNewProject);
                        }

                        var emailTemplate = await EmailInputBuilder.BuildTheTemplate(databaseService, EmailTemplatesEmailWith.EmailBlast, CommonModuleFor.Projects,
                            CommonActionFor.MainProjectsPublishProjectStage, parametersBodyEmail, defaultFrom: defaultFrom, explicitTos: internalTos);

                        emailService.SendEmails(emailTemplate);
                    }

                    if (externalTos.Count > 0)
                    {
                        if (projectStage.Level == 0)
                        {
                            var emailTemplateNewProject = await EmailInputBuilder.BuildTheTemplate(databaseService, EmailTemplatesEmailWith.TwilioSendGrid, CommonModuleFor.Projects,
                                CommonActionFor.MainProjectsCreateProject, parametersBodyEmailNewProject, defaultFrom: defaultFrom, explicitTos: externalTos);

                            emailService.SendEmails(emailTemplateNewProject);
                        }

                        var emailTemplate = await EmailInputBuilder.BuildTheTemplate(databaseService, EmailTemplatesEmailWith.TwilioSendGrid, CommonModuleFor.Projects,
                            CommonActionFor.MainProjectsPublishProjectStage, parametersBodyEmail, defaultFrom: defaultFrom, explicitTos: externalTos);

                        emailService.SendEmails(emailTemplate);
                    }
                }
                catch (Exception)
                {
                    // log warning only
                }
            }
            #endregion
            #endregion
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);

            throw;
        }
    }
}
