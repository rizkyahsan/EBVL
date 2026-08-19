using EBVL.Shared.Dto.Modules.Authentication.ExternalUsers.SendOtpExternalUser;

namespace EBVL.FrontEnd.Logics.Modules.Authentication.ExternalUsers.SendOtpExternalUser;

public sealed record SendOtpExternalUserCommand : SendOtpExternalUserRequest, IRequest<SendOtpExternalUserResponse>
{
}

public sealed class SendOtpExternalUserCommandValidator : AbstractValidatorBase<SendOtpExternalUserCommand>
{
    public SendOtpExternalUserCommandValidator()
    {
        Include(new SendOtpExternalUserRequestValidator());
    }
}

public sealed class SendOtpExternalUserCommandHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<SendOtpExternalUserCommand, SendOtpExternalUserResponse>
{
    public async Task<SendOtpExternalUserResponse> Handle(SendOtpExternalUserCommand request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(SendOtpExternalUserRoute.ResourceUri(request.ExternalLoginId), Method.Post);
        _ = restRequest.AddJsonBody(request);

        return await backEndApiService.SendRequestAsync<SendOtpExternalUserResponse>(restRequest, cancellationToken);
    }
}

