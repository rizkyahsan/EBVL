using EBVL.Shared.Dto.Modules.MasterData.Users.SendResetPasswordUser;

namespace EBVL.FrontEnd.Logics.Modules.MasterData.Users.SendResetPasswordUser;

public sealed record SendResetPasswordUserCommand : SendResetPasswordUserRequest, IRequest<SendResetPasswordUserResponse>
{
}

public sealed class SendResetPasswordUserCommandValidator : AbstractValidatorBase<SendResetPasswordUserCommand>
{
    public SendResetPasswordUserCommandValidator()
    {
        Include(new SendResetPasswordUserRequestValidator());
    }
}

public sealed class SendResetPasswordUserCommandHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<SendResetPasswordUserCommand, SendResetPasswordUserResponse>
{
    public async Task<SendResetPasswordUserResponse> Handle(SendResetPasswordUserCommand request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(SendResetPasswordUserRoute.ResourceUri(request.UserId), Method.Post);
        _ = restRequest.AddJsonBody(request);

        return await backEndApiService.SendRequestAsync<SendResetPasswordUserResponse>(restRequest, cancellationToken);
    }
}

