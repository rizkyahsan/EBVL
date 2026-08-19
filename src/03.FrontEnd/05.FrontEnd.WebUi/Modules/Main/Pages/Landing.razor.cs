using EBVL.FrontEnd.WebUi.Layouts.Models;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Pages;

public partial class Landing
{
    [Inject]
    public required NavigationManager NavigationManager { get; init; }

    [Inject]
    public required IOptions<AppConfigFrontEndOptions> AppConfigFrontEndOptions { get; init; }

    [CascadingParameter]
    public DisplayInfo DisplayInfo { get; set; } = default!;

    [Parameter]
    [SupplyParameterFromQuery]
    public string? ReturnUrl { get; set; }

    [CascadingParameter]
    public Task<AuthenticationState> AuthenticationStateTask { get; set; } = default!;

    private string _loginRoute = AuthenticationRouteFor.Login();
    private string _localLoginRoute = AuthenticationRouteFor.LocalLoginPage();

    protected override async Task OnInitializedAsync()
    {
        var user = (await AuthenticationStateTask).User;

        if (user.Identity is not null && user.Identity.IsAuthenticated)
        {
            NavigationManager.NavigateTo(MainRouteFor.Index, true);

            return;
        }

        _loginRoute = AuthenticationRouteFor.Login(ReturnUrl);
        _localLoginRoute = AuthenticationRouteFor.LocalLoginPage(ReturnUrl);
    }
}
