namespace EBVL.FrontEnd.Services.Emoji;

public sealed record EmojiItem
{
    public required string Name { get; init; }
    public required string Category { get; init; }
    public required string Group { get; init; }
    public required string[] HtmlCodes { get; init; }
    public required string[] Unicodes { get; init; }

    public string HtmlCode => HtmlCodes.FirstOrDefault() ?? string.Empty;
    public string HtmlCodesDisplayText => string.Join(" ", HtmlCodes);
    public string UnicodesDisplayText => string.Join(", ", Unicodes);
}
