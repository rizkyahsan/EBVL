using EBVL.Shared.Dto.Modules.Examples.Dummies.GetDummies;

namespace EBVL.FrontEnd.Logics.Modules.Examples.Dummies.GetDummies;

public sealed record GetDummiesQuery : GetDummiesRequest, IRequest<GetDummiesResponse>
{
}

public sealed class GetDummiesQueryValidator : AbstractValidatorBase<GetDummiesQuery>
{
    public GetDummiesQueryValidator()
    {
        Include(new GetDummiesRequestValidator());
    }
}

public sealed class GetDummiesQueryHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<GetDummiesQuery, GetDummiesResponse>
{
    public async Task<GetDummiesResponse> Handle(GetDummiesQuery request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(GetDummiesRoute.ResourceUri, Method.Get);
        restRequest.AddQueryStringParameters(request);

        return await backEndApiService.SendRequestAsync<GetDummiesResponse>(restRequest, cancellationToken);
    }
}
