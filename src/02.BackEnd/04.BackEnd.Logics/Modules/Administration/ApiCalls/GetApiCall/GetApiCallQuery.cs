using System.Text.Json;
using EBVL.Shared.Dto.Modules.Administration.ApiCalls.GetApiCall;

namespace EBVL.BackEnd.Logics.Modules.Administration.ApiCalls.GetApiCall;

public sealed record GetApiCallQuery : GetApiCallRequest, IRequest<GetApiCallResponse>
{
}

public sealed class GetApiCallQueryValidator : AbstractValidatorBase<GetApiCallQuery>
{
    public GetApiCallQueryValidator()
    {
        Include(new GetApiCallRequestValidator());
    }
}

public sealed class GetApiCallQueryHandler(IDatabaseService databaseService)
    : IRequestHandler<GetApiCallQuery, GetApiCallResponse>
{
    private static readonly JsonSerializerOptions _options = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<GetApiCallResponse> Handle(GetApiCallQuery request, CancellationToken cancellationToken)
    {
        var apiCall = await databaseService.ApiCalls
            .Where(x => !x.IsDeleted && x.Id == request.ApiCallId)
            .Select(x => new ApiCallItem
            {
                Id = x.Id,
                Created = x.Created,
                CreatedBy = x.CreatedBy,
                ServiceName = x.ServiceName,
                ServiceProvider = x.ServiceProvider,
                ServiceCategory = x.ServiceCategory,
                RequestUrl = x.RequestUrl,
                RequestMethod = x.RequestMethod,
                RequestParameters = ToKeyValuePairs(x.RequestParameters),
                ResponseStatusCode = x.ResponseStatusCode,
                ResponseHeaders = ToKeyValuePairs(x.ResponseHeaders),
                ResponseContent = x.ResponseContent,
                ErrorMessage = x.ErrorMessage
            })
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw ExceptionFor.EntityNotFound(ApiCallsDisplayTextFor.ApiCall, CommonDisplayTextFor.Id, request.ApiCallId);

        return new GetApiCallResponse
        {
            Item = apiCall
        };
    }

    private static IEnumerable<KeyValuePair<string, string>> ToKeyValuePairs(string? jsonString)
    {
        if (string.IsNullOrWhiteSpace(jsonString))
        {
            return new List<KeyValuePair<string, string>>();
        }

        var pairs = JsonSerializer.Deserialize<List<Pair>>(jsonString, _options);

        if (pairs is null)
        {
            return new List<KeyValuePair<string, string>>();
        }

        return pairs.Select(pair => new KeyValuePair<string, string>(pair.Name, pair.Value));
    }

    private sealed record Pair(string Name, string Value);
}
