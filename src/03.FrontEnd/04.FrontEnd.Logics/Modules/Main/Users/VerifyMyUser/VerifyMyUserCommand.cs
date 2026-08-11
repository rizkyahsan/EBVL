using EBVL.Shared.Dto.Modules.Main.Users.VerifyMyUser;

namespace EBVL.FrontEnd.Logics.Modules.Main.Users.VerifyMyUser;

public sealed record VerifyMyUserCommand : VerifyMyUserRequest, IRequest
{
}

public sealed class VerifyMyUserCommandValidator : AbstractValidatorBase<VerifyMyUserCommand>
{
    public VerifyMyUserCommandValidator()
    {
        Include(new VerifyMyUserRequestValidator());
    }
}

public sealed class VerifyMyUserCommandHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<VerifyMyUserCommand>
{
    public async Task Handle(VerifyMyUserCommand request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(VerifyMyUserRoute.ResourceUri, Method.Patch)
            .AddJsonBody(request);

        await backEndApiService.SendRequestAsync(restRequest, cancellationToken);
    }
}
