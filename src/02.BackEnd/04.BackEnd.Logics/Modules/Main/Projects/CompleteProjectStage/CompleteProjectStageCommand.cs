using EBVL.BackEnd.Logics.Common.Builder;
using EBVL.BackEnd.Services.AppConfigBackEnd;
using EBVL.BackEnd.Services.EmailBlast2;
using EBVL.BackEnd.Services.EmailBlast2.Model;
using EBVL.Shared.Dto.Modules.Main.Projects.CompleteProjectStage;
using EBVL.Shared.Statics;
using EBVL.Shared.Statics.Configurations;
using Microsoft.Extensions.Options;
using Pertamina.Services.CurrentUser;

namespace EBVL.BackEnd.Logics.Modules.Main.Projects.CompleteProjectStage;

[AuthorizeRequest]
public sealed record CompleteProjectStageCommand : CompleteProjectStageRequest, IRequest { }

public sealed class CompleteProjectStageCommandValidator : AbstractValidatorBase<CompleteProjectStageCommand>
{
    public CompleteProjectStageCommandValidator()
    {
        Include(new CompleteProjectStageRequestValidator());
    }
}

public sealed class CompleteProjectStageCommandHandler(IDatabaseService databaseService,
    ICurrentUserService currentUserService,
    IEmailBlast2Service emailService,
    IOptions<AppConfigBackEndOptions> appConfigBackEndOptions)
    : IRequestHandler<CompleteProjectStageCommand>
{
    private readonly AppConfigBackEndOptions _appConfigBackEndOptions = appConfigBackEndOptions.Value;
    public async Task Handle(CompleteProjectStageCommand request, CancellationToken cancellationToken)
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
                LenderLose = statuses[$"{StatusesTableFor.ProjectLender}-{StatusesStatusCodeFor.ProjectLenderLose}"],
                StageReview = statuses[$"{StatusesTableFor.ProjectStage}-{StatusesStatusCodeFor.ProjectStageOnReview}"],
                StageComplete = statuses[$"{StatusesTableFor.ProjectStage}-{StatusesStatusCodeFor.ProjectStageComplete}"],
                ReqAccept = statuses[$"{StatusesTableFor.ProjectLenderReq}-{StatusesStatusCodeFor.ProjectLenderReqAccept}"],
                ReqReject = statuses[$"{StatusesTableFor.ProjectLenderReq}-{StatusesStatusCodeFor.ProjectLenderReqReject}"]
            };
            #endregion

            var projectStage = await databaseService.ProjectStages
                .Include(x => x.Project)
                .Include(x => x.ProjectLenderReqs).ThenInclude(x => x.ProjectLender).ThenInclude(x => x.Lender)
                .Where(x => !x.IsDeleted && x.Id == request.Id)
                .SingleOrDefaultAsync(cancellationToken)
                ?? throw ExceptionFor.EntityNotFound(ProjectStagesDisplayTextFor.ProjectStage, CommonDisplayTextFor.Id, request.Id);

            if (projectStage.StatusId != status.StageReview)
            {
                throw new InvalidOperationException(
                    "Only project stages under review can be completed.");
            }

            projectStage.StatusId = status.StageComplete;

            #region Update ProjectLenderReq
            var reqLookup = projectStage.ProjectLenderReqs
                .Where(x => !x.IsDeleted)
                .ToDictionary(x => x.Id);

            var projectLenderIds = projectStage.ProjectLenderReqs
                .Where(x => !x.IsDeleted)
                .Select(x => x.ProjectLenderId)
                .ToHashSet();

            var projectLenders = await databaseService.ProjectLenders
                .Where(x => !x.IsDeleted)
                .Where(x => projectLenderIds.Contains(x.Id))
                .ToDictionaryAsync(x => x.Id, cancellationToken);

            var updatedLenders = new List<(CompleteProjectLenderReqRequest Request, ProjectLenderReq Entity)>();
            foreach (var lender in request.ProjectLenderReqs)
            {
                if (!reqLookup.TryGetValue(lender.Id, out var projectLender))
                {
                    throw ExceptionFor.EntityNotFound(ProjectLenderReqsDisplayTextFor.ProjectLenderReq, CommonDisplayTextFor.Id, lender.Id);
                }

                projectLender.StatusId = lender.StatusCode switch
                {
                    StatusesStatusCodeFor.ProjectLenderReqAccept => status.ReqAccept,
                    StatusesStatusCodeFor.ProjectLenderReqReject => status.ReqReject,
                    _ => throw new ValidationException("Invalid status.")
                };

                if (lender.StatusCode == StatusesStatusCodeFor.ProjectLenderReqReject)
                {
                    var pLender = projectLenders[projectLender.ProjectLenderId];
                    pLender.StatusId = status.LenderLose;
                    pLender.Note = $"Rejected on Project Stage {projectStage.Name}";
                }

                updatedLenders.Add((lender, projectLender));
            }
            #endregion

            #region Add Logs
            var logs = new LogTransaction
            {
                ProjectId = projectStage.ProjectId,
                ProjectStageId = projectStage.Id,
                Action = CommonActionFor.MainProjectsCreateProject,
                Role = RoleNameFor.Admin
            };
            _ = await databaseService.LogTransactions.AddAsync(logs, cancellationToken);
            #endregion

            _ = await databaseService.SaveAsync(nameof(CompleteProjectStage), cancellationToken);
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
            var defaultAdminEmail = configuration[KeyFor.DefaultAdminEmail];

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
                ["ProjectName"] = $"{projectStage.Project.Title}",
                ["ProjectStageName"] = $"{projectStage.Name}",
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
                    CommonActionFor.MainProjectsCompleteProjectStage, parametersBodyEmailAdmin, defaultFrom: defaultFrom, explicitTos: internalTosAdmin);

                emailService.SendEmails(emailTemplate);
            }

            if (externalTosAdmin.Count > 0)
            {
                var emailTemplate = await EmailInputBuilder.BuildTheTemplate(databaseService, EmailTemplatesEmailWith.TwilioSendGrid, CommonModuleFor.Projects,
                    CommonActionFor.MainProjectsCompleteProjectStage, parametersBodyEmailAdmin, defaultFrom: defaultFrom, explicitTos: externalTosAdmin);

                emailService.SendEmails(emailTemplate);
            }
            #endregion

            #region Email to Lender
            var lenderIds = reqLookup.Values
                .Select(x => x.ProjectLender.LenderId)
                .Distinct().ToList();

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
                .ToDictionary(g => g.Key, g => g.Select(x => x.Contact).ToList());

            foreach (var (input, lender) in updatedLenders)
            {
                if (!usersByLender.TryGetValue(lender.ProjectLender.LenderId, out var tos) || tos.Count == 0)
                {
                    continue;
                }

                try
                {
                    var parametersBodyEmail = new Dictionary<string, string>
                    {
                        ["LenderName"] = $"{lender.ProjectLender.Lender.Name}",
                        ["ProjectName"] = $"{projectStage.Project.Title}",
                        ["ProjectStageName"] = $"{projectStage.Name}",
                        ["FrontendLink"] = $"<a href=\"{_appConfigBackEndOptions.FrontEndBaseUrl}\">{_appConfigBackEndOptions.AppNickName}</a> - {companyName}"
                    };

                    var action = input.StatusCode == StatusesStatusCodeFor.ProjectLenderReqAccept ? CommonActionFor.MainProjectsCompleteProjectStageAccept : CommonActionFor.MainProjectsCompleteProjectStageReject;

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
                    // log warning
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
