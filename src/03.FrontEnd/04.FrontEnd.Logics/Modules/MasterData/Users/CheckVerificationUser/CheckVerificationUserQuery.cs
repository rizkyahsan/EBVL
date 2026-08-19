using EBVL.Shared.Dto.Modules.MasterData.Users.CheckVerificationUser;

namespace EBVL.FrontEnd.Logics.Modules.MasterData.Users.CheckVerificationUser;

public sealed record CheckVerificationUserQuery : CheckVerificationUserRequest, IRequest<CheckVerificationUserResponse>
{
}

public sealed class CheckVerificationUserQueryValidator : AbstractValidatorBase<CheckVerificationUserQuery>
{
    public CheckVerificationUserQueryValidator()
    {
        Include(new CheckVerificationUserRequestValidator());
    }
}

public sealed class CheckVerificationUserQueryHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<CheckVerificationUserQuery, CheckVerificationUserResponse>
{
    public async Task<CheckVerificationUserResponse> Handle(CheckVerificationUserQuery request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(CheckVerificationUserRoute.ResourceUri(request.UserId, request.Token), Method.Get);

        return await backEndApiService.SendRequestAsync<CheckVerificationUserResponse>(restRequest, cancellationToken);
    }
}
