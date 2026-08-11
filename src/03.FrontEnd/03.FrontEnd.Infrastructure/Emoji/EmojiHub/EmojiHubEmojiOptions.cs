namespace EBVL.FrontEnd.Infrastructure.Emoji.EmojiHub;

public sealed record EmojiHubEmojiOptions
{
    public const string SectionKey = $"{nameof(Emoji)}:{nameof(EmojiHub)}";

    public required string BaseUrl { get; init; }
    public required string RandomEndpoint { get; init; }
    public required string SearchEndpoint { get; init; }
}
