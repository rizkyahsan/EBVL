using EBVL.Shared.Dto.Modules.MasterData.Users.UpdateUser;

namespace EBVL.BackEnd.Logics.Modules.MasterData.Users.UpdateUser;

[AuthorizeRequest]
public sealed record UpdateUserCommand : UpdateUserRequest, IRequest { }

public sealed class UpdateUserCommandValidator : AbstractValidatorBase<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        Include(new UpdateUserRequestValidator());
    }
}

public sealed class UpdateUserCommandHandler(IDatabaseService databaseService)
    : IRequestHandler<UpdateUserCommand>
{
    public async Task Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await databaseService.Users
            .Where(x => !x.IsDeleted && x.Id == request.UserId)
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw ExceptionFor.EntityNotFound(UsersDisplayTextFor.User, CommonDisplayTextFor.Id, request.UserId);

        user.DisplayName = request.Name;
        user.PhoneCode = request.CountryPhoneCode;
        user.PhoneNumber = request.PhoneNumber;

        _ = await databaseService.SaveAsync(nameof(UpdateUser), cancellationToken);
    }
}
