using EBVL.Shared.Dto.Modules.Administration.VendorRegistrations.AuthenticateVendor;

namespace EBVL.FrontEnd.Logics.Modules.Administration.VendorRegistrations.AuthenticateVendor;

public sealed record AuthenticateVendorCommand : AuthenticateVendorRequest, IRequest<AuthenticateVendorResponse>;

public sealed class AuthenticateVendorCommandValidator : AbstractValidatorBase<AuthenticateVendorCommand>
{
    public AuthenticateVendorCommandValidator()
    {
        Include(new AuthenticateVendorRequestValidator());
    }
}

public sealed class AuthenticateVendorCommandHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<AuthenticateVendorCommand, AuthenticateVendorResponse>
{
    public Task<AuthenticateVendorResponse> Handle(AuthenticateVendorCommand request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(AuthenticateVendorRoute.ResourceUri, Method.Post).AddJsonBody(request);
        return backEndApiService.SendAnonymousRequestAsync<AuthenticateVendorResponse>(restRequest, cancellationToken);
    }
}
