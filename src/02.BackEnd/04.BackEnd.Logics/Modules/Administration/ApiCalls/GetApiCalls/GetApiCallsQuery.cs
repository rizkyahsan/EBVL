using EBVL.Shared.Dto.Modules.Administration.ApiCalls.GetApiCalls;

namespace EBVL.BackEnd.Logics.Modules.Administration.ApiCalls.GetApiCalls;

public sealed record GetApiCallsQuery : IRequest<GetApiCallsResponse>
{
}

public sealed class GetApiCallsQueryHandler(IDatabaseService databaseService)
    : IRequestHandler<GetApiCallsQuery, GetApiCallsResponse>
{
    public async Task<GetApiCallsResponse> Handle(GetApiCallsQuery request, CancellationToken cancellationToken)
    {
        var apiCalls = await databaseService.ApiCalls
            .Where(x => !x.IsDeleted)
            .OrderByDescending(x => x.Created)
            .Select(x => new ApiCallItem
            {
                Id = x.Id,
                Created = x.Created,
                ServiceName = x.ServiceName,
                ServiceProvider = x.ServiceProvider,
                ServiceCategory = x.ServiceCategory,
                RequestMethod = x.RequestMethod,
                ResponseStatusCode = x.ResponseStatusCode
            })
            .ToListAsync(cancellationToken);

        return new GetApiCallsResponse
        {
            Items = apiCalls
        };
    }
}
