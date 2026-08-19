using EBVL.BackEnd.Logics.Common.Services.FileStorageDb;
using EBVL.Shared.Dto.Modules.Main.Projects.UploadProjectFile;
using EBVL.Shared.Statics;
using Pertamina.Services.CurrentUser;

namespace EBVL.BackEnd.Logics.Modules.Main.Projects.UploadProjectFile;

[AuthorizeRequest]
public sealed record UploadProjectFileCommand : UploadProjectFileRequest, IRequest { }

public sealed class UploadProjectFileCommandValidator : AbstractValidatorBase<UploadProjectFileCommand>
{
    public UploadProjectFileCommandValidator()
    {
        Include(new UploadProjectFileRequestValidator());
    }
}

public sealed class UploadProjectFileCommandHandler(IDatabaseService databaseService,
    ICurrentUserService currentUserService,
    IFileStorageDbService fileStorageDbService)
    : IRequestHandler<UploadProjectFileCommand>
{
    public async Task Handle(UploadProjectFileCommand request, CancellationToken cancellationToken)
    {
        var username = currentUserService.Username
            ?? throw ExceptionFor.NotAuthenticated();

        var createdFileStorageId = new Guid();

        await using var transaction =
            await databaseService.BeginTransactionAsync(cancellationToken);

        try
        {
            var fileStorage = await fileStorageDbService.CreateAsync(request.File!, cancellationToken);
            createdFileStorageId = fileStorage.Id;

            var projectFile = new ProjectFile
            {
                ProjectId = request.Id,
                FileStorageId = fileStorage.Id
            };

            _ = await databaseService.ProjectFiles.AddAsync(projectFile, cancellationToken);

            #region Add Logs
            var logs = new LogTransaction
            {
                ProjectId = request.Id,
                Action = CommonActionFor.MainProjectsUploadProjectFile,
                Role = RoleNameFor.Admin
            };
            _ = await databaseService.LogTransactions.AddAsync(logs, cancellationToken);
            #endregion

            _ = await databaseService.SaveAsync(nameof(UploadProjectFile), cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);

            #region Delete all created file storage
            try
            {
                await fileStorageDbService.DeleteAsync(createdFileStorageId, cancellationToken);
            }
            catch
            {
                // log only
            }
            #endregion

            throw;
        }
    }
}
