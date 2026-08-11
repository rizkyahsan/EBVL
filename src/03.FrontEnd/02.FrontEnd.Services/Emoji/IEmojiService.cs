namespace EBVL.FrontEnd.Services.Emoji;

public interface IEmojiService
{
    public Task<EmojiItem> GetRandomEmoji(CancellationToken cancellationToken = default);
    public Task<IEnumerable<EmojiItem>> SearchEmojis(string keyword, CancellationToken cancellationToken = default);
}
