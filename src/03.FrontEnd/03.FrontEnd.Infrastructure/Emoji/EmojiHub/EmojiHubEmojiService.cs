using System.Text.Json.Serialization;
using EBVL.FrontEnd.Services.Emoji;
using RestSharp;

namespace EBVL.FrontEnd.Infrastructure.Emoji.EmojiHub;

public sealed class EmojiHubEmojiService : IEmojiService
{
    private readonly RestClient _restClient;
    private readonly EmojiHubEmojiOptions _options;

    public EmojiHubEmojiService(IOptions<EmojiHubEmojiOptions> options, HttpClient httpClient)
    {
        _options = options.Value;
        httpClient.BaseAddress = new Uri(_options.BaseUrl);
        _restClient = new RestClient(httpClient);
    }

    public async Task<IEnumerable<EmojiItem>> SearchEmojis(string keyword, CancellationToken cancellationToken = default)
    {
        var restRequest = new RestRequest(_options.SearchEndpoint, Method.Get);
        _ = restRequest.AddParameter("q", keyword);

        var restResponse = await _restClient.ExecuteAsync<EmojiDto[]>(restRequest, cancellationToken);

        var data = restResponse.Data
            ?? throw new InvalidOperationException($"Failed to deserialize JSON Content into {nameof(EmojiDto)}[].");

        return data.Select(item => new EmojiItem
        {
            Name = item.Name,
            Category = item.Category,
            Group = item.Group,
            HtmlCodes = item.HtmlCodes,
            Unicodes = item.Unicodes
        });
    }

    public async Task<EmojiItem> GetRandomEmoji(CancellationToken cancellationToken = default)
    {
        var restRequest = new RestRequest(_options.RandomEndpoint, Method.Get);
        var restResponse = await _restClient.ExecuteAsync<EmojiDto>(restRequest, cancellationToken);

        var data = restResponse.Data
            ?? throw new InvalidOperationException($"Failed to deserialize JSON Content into {nameof(EmojiDto)}.");

        return new EmojiItem
        {
            Name = data.Name,
            Category = data.Category,
            Group = data.Group,
            HtmlCodes = data.HtmlCodes,
            Unicodes = data.Unicodes
        };
    }

    private sealed record EmojiDto
    {
        [JsonPropertyName("name")]
        public required string Name { get; init; }

        [JsonPropertyName("category")]
        public required string Category { get; init; }

        [JsonPropertyName("group")]
        public required string Group { get; init; }

        [JsonPropertyName("htmlCode")]
        public required string[] HtmlCodes { get; init; }

        [JsonPropertyName("unicode")]
        public required string[] Unicodes { get; init; }
    }
}
