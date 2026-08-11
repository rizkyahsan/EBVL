using EBVL.Shared.Dto.Modules.Main.Users.SendMyVerificationCode;

namespace EBVL.FrontEnd.Logics.Modules.Main.Users.SendMyVerificationCode;

public sealed class SendMyVerificationCodeCommand : IRequest<SendMyVerificationCodeResponse>
{
}

public sealed class SendMyVerificationCodeCommandHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<SendMyVerificationCodeCommand, SendMyVerificationCodeResponse>
{
    public async Task<SendMyVerificationCodeResponse> Handle(SendMyVerificationCodeCommand request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(SendMyVerificationCodeRoute.ResourceUri, Method.Post);

        return await backEndApiService.SendRequestAsync<SendMyVerificationCodeResponse>(restRequest, cancellationToken);
    }
}
