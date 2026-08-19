using EBVL.BackEnd.Services.LocalIdentity;
using EBVL.Shared.Dto.Modules.MasterData.Users.DeleteUser;

namespace EBVL.BackEnd.Logics.Modules.MasterData.Users.DeleteUser;

[AuthorizeRequest]
public sealed record DeleteUserCommand : DeleteUserRequest, IRequest
{
}

public sealed class DeleteUserCommandValidator : AbstractValidatorBase<DeleteUserCommand>
{
    public DeleteUserCommandValidator()
    {
        Include(new DeleteUserRequestValidator());
    }
}

public sealed class DeleteUserCommandHandler(IDatabaseService databaseService,
    ILocalIdentityService localIdentityService)
    : IRequestHandler<DeleteUserCommand>
{
    public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await databaseService.Users
            .Where(x => !x.IsDeleted && x.Id == request.UserId)
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw ExceptionFor.EntityNotFound(UsersDisplayTextFor.User, CommonDisplayTextFor.Id, request.UserId);

        user.IsDeleted = true;

        await localIdentityService.DeleteUserAsync(user.IdentityUserId);

        _ = await databaseService.SaveAsync(nameof(DeleteUser), cancellationToken);
    }
}
