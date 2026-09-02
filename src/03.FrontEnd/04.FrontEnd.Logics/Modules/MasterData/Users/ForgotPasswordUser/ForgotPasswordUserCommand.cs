using EBVL.Shared.Dto.Modules.MasterData.Users.ForgotPasswordUser;

namespace EBVL.FrontEnd.Logics.Modules.MasterData.Users.ForgotPasswordUser;

public sealed record ForgotPasswordUserCommand : ForgotPasswordUserRequest, IRequest<ForgotPasswordUserResponse>;

public sealed class ForgotPasswordUserCommandValidator : AbstractValidatorBase<ForgotPasswordUserCommand>
{
    public ForgotPasswordUserCommandValidator()
    {
        Include(new ForgotPasswordUserRequestValidator());
    }
}

public sealed class ForgotPasswordUserCommandHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<ForgotPasswordUserCommand, ForgotPasswordUserResponse>
{
    public async Task<ForgotPasswordUserResponse> Handle(ForgotPasswordUserCommand request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(ForgotPasswordUserRoute.ResourceUri, Method.Post);
        _ = restRequest.AddJsonBody(request);
        return await backEndApiService.SendRequestAsync<ForgotPasswordUserResponse>(restRequest, cancellationToken);
    }
}
