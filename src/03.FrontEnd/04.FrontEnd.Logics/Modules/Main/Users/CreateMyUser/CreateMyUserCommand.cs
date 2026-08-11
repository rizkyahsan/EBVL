using EBVL.Shared.Dto.Modules.Main.Users.CreateMyUser;

namespace EBVL.FrontEnd.Logics.Modules.Main.Users.CreateMyUser;

public sealed record CreateMyUserCommand : IRequest<CreateMyUserResponse>
{
}

public sealed class CreateMyUserCommandHandler(
    IBackEndApiService backEndApiService)
    : IRequestHandler<CreateMyUserCommand, CreateMyUserResponse>
{
    public async Task<CreateMyUserResponse> Handle(CreateMyUserCommand request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(CreateMyUserRoute.ResourceUri, Method.Post);

        return await backEndApiService.SendRequestAsync<CreateMyUserResponse>(restRequest, cancellationToken);
    }
}
