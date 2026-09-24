using EBVL.Shared.Dto.Modules.Main.Vendor.RequestRegistrations.GetRequestRegistrations;

namespace EBVL.FrontEnd.Logics.Modules.Main.Vendor.RequestRegistrations.GetRequestRegistrations;

public sealed record GetRequestRegistrationsQuery : GetRequestRegistrationsRequest, IRequest<GetRequestRegistrationsResponse>;

public sealed class GetRequestRegistrationsQueryValidator : AbstractValidatorBase<GetRequestRegistrationsQuery>
{
    public GetRequestRegistrationsQueryValidator()
    {
        Include(new GetRequestRegistrationsRequestValidator());
    }
}

public sealed class GetRequestRegistrationsQueryHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<GetRequestRegistrationsQuery, GetRequestRegistrationsResponse>
{
    public async Task<GetRequestRegistrationsResponse> Handle(GetRequestRegistrationsQuery request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(GetRequestRegistrationsRoute.ResourceUri, Method.Get);
        restRequest.AddQueryStringParameters(request);

        return await backEndApiService.SendRequestAsync<GetRequestRegistrationsResponse>(restRequest, cancellationToken);
    }
}
