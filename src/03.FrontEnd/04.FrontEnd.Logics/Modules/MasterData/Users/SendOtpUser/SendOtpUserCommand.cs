using EBVL.Shared.Dto.Modules.MasterData.Users.SendOtpUser;

namespace EBVL.FrontEnd.Logics.Modules.MasterData.Users.SendOtpUser;

public sealed record SendOtpUserCommand : SendOtpUserRequest, IRequest<SendOtpUserResponse>
{
}

public sealed class SendOtpUserCommandValidator : AbstractValidatorBase<SendOtpUserCommand>
{
    public SendOtpUserCommandValidator()
    {
        Include(new SendOtpUserRequestValidator());
    }
}

public sealed class SendOtpUserCommandHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<SendOtpUserCommand, SendOtpUserResponse>
{
    public async Task<SendOtpUserResponse> Handle(SendOtpUserCommand request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(SendOtpUserRoute.ResourceUri(request.UserId), Method.Post);
        _ = restRequest.AddJsonBody(request);

        return await backEndApiService.SendRequestAsync<SendOtpUserResponse>(restRequest, cancellationToken);
    }
}

