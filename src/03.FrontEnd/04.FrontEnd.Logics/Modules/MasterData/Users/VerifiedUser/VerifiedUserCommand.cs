using EBVL.Shared.Dto.Modules.MasterData.Users.VerifiedUser;

namespace EBVL.FrontEnd.Logics.Modules.MasterData.Users.VerifiedUser;

public sealed record VerifiedUserCommand : VerifiedUserRequest, IRequest<VerifiedUserResponse>
{
}

public sealed class VerifiedUserCommandValidator : AbstractValidatorBase<VerifiedUserCommand>
{
    public VerifiedUserCommandValidator()
    {
        Include(new VerifiedUserRequestValidator());
    }
}

public sealed class VerifiedUserCommandHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<VerifiedUserCommand, VerifiedUserResponse>
{
    public async Task<VerifiedUserResponse> Handle(VerifiedUserCommand request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(VerifiedUserRoute.ResourceUri(request.UserId), Method.Post);
        _ = restRequest.AddJsonBody(request);

        return await backEndApiService.SendRequestAsync<VerifiedUserResponse>(restRequest, cancellationToken);
    }
}
