using System.Text;
using System.Text.Json.Serialization;
using EBVL.FrontEnd.Services.Emoji;
using RestSharp;

namespace EBVL.FrontEnd.Infrastructure.Emoji.EmojisWorld;

public sealed class EmojisWorldEmojiService : IEmojiService
{
    private readonly RestClient _restClient;
    private readonly EmojisWorldEmojiOptions _options;

    public EmojisWorldEmojiService(IOptions<EmojisWorldEmojiOptions> options, HttpClient httpClient)
    {
        _options = options.Value;
        httpClient.BaseAddress = new Uri(_options.BaseUrl);
        _restClient = new RestClient(httpClient);
    }

    public async Task<IEnumerable<EmojiItem>> SearchEmojis(string keyword, CancellationToken cancellationToken = default)
    {
        var restRequest = new RestRequest(_options.SearchEndpoint, Method.Get);
        _ = restRequest.AddParameter("q", keyword);

        var restResponse = await _restClient.ExecuteAsync<EmojisResponse>(restRequest, cancellationToken);

        var data = restResponse.Data
            ?? throw new InvalidOperationException($"Failed to deserialize JSON Content into {nameof(EmojisResponse)}.");

        return data.Emojis.Select(item => new EmojiItem
        {
            Name = item.Name,
            Category = item.Category.Name,
            Group = item.SubCategory.Name,
            HtmlCodes = [ConvertEmojiToHtmlCode(item.Emoji)],
            Unicodes = [$"U+{item.Unicode}"]
        });
    }

    public async Task<EmojiItem> GetRandomEmoji(CancellationToken cancellationToken = default)
    {
        var restRequest = new RestRequest(_options.RandomEndpoint, Method.Get);
        _ = restRequest.AddParameter("limit", "1");

        var restResponse = await _restClient.ExecuteAsync<EmojisResponse>(restRequest, cancellationToken);

        var data = restResponse.Data
            ?? throw new InvalidOperationException($"Failed to deserialize JSON Content into {nameof(EmojisResponse)}.");

        var item = data.Emojis[0];

        return new EmojiItem
        {
            Name = item.Name,
            Category = item.Category.Name,
            Group = item.SubCategory.Name,
            HtmlCodes = [ConvertEmojiToHtmlCode(item.Emoji)],
            Unicodes = [$"U+{item.Unicode}"]
        };
    }

    private static string ConvertEmojiToHtmlCode(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return input;
        }

        var result = new StringBuilder();

        var i = 0;

        while (i < input.Length)
        {
            var c = input[i];

            if (char.IsHighSurrogate(c))
            {
                if (i + 1 < input.Length && char.IsLowSurrogate(input[i + 1]))
                {
                    var codePoint = char.ConvertToUtf32(c, input[i + 1]);

                    _ = result.Append($"&#x{codePoint:X};");

                    i++;
                }
                else
                {
                    _ = result.Append(c);
                }
            }
            else if (char.IsLowSurrogate(c))
            {
                _ = result.Append(c);
            }
            else
            {
                if (char.GetUnicodeCategory(c) == System.Globalization.UnicodeCategory.OtherSymbol)
                {
                    _ = result.Append($"&#x{(int)c:X};");
                }
                else
                {
                    _ = result.Append(c);
                }
            }

            i++;
        }

        return result.ToString();
    }

    private sealed record EmojisResponse
    {
        [JsonPropertyName("results")]
        public required EmojiDto[] Emojis { get; init; }
    }

    private sealed record EmojiDto
    {
        [JsonPropertyName("name")]
        public required string Name { get; init; }

        [JsonPropertyName("emoji")]
        public required string Emoji { get; set; }

        [JsonPropertyName("unicode")]
        public required string Unicode { get; init; }

        [JsonPropertyName("category")]
        public required Category Category { get; init; }

        [JsonPropertyName("sub_category")]
        public required SubCategory SubCategory { get; init; }
    }

    public sealed record Category
    {
        [JsonPropertyName("name")]
        public required string Name { get; init; }
    }

    public sealed record SubCategory
    {
        [JsonPropertyName("name")]
        public required string Name { get; init; }
    }
}
