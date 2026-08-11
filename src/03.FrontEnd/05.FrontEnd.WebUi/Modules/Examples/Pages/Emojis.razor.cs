using EBVL.FrontEnd.Services.Emoji;

namespace EBVL.FrontEnd.WebUi.Modules.Examples.Pages;

public partial class Emojis
{
    [Inject]
    public required IEmojiService EmojiService { get; init; }

    private EmojiItem? _randomEmoji;
    private Exception? _exceptionForGetRandomEmoji;

    private string? _keyword;
    private IEnumerable<EmojiItem> _emojis = [];
    private Exception? _exceptionForSearchEmojis;

    protected override void OnInitialized()
    {
        LoadBreadcrumbs();
    }

    protected override void LoadBreadcrumbs()
    {
        _breadcrumbItems =
        [
            MainBreadcrumbFor.Home,
            ExamplesBreadcrumbFor.Index,
            CommonBreadcrumbFor.Active(ExamplesDisplayTextFor.Defaults)
        ];
    }

    private async Task GetRandomEmoji()
    {
        try
        {
            _isLoading = true;
            _exceptionForGetRandomEmoji = null;

            _randomEmoji = await EmojiService.GetRandomEmoji();
        }
        catch (Exception exception)
        {
            _exceptionForGetRandomEmoji = exception;
        }
        finally
        {
            _isLoading = false;
        }
    }

    private async Task SearchEmojis()
    {
        try
        {
            _isLoading = true;
            _exceptionForSearchEmojis = null;

            if (string.IsNullOrWhiteSpace(_keyword))
            {
                throw new InvalidOperationException("Please enter a keyword to search for emojis.");
            }

            _emojis = await EmojiService.SearchEmojis(_keyword);

            if (!_emojis.Any())
            {
                throw new InvalidOperationException($"No emojis found for the keyword \"{_keyword}\".");
            }
        }
        catch (Exception exception)
        {
            _exceptionForSearchEmojis = exception;
        }
        finally
        {
            _isLoading = false;
        }
    }
}
