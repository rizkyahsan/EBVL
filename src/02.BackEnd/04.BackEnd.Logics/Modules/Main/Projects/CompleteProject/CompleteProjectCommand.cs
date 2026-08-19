using EBVL.BackEnd.Logics.Common.Builder;
using EBVL.BackEnd.Logics.Common.Services.FileStorageDb;
using EBVL.BackEnd.Services.AppConfigBackEnd;
using EBVL.BackEnd.Services.EmailBlast2;
using EBVL.BackEnd.Services.EmailBlast2.Model;
using EBVL.Shared.Dto.Modules.Main.Projects.CompleteProject;
using EBVL.Shared.Statics;
using EBVL.Shared.Statics.Configurations;
using Microsoft.Extensions.Options;
using Pertamina.Services.CurrentUser;

namespace EBVL.BackEnd.Logics.Modules.Main.Projects.CompleteProject;

[AuthorizeRequest]
public sealed record CompleteProjectCommand : CompleteProjectRequest, IRequest { }

public sealed class CompleteProjectCommandValidator : AbstractValidatorBase<CompleteProjectCommand>
{
    public CompleteProjectCommandValidator()
    {
        Include(new CompleteProjectRequestValidator());
    }
}

public sealed class CompleteProjectCommandHandler(IDatabaseService databaseService,
    ICurrentUserService currentUserService,
    IFileStorageDbService fileStorageDbService,
    IEmailBlast2Service emailService,
    IOptions<AppConfigBackEndOptions> appConfigBackEndOptions)
    : IRequestHandler<CompleteProjectCommand>
{
    private readonly AppConfigBackEndOptions _appConfigBackEndOptions = appConfigBackEndOptions.Value;
    public async Task Handle(CompleteProjectCommand request, CancellationToken cancellationToken)
    {
        var username = currentUserService.Username
            ?? throw ExceptionFor.NotAuthenticated();

        var createdFileStorageIds = new List<Guid>();

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
                Complete = statuses[$"{StatusesTableFor.Project}-{StatusesStatusCodeFor.ProjectComplete}"],
                LenderWin = statuses[$"{StatusesTableFor.ProjectLender}-{StatusesStatusCodeFor.ProjectLenderWin}"],
                LenderLose = statuses[$"{StatusesTableFor.ProjectLender}-{StatusesStatusCodeFor.ProjectLenderLose}"]
            };
            #endregion

            var project = await databaseService.Projects
                .Where(x => !x.IsDeleted && x.Id == request.Id)
                .SingleOrDefaultAsync(cancellationToken)
                ?? throw ExceptionFor.EntityNotFound(ProjectsDisplayTextFor.Project, CommonDisplayTextFor.Id, request.Id);

            project.StatusId = status.Complete;

            #region Project Lender
            var projectLenders = await databaseService.ProjectLenders
                .Include(x => x.Lender)
                .Include(x => x.Project)
                .Where(x => !x.IsDeleted && x.ProjectId == project.Id)
                .ToDictionaryAsync(x => x.Id, cancellationToken);

            var updatedLenders = new List<(CompleteProjectLenderRequest Request, ProjectLender Entity)>();
            foreach (var lenderRequest in request.ProjectLenders)
            {
                if (!projectLenders.TryGetValue(lenderRequest.Id, out var projectLender))
                {
                    throw ExceptionFor.EntityNotFound(ProjectLendersDisplayTextFor.ProjectLender, CommonDisplayTextFor.Id, lenderRequest.Id);
                }

                projectLender.Note = lenderRequest.Note!;

                Guid? fileStorageId = null;
                if (lenderRequest.File is not null)
                {
                    var fileStorage = await fileStorageDbService.CreateAsync(lenderRequest.File, cancellationToken);
                    createdFileStorageIds.Add(fileStorage.Id);

                    fileStorageId = fileStorage.Id;
                }
                else
                {
                    fileStorageId = lenderRequest.FileStorageId;
                }

                projectLender.FileStorageId = fileStorageId;
                projectLender.StatusId = lenderRequest.StatusCode switch
                {
                    StatusesStatusCodeFor.ProjectLenderWin => status.LenderWin,
                    StatusesStatusCodeFor.ProjectLenderLose => status.LenderLose,
                    _ => throw new InvalidOperationException(
                        $"Invalid ProjectLender status '{lenderRequest.StatusCode}'.")
                };

                updatedLenders.Add((lenderRequest, projectLender));
            }
            #endregion

            #region Add Logs
            var logs = updatedLenders
                .Select(x => new LogTransaction
                {
                    ProjectId = project.Id,
                    ProjectLenderId = x.Request.Id,
                    Role = RoleNameFor.Admin,
                    Action = x.Request.StatusCode switch
                    {
                        StatusesStatusCodeFor.ProjectLenderWin => CommonActionFor.MainProjectsCompleteProjectWin,
                        StatusesStatusCodeFor.ProjectLenderLose => CommonActionFor.MainProjectsCompleteProjectLose,
                        _ => throw new ValidationException("Invalid status.")
                    }
                })
                .ToList();

            await databaseService.LogTransactions.AddRangeAsync(logs, cancellationToken);
            #endregion

            _ = await databaseService.SaveAsync(nameof(CompleteProject), cancellationToken);
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
                ["ProjectName"] = $"{project.Title}",
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
                    CommonActionFor.MainProjectsCompleteProject, parametersBodyEmailAdmin, defaultFrom: defaultFrom, explicitTos: internalTosAdmin);

                emailService.SendEmails(emailTemplate);
            }

            if (externalTosAdmin.Count > 0)
            {
                var emailTemplate = await EmailInputBuilder.BuildTheTemplate(databaseService, EmailTemplatesEmailWith.TwilioSendGrid, CommonModuleFor.Projects,
                    CommonActionFor.MainProjectsCompleteProject, parametersBodyEmailAdmin, defaultFrom: defaultFrom, explicitTos: externalTosAdmin);

                emailService.SendEmails(emailTemplate);
            }
            #endregion

            var lenderIds = projectLenders.Values
                .Select(x => x.LenderId)
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

            var fileStorageIds = updatedLenders
                .Select(x => x.Entity.FileStorageId)
                .Where(x => x.HasValue)
                .Select(x => x!.Value)
                .Distinct()
                .ToHashSet();

            var fileStorages = await databaseService.FileStorages
                .Where(x => !x.IsDeleted)
                .Where(x => fileStorageIds.Contains(x.Id))
                .ToDictionaryAsync(x => x.Id, cancellationToken);

            foreach (var (input, lender) in updatedLenders)
            {
                if (!usersByLender.TryGetValue(lender.LenderId, out var tos) || tos.Count == 0)
                {
                    continue;
                }

                try
                {
                    var attachment = new List<EmailAttachment2>();

                    if (lender.FileStorageId is not null)
                    {
                        var fileStorage = fileStorages[lender.FileStorageId.Value];
                        var fileContent = await fileStorageDbService.ReadAsync(fileStorage.Id, cancellationToken);

                        attachment.Add(new EmailAttachment2()
                        {
                            FileName = fileStorage.OriginalFileName,
                            ContentType = fileStorage.FileContentType,
                            Content = fileContent
                        });
                    }

                    var parametersBodyEmail = new Dictionary<string, string>
                    {
                        ["LenderName"] = $"{lender.Lender.Name}",
                        ["ProjectName"] = $"{lender.Project.Title}",
                        ["FrontendLink"] = $"<a href=\"{_appConfigBackEndOptions.FrontEndBaseUrl}\">{_appConfigBackEndOptions.AppNickName}</a> - {companyName}"
                    };

                    var action = input.StatusCode == StatusesStatusCodeFor.ProjectLenderWin
                        ? CommonActionFor.MainProjectsCompleteProjectWin : CommonActionFor.MainProjectsCompleteProjectLose;

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
                            action, parametersBodyEmail, defaultFrom: defaultFrom, explicitTos: externalTos, attachments: attachment);

                        emailService.SendEmails(emailTemplate);
                    }
                }
                catch (Exception)
                {
                    // log warning
                }
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
