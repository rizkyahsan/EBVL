using EBVL.BackEnd.Logics.Common.Services.FileStorageDb;
using EBVL.Shared.Dto.Modules.Main.Projects.DeleteProjectFile;
using EBVL.Shared.Statics;
using Pertamina.Services.CurrentUser;

namespace EBVL.BackEnd.Logics.Modules.Main.Projects.DeleteProjectFile;

[AuthorizeRequest]
public sealed record DeleteProjectFileCommand : DeleteProjectFileRequest, IRequest { }

public sealed class DeleteProjectFileCommandValidator : AbstractValidatorBase<DeleteProjectFileCommand>
{
    public DeleteProjectFileCommandValidator()
    {
        Include(new DeleteProjectFileRequestValidator());
    }
}

public sealed class DeleteProjectFileCommandHandler(IDatabaseService databaseService,
    ICurrentUserService currentUserService,
    IFileStorageDbService fileStorageDbService)
    : IRequestHandler<DeleteProjectFileCommand>
{
    public async Task Handle(DeleteProjectFileCommand request, CancellationToken cancellationToken)
    {
        var username = currentUserService.Username
            ?? throw ExceptionFor.NotAuthenticated();

        var fileStorageIdsToDelete = new Guid();

        await using var transaction =
            await databaseService.BeginTransactionAsync(cancellationToken);

        try
        {
            var projectFile = await databaseService.ProjectFiles
                .Where(x => !x.IsDeleted && x.Id == request.Id)
                .SingleOrDefaultAsync(cancellationToken)
                ?? throw ExceptionFor.EntityNotFound(ProjectFilesDisplayTextFor.ProjectFile, CommonDisplayTextFor.Id, request.Id);

            projectFile.IsDeleted = true;
            fileStorageIdsToDelete = projectFile.FileStorageId;

            #region Add Logs
            var logs = new LogTransaction
            {
                ProjectId = projectFile.ProjectId,
                Action = CommonActionFor.MainProjectsDeleteFileProject,
                Role = RoleNameFor.Admin
            };
            _ = await databaseService.LogTransactions.AddAsync(logs, cancellationToken);
            #endregion

            _ = await databaseService.SaveAsync(nameof(DeleteProjectFile), cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            #region Delete all old file storage
            try
            {
                await fileStorageDbService.DeleteAsync(fileStorageIdsToDelete, cancellationToken);
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
