using EBVL.Shared.Dto.Modules.Main.Users.UpdateMyUser;

namespace EBVL.FrontEnd.Logics.Modules.Main.Users.UpdateMyUser;

public sealed record UpdateMyUserCommand : UpdateMyUserRequest, IRequest
{
}

public sealed class UpdateMyUserCommandValidator : AbstractValidatorBase<UpdateMyUserCommand>
{
    public UpdateMyUserCommandValidator()
    {
        Include(new UpdateMyUserRequestValidator());
    }
}

public sealed class UpdateMyUserCommandHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<UpdateMyUserCommand>
{
    public async Task Handle(UpdateMyUserCommand request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(UpdateMyUserRoute.ResourceUri, Method.Patch)
            .AddJsonBody(request);

        await backEndApiService.SendRequestAsync(restRequest, cancellationToken);
    }
}
