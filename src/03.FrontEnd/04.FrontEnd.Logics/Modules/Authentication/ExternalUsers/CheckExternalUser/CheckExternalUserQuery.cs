using EBVL.Shared.Dto.Modules.Authentication.ExternalUsers.CheckExternalUser;

namespace EBVL.FrontEnd.Logics.Modules.Authentication.ExternalUsers.CheckExternalUser;

public sealed record CheckExternalUserQuery : CheckExternalUserRequest, IRequest<CheckExternalUserResponse>
{
}

public sealed class CheckExternalUserQueryValidator : AbstractValidatorBase<CheckExternalUserQuery>
{
    public CheckExternalUserQueryValidator()
    {
        Include(new CheckExternalUserRequestValidator());
    }
}

public sealed class CheckExternalUserQueryHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<CheckExternalUserQuery, CheckExternalUserResponse>
{
    public async Task<CheckExternalUserResponse> Handle(CheckExternalUserQuery request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(CheckExternalUserRoute.ResourceUri(request.ExternalLoginId), Method.Get);

        return await backEndApiService.SendRequestAsync<CheckExternalUserResponse>(restRequest, cancellationToken);
    }
}
