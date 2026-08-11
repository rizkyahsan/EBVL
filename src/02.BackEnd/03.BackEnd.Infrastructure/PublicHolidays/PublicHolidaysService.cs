using System.Text.Json;
using System.Text.Json.Serialization;
using EBVL.BackEnd.Services.PublicHolidays;
using RestSharp;

namespace EBVL.BackEnd.Infrastructure.PublicHolidays;

public sealed class PublicHolidaysService : IPublicHolidaysService
{
    private readonly RestClient _restClient;
    private readonly IDatabaseService _databaseService;

    public PublicHolidaysService(IOptions<PublicHolidaysOptions> options, HttpClient httpClient, IDatabaseService databaseService)
    {
        httpClient.BaseAddress = new Uri(options.Value.BaseUrl);
        _restClient = new RestClient(httpClient);
        _databaseService = databaseService;
    }

    public async Task<IEnumerable<PublicHolidayData>> GetPublicHolidaysAsync(int year, string countryCode, CancellationToken cancellationToken = default)
    {
        var restRequest = new RestRequest($"/{year}/{countryCode}", Method.Get);
        _ = restRequest.AddParameter("type", "public");
        _ = restRequest.AddParameter("fufu", "hehe");

        var restResponse = await _restClient.ExecuteAsync<PublicHolidayDto[]>(restRequest, cancellationToken);

        var url = _restClient.BuildUri(restRequest).ToString();
        var requestParameters = JsonSerializer.Serialize(restRequest.Parameters);

        var apiCall = new ApiCall
        {
            ServiceName = "Public Holidays",
            ServiceProvider = "Nager.Date",
            ServiceCategory = "Miscellaneous",
            RequestUrl = url,
            RequestMethod = restRequest.Method.ToString(),
            RequestParameters = requestParameters,
            ResponseStatusCode = (ushort)restResponse.StatusCode,
            ResponseContent = restResponse.Content,
            ResponseHeaders = JsonSerializer.Serialize(restResponse.Headers),
            ErrorMessage = restResponse.ErrorMessage
        };

        _ = await _databaseService.ApiCalls.AddAsync(apiCall, cancellationToken);
        _ = await _databaseService.SaveAsync(nameof(GetPublicHolidaysAsync), cancellationToken);

        var data = restResponse.Data
            ?? throw new InvalidOperationException($"Failed to deserialize JSON Content into {nameof(PublicHolidayDto)}[].");

        return data.Select(item => new PublicHolidayData
        {
            Date = item.Date,
            Name = item.Name,
            LocalName = item.LocalName,
            CountryCode = item.CountryCode
        });
    }

    private sealed record PublicHolidayDto
    {
        [JsonPropertyName("date")]
        public required DateOnly Date { get; init; }

        [JsonPropertyName("localName")]
        public required string LocalName { get; init; }

        [JsonPropertyName("name")]
        public required string Name { get; init; }

        [JsonPropertyName("countryCode")]
        public required string CountryCode { get; init; }
    }
}
