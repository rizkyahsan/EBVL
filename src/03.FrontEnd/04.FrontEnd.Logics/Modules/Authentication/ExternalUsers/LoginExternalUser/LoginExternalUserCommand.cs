using EBVL.Shared.Dto.Modules.Authentication.ExternalUsers.LoginExternalUser;

namespace EBVL.FrontEnd.Logics.Modules.Authentication.ExternalUsers.LoginExternalUser;

public sealed record LoginExternalUserCommand : LoginExternalUserRequest, IRequest<LoginExternalUserResponse>
{
}

public sealed class LoginExternalUserCommandHandler(
    IBackEndApiService backEndApiService)
    : IRequestHandler<LoginExternalUserCommand, LoginExternalUserResponse>
{
    public async Task<LoginExternalUserResponse> Handle(LoginExternalUserCommand request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(LoginExternalUserRoute.ResourceUri, Method.Post);
        _ = restRequest.AddJsonBody(request);

        return await backEndApiService.SendRequestAsync<LoginExternalUserResponse>(restRequest, cancellationToken);
    }
}
