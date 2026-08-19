using EBVL.Shared.Dto.Modules.Authentication.ExternalUsers.VerifiedExternalUser;

namespace EBVL.FrontEnd.Logics.Modules.Authentication.ExternalUsers.VerifiedExternalUser;

public sealed record VerifiedExternalUserCommand : VerifiedExternalUserRequest, IRequest<VerifiedExternalUserResponse>
{
}

public sealed class VerifiedExternalUserCommandValidator : AbstractValidatorBase<VerifiedExternalUserCommand>
{
    public VerifiedExternalUserCommandValidator()
    {
        Include(new VerifiedExternalUserRequestValidator());
    }
}

public sealed class VerifiedExternalUserCommandHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<VerifiedExternalUserCommand, VerifiedExternalUserResponse>
{
    public async Task<VerifiedExternalUserResponse> Handle(VerifiedExternalUserCommand request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(VerifiedExternalUserRoute.ResourceUri(request.ExternalLoginId), Method.Post);
        _ = restRequest.AddJsonBody(request);

        return await backEndApiService.SendRequestAsync<VerifiedExternalUserResponse>(restRequest, cancellationToken);
    }
}
