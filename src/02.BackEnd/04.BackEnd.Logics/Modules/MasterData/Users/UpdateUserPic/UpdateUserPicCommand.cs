using EBVL.Shared.Dto.Modules.MasterData.Users.UpdateUserPic;

namespace EBVL.BackEnd.Logics.Modules.MasterData.Users.UpdateUserPic;

[AuthorizeRequest]
public sealed record UpdateUserPicCommand : UpdateUserPicRequest, IRequest { }

public sealed class UpdateUserPicCommandValidator : AbstractValidatorBase<UpdateUserPicCommand>
{
    public UpdateUserPicCommandValidator()
    {
        Include(new UpdateUserPicRequestValidator());
    }
}

public sealed class UpdateUserPicCommandHandler(IDatabaseService databaseService)
    : IRequestHandler<UpdateUserPicCommand>
{
    public async Task Handle(UpdateUserPicCommand request, CancellationToken cancellationToken)
    {
        var user = await databaseService.Users
            .Where(x => !x.IsDeleted && x.Id == request.UserId)
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw ExceptionFor.EntityNotFound(UsersDisplayTextFor.User, CommonDisplayTextFor.Id, request.UserId);

        user.IsPicLender = request.IsPic;

        _ = await databaseService.SaveAsync(nameof(UpdateUserPic), cancellationToken);
    }
}
