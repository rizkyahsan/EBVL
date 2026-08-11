using EBVL.Shared.Dto.Modules.Main.Users.ReloadMyUser;

namespace EBVL.FrontEnd.Logics.Modules.Main.Users.ReloadMyUser;

public sealed record ReloadMyUserCommand : ReloadMyUserRequest, IRequest
{
}

public sealed class ReloadMyUserCommandValidator : AbstractValidatorBase<ReloadMyUserCommand>
{
    public ReloadMyUserCommandValidator()
    {
        Include(new ReloadMyUserRequestValidator());
    }
}

public sealed class ReloadMyUserCommandHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<ReloadMyUserCommand>
{
    public async Task Handle(ReloadMyUserCommand request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(ReloadMyUserRoute.ResourceUri, Method.Patch)
            .AddJsonBody(request);

        await backEndApiService.SendRequestAsync(restRequest, cancellationToken);
    }
}
