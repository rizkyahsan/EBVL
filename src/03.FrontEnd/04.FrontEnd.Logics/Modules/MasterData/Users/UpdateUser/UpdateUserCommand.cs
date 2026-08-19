using EBVL.Shared.Dto.Modules.MasterData.Users.UpdateUser;

namespace EBVL.FrontEnd.Logics.Modules.MasterData.Users.UpdateUser;

public sealed record UpdateUserCommand : UpdateUserRequest, IRequest { }

public sealed class UpdateUserCommandValidator : AbstractValidatorBase<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        Include(new UpdateUserRequestValidator());
    }
}

public sealed class UpdateUserCommandHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<UpdateUserCommand>
{
    public async Task Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(UpdateUserRoute.ResourceUri(request.UserId), Method.Patch);
        _ = restRequest.AddJsonBody(request);

        await backEndApiService.SendRequestAsync(restRequest, cancellationToken);
    }
}
