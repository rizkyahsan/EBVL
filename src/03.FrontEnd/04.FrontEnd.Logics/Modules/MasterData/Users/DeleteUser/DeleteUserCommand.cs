using EBVL.Shared.Dto.Modules.MasterData.Users.DeleteUser;

namespace EBVL.FrontEnd.Logics.Modules.MasterData.Users.DeleteUser;

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

public sealed class DeleteUserCommandHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<DeleteUserCommand>
{
    public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(DeleteUserRoute.ResourceUri(request.UserId), Method.Delete);

        await backEndApiService.SendRequestAsync(restRequest, cancellationToken);
    }
}
