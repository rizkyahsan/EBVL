using EBVL.FrontEnd.WebUi.Layouts.Statics;

namespace EBVL.FrontEnd.WebUi.Layouts;

public partial class LayoutLanding
{
    private static readonly MudTheme _theme = NewTheme();

    private static MudTheme NewTheme()
    {
        return ThemeFor.Default.Clone();
    }
}
