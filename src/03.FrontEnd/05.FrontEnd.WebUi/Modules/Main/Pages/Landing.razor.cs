namespace EBVL.FrontEnd.WebUi.Modules.Main.Pages;

public partial class Landing
{
    [Inject]
    public required NavigationManager NavigationManager { get; init; }

    [Inject]
    public required IOptions<AppConfigFrontEndOptions> AppConfigFrontEndOptions { get; init; }

    [Parameter]
    [SupplyParameterFromQuery]
    public string? ReturnUrl { get; set; }

    [CascadingParameter]
    public Task<AuthenticationState> AuthenticationStateTask { get; set; } = default!;

    private string _loginRoute = AuthenticationRouteFor.Login();

    protected override async Task OnInitializedAsync()
    {
        var user = (await AuthenticationStateTask).User;

        if (user.Identity is not null && user.Identity.IsAuthenticated)
        {
            NavigationManager.NavigateTo(MainRouteFor.Index, true);

            return;
        }

        _loginRoute = AuthenticationRouteFor.Login(ReturnUrl);
    }
}
