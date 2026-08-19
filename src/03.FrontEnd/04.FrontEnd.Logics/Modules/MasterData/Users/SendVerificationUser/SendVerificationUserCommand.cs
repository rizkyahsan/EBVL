using EBVL.Shared.Dto.Modules.MasterData.Users.SendVerificationUser;

namespace EBVL.FrontEnd.Logics.Modules.MasterData.Users.SendVerificationUser;

public sealed record SendVerificationUserCommand : SendVerificationUserRequest, IRequest<SendVerificationUserResponse>
{
}

public sealed class SendVerificationUserCommandValidator : AbstractValidatorBase<SendVerificationUserCommand>
{
    public SendVerificationUserCommandValidator()
    {
        Include(new SendVerificationUserRequestValidator());
    }
}

public sealed class SendVerificationUserCommandHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<SendVerificationUserCommand, SendVerificationUserResponse>
{
    public async Task<SendVerificationUserResponse> Handle(SendVerificationUserCommand request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(SendVerificationUserRoute.ResourceUri(request.UserId), Method.Post);
        _ = restRequest.AddJsonBody(request);

        return await backEndApiService.SendRequestAsync<SendVerificationUserResponse>(restRequest, cancellationToken);
    }
}

