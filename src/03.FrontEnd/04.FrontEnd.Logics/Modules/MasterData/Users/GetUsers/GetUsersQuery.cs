using EBVL.Shared.Dto.Modules.MasterData.Users.GetUsers;

namespace EBVL.FrontEnd.Logics.Modules.MasterData.Users.GetUsers;

public sealed record GetUsersQuery : IRequest<GetUsersResponse>
{
}

public sealed class GetUsersQueryHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<GetUsersQuery, GetUsersResponse>
{
    public async Task<GetUsersResponse> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(GetUsersRoute.ResourceUri, Method.Get);

        return await backEndApiService.SendRequestAsync<GetUsersResponse>(restRequest, cancellationToken);
    }
}
