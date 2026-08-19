using EBVL.BackEnd.Logics.Common.Builder;
using EBVL.BackEnd.Logics.Common.Services.FileStorageDb;
using EBVL.BackEnd.Services.AppConfigBackEnd;
using EBVL.BackEnd.Services.EmailBlast2;
using EBVL.BackEnd.Services.EmailBlast2.Model;
using EBVL.Shared.Dto.Modules.Main.Projects.UpdateProject;
using EBVL.Shared.Statics;
using EBVL.Shared.Statics.Configurations;
using Microsoft.Extensions.Options;
using Pertamina.Services.CurrentUser;

namespace EBVL.BackEnd.Logics.Modules.Main.Projects.UpdateProject;

[AuthorizeRequest]
public sealed record UpdateProjectCommand : UpdateProjectRequest, IRequest { }

public sealed class UpdateProjectCommandValidator : AbstractValidatorBase<UpdateProjectCommand>
{
    public UpdateProjectCommandValidator()
    {
        Include(new UpdateProjectRequestValidator());
    }
}

public sealed class UpdateProjectCommandHandler(IDatabaseService databaseService,
    ICurrentUserService currentUserService,
    IFileStorageDbService fileStorageDbService,
    IEmailBlast2Service emailService,
    IOptions<AppConfigBackEndOptions> appConfigBackEndOptions)
    : IRequestHandler<UpdateProjectCommand>
{
    private readonly AppConfigBackEndOptions _appConfigBackEndOptions = appConfigBackEndOptions.Value;
    public async Task Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
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
                Draft = statuses[$"{StatusesTableFor.Project}-{StatusesStatusCodeFor.ProjectDraft}"],
                LenderWin = statuses[$"{StatusesTableFor.ProjectLender}-{StatusesStatusCodeFor.ProjectLenderWin}"],
                LenderLose = statuses[$"{StatusesTableFor.ProjectLender}-{StatusesStatusCodeFor.ProjectLenderLose}"]
            };
            #endregion

            var project = await databaseService.Projects
                .Where(x => !x.IsDeleted && x.Id == request.Id)
                .SingleOrDefaultAsync(cancellationToken)
                ?? throw ExceptionFor.EntityNotFound(ProjectsDisplayTextFor.Project, CommonDisplayTextFor.Id, request.Id);

            project.Title = request.Title;
            project.StatusId = request.StatusId;

            #region Project Lender
            var projectLenders = await databaseService.ProjectLenders
                .Include(x => x.Lender)
                .Include(x => x.Project)
                .Where(x => !x.IsDeleted && x.ProjectId == project.Id)
                .ToDictionaryAsync(x => x.Id, cancellationToken);

            var updatedLenders = new List<(UpdateProjectLenderRequest Request, ProjectLender Entity)>();

            if (project.StatusId == status.Draft)
            {
                var duplicateLenders = request.ProjectLenders.GroupBy(x => x.LenderId).Any(x => x.Count() > 1);

                if (duplicateLenders)
                {
                    throw new InvalidOperationException("Duplicate lenders are not allowed.");
                }

                var requestLenders = request.ProjectLenders
                    .Where(x => x.Id != Guid.Empty)
                    .ToDictionary(x => x.Id);

                foreach (var lender in projectLenders.Values)
                {
                    if (!requestLenders.TryGetValue(lender.Id, out var requestItem))
                    {
                        lender.IsDeleted = true;
                        continue;
                    }

                    lender.LenderId = requestItem.LenderId;
                    lender.StatusId = statuses[$"{StatusesTableFor.ProjectLender}-{requestItem.StatusCode}"];
                }

                var newLenders = request.ProjectLenders
                    .Where(x => x.Id == Guid.Empty)
                    .Select(x => new ProjectLender
                    {
                        ProjectId = project.Id,
                        LenderId = x.LenderId,
                        StatusId = statuses[$"{StatusesTableFor.ProjectLender}-{StatusesStatusCodeFor.ProjectLenderDraft}"],
                        Note = string.Empty
                    });

                await databaseService.ProjectLenders.AddRangeAsync(newLenders, cancellationToken);
            }
            else //Reject lender on middle Project On Progress
            {
                foreach (var lenderRequest in request.ProjectLenders
                    .Where(x => x.StatusCode == StatusesStatusCodeFor.ProjectLenderLose))
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
                    projectLender.StatusId = status.LenderLose;

                    updatedLenders.Add((lenderRequest, projectLender));
                }
            }
            #endregion

            #region Add Logs
            var logs = new LogTransaction
            {
                ProjectId = project.Id,
                Action = CommonActionFor.MainProjectsUpdateProject,
                Role = RoleNameFor.Admin
            };
            _ = await databaseService.LogTransactions.AddAsync(logs, cancellationToken);
            #endregion

            _ = await databaseService.SaveAsync(nameof(UpdateProject), cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            #region Send notification email
            if (project.StatusId != status.Draft) //Reminded Reject lender on middle Project On Progress
            {
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

                        var action = CommonActionFor.MainProjectsCompleteProjectLose;

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
