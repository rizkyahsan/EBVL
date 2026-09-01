using EBVL.FrontEnd.WebUi.Layouts.Statics;

namespace EBVL.FrontEnd.WebUi.Layouts;

public partial class LayoutLanding
{
    [Inject]
    public required IDialogService DialogService { get; init; }

    private static readonly MudTheme _theme = NewTheme();

    private static MudTheme NewTheme()
    {
        return ThemeFor.Default.Clone();
    }

    private async Task ShowSignInDialog()
    {
        _ = await DialogService.ShowAsync<Components.DialogSignIn>(
            string.Empty,
            new DialogOptions
            {
                MaxWidth = MaxWidth.Small,
                FullWidth = true,
                CloseButton = true
            });
    }
}
