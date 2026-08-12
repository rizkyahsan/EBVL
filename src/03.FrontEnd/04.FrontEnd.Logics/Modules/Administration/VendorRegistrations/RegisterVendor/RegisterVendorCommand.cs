using EBVL.Shared.Dto.Modules.Administration.VendorRegistrations.RegisterVendor;

namespace EBVL.FrontEnd.Logics.Modules.Administration.VendorRegistrations.RegisterVendor;

public sealed record RegisterVendorCommand : RegisterVendorRequest, IRequest<RegisterVendorResponse>;

public sealed class RegisterVendorCommandValidator : AbstractValidatorBase<RegisterVendorCommand>
{
    public RegisterVendorCommandValidator()
    {
        Include(new RegisterVendorRequestValidator());
    }
}

public sealed class RegisterVendorCommandHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<RegisterVendorCommand, RegisterVendorResponse>
{
    public Task<RegisterVendorResponse> Handle(RegisterVendorCommand request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(RegisterVendorRoute.ResourceUri, Method.Post).AddJsonBody(request);
        return backEndApiService.SendAnonymousRequestAsync<RegisterVendorResponse>(restRequest, cancellationToken);
    }
}
