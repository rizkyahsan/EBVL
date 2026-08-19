using EBVL.Shared.Dto.Modules.MasterData.Users.ResetPasswordUser;

namespace EBVL.FrontEnd.Logics.Modules.MasterData.Users.ResetPasswordUser;

public sealed record ResetPasswordUserCommand : ResetPasswordUserRequest, IRequest<ResetPasswordUserResponse>
{
}

public sealed class ResetPasswordUserCommandValidator : AbstractValidatorBase<ResetPasswordUserCommand>
{
    public ResetPasswordUserCommandValidator()
    {
        Include(new ResetPasswordUserRequestValidator());
    }
}

public sealed class ResetPasswordUserCommandHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<ResetPasswordUserCommand, ResetPasswordUserResponse>
{
    public async Task<ResetPasswordUserResponse> Handle(ResetPasswordUserCommand request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(ResetPasswordUserRoute.ResourceUri(request.UserId), Method.Post);
        _ = restRequest.AddJsonBody(request);

        return await backEndApiService.SendRequestAsync<ResetPasswordUserResponse>(restRequest, cancellationToken);
    }
}
