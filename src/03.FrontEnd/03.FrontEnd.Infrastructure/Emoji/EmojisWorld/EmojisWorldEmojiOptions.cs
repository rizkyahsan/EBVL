namespace EBVL.FrontEnd.Infrastructure.Emoji.EmojisWorld;

public sealed record EmojisWorldEmojiOptions
{
    public const string SectionKey = $"{nameof(Emoji)}:{nameof(EmojisWorld)}";

    public required string BaseUrl { get; init; }
    public required string RandomEndpoint { get; init; }
    public required string SearchEndpoint { get; init; }
}
