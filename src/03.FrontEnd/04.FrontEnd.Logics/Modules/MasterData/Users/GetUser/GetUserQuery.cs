using EBVL.Shared.Dto.Modules.MasterData.Users.GetUser;

namespace EBVL.FrontEnd.Logics.Modules.MasterData.Users.GetUser;

public sealed record GetUserQuery : GetUserRequest, IRequest<GetUserResponse>
{
}

public sealed class GetUserQueryValidator : AbstractValidatorBase<GetUserQuery>
{
    public GetUserQueryValidator()
    {
        Include(new GetUserRequestValidator());
    }
}

public sealed class GetUserQueryHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<GetUserQuery, GetUserResponse>
{
    public async Task<GetUserResponse> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(GetUserRoute.ResourceUri(request.UserId), Method.Get);

        return await backEndApiService.SendRequestAsync<GetUserResponse>(restRequest, cancellationToken);
    }
}
