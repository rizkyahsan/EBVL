using EBVL.Shared.Dto.Modules.Main.Users.GetMyUser;

namespace EBVL.FrontEnd.Logics.Modules.Main.Users.GetMyUser;

public sealed record GetMyUserQuery : IRequest<GetMyUserResponse>
{
}

public sealed class GetMyUserQueryHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<GetMyUserQuery, GetMyUserResponse>
{
    public async Task<GetMyUserResponse> Handle(GetMyUserQuery request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(GetMyUserRoute.ResourceUri, Method.Get);

        return await backEndApiService.SendRequestAsync<GetMyUserResponse>(restRequest, cancellationToken);
    }
}
