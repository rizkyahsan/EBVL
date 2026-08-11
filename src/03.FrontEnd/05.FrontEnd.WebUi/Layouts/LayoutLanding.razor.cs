using EBVL.FrontEnd.WebUi.Layouts.Statics;

namespace EBVL.FrontEnd.WebUi.Layouts;

public partial class LayoutLanding
{
    private static readonly MudTheme _theme = NewTheme();

    private static MudTheme NewTheme()
    {
        var theme = ThemeFor.Default.Clone();

        theme.LayoutProperties.AppbarHeight = "0px";

        return theme;
    }
}
