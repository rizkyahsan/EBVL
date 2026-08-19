using EBVL.BackEnd.Logics.Common.Builder;
using EBVL.BackEnd.Services.AppConfigBackEnd;
using EBVL.BackEnd.Services.EmailBlast2;
using EBVL.BackEnd.Services.EmailBlast2.Model;
using EBVL.Shared.Dto.Modules.Main.Projects.RevisionProjectLenderReq;
using EBVL.Shared.Statics;
using EBVL.Shared.Statics.Configurations;
using Microsoft.Extensions.Options;
using Pertamina.Services.CurrentUser;

namespace EBVL.BackEnd.Logics.Modules.Main.Projects.RevisionProjectLenderReq;

[AuthorizeRequest]
public sealed record RevisionProjectLenderReqCommand : RevisionProjectLenderReqRequest, IRequest { }

public sealed class RevisionProjectLenderReqCommandValidator : AbstractValidatorBase<RevisionProjectLenderReqCommand>
{
    public RevisionProjectLenderReqCommandValidator()
    {
        Include(new RevisionProjectLenderReqRequestValidator());
    }
}

public sealed class RevisionProjectLenderReqCommandHandler(IDatabaseService databaseService,
    ICurrentUserService currentUserService,
    IEmailBlast2Service emailService,
    IOptions<AppConfigBackEndOptions> appConfigBackEndOptions)
    : IRequestHandler<RevisionProjectLenderReqCommand>
{
    private readonly AppConfigBackEndOptions _appConfigBackEndOptions = appConfigBackEndOptions.Value;
    public async Task Handle(RevisionProjectLenderReqCommand request, CancellationToken cancellationToken)
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
                ReqSubmit = statuses[$"{StatusesTableFor.ProjectLenderReq}-{StatusesStatusCodeFor.ProjectLenderReqSubmit}"],
                ReqRevision = statuses[$"{StatusesTableFor.ProjectLenderReq}-{StatusesStatusCodeFor.ProjectLenderReqRevision}"]
            };
            #endregion

            var projectLenderReq = await databaseService.ProjectLenderReqs
                .Include(x => x.Project)
                .Include(x => x.ProjectStage)
                .Include(x => x.ProjectLender).ThenInclude(x => x.Lender)
                .Where(x => !x.IsDeleted && x.Id == request.Id)
                .SingleOrDefaultAsync(cancellationToken)
                ?? throw ExceptionFor.EntityNotFound(ProjectLenderReqsDisplayTextFor.ProjectLenderReq, CommonDisplayTextFor.Id, request.Id);

            if (projectLenderReq.StatusId != status.ReqSubmit)
            {
                throw new InvalidOperationException("Only submitted project lender requests can be revised.");
            }

            projectLenderReq.StatusId = status.ReqRevision;

            _ = await databaseService.ProjectLenderHistories.AddAsync(
                new ProjectLenderHistory
                {
                    ProjectId = projectLenderReq.ProjectId,
                    ProjectLenderReqId = projectLenderReq.Id,
                    Remarks = request.Remarks
                }, cancellationToken);

            #region Add Logs
            var logs = new LogTransaction
            {
                ProjectId = projectLenderReq.ProjectId,
                ProjectStageId = projectLenderReq.ProjectStageId,
                ProjectLenderId = projectLenderReq.ProjectLenderId,
                Action = CommonActionFor.MainProjectsRevisionProjectLenderReq,
                Role = RoleNameFor.Lender
            };
            _ = await databaseService.LogTransactions.AddAsync(logs, cancellationToken);
            #endregion

            _ = await databaseService.SaveAsync(nameof(RevisionProjectLenderReq), cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            #region send notification email
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

            var tos = await databaseService.Users
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.IsPicLender)
                .Where(x => x.LenderId == projectLenderReq.ProjectLender.LenderId)
                .Select(x => new EmailContact2
                {
                    Address = x.Username,
                    Name = x.Username
                })
                .ToListAsync(cancellationToken);

            if (tos.Count == 0)
            {
                return;
            }

            try
            {
                var parametersBodyEmail = new Dictionary<string, string>
                {
                    ["LenderName"] = $"{projectLenderReq.ProjectLender.Lender.Name}",
                    ["ProjectName"] = $"{projectLenderReq.Project.Title}",
                    ["ProjectStageName"] = $"{projectLenderReq.ProjectStage.Name}",
                    ["Note"] = $"{request.Remarks}",
                    ["DueDate"] = $"{projectLenderReq.ProjectStage.DueDate}",
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
                    var emailTemplate = await EmailInputBuilder.BuildTheTemplate(databaseService, EmailTemplatesEmailWith.EmailBlast, CommonModuleFor.Projects,
                        CommonActionFor.MainProjectsRevisionProjectLenderReq, parametersBodyEmail, defaultFrom: defaultFrom, explicitTos: internalTos);

                    emailService.SendEmails(emailTemplate);
                }

                if (externalTos.Count > 0)
                {
                    var emailTemplate = await EmailInputBuilder.BuildTheTemplate(databaseService, EmailTemplatesEmailWith.TwilioSendGrid, CommonModuleFor.Projects,
                        CommonActionFor.MainProjectsRevisionProjectLenderReq, parametersBodyEmail, defaultFrom: defaultFrom, explicitTos: externalTos);

                    emailService.SendEmails(emailTemplate);
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
            throw;
        }
    }
}
