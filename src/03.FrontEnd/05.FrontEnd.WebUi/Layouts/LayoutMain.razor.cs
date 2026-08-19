using System.Security.Claims;

namespace EBVL.FrontEnd.WebUi.Layouts;

public partial class LayoutMain
{
    [Inject]
    public required NavigationManager NavigationManager { get; init; }

    [Inject]
    public required ISnackbar Snackbar { get; init; }

    [CascadingParameter]
    public Task<AuthenticationState> AuthenticationStateTask { get; set; } = default!;

    private bool _isLoading = false;
    private ClaimsPrincipal? _user;

    protected override async Task OnInitializedAsync()
    {
        _isLoading = true;

        _user = (await AuthenticationStateTask).User;

        if (_user.Identity is null || !_user.Identity.IsAuthenticated)
        {
            Snackbar.AddWarning("You session not found, please relogin.");
            NavigationManager.NavigateTo(AuthenticationRouteFor.Logout(), true);
            return;
        }

        // Check whether the authenticated user has any role
        var hasRole = _user.Claims.Any(c => c.Type is ClaimTypes.Role or "role");

        if (!hasRole)
        {
            Snackbar.AddWarning("You dont have access to this application, please contact admin.");
            NavigationManager.NavigateTo(AuthenticationRouteFor.Logout(), true);
            return;
        }

        _isLoading = false;
    }

    private void ToggleDrawer()
    {
        DisplayInfo.IsDrawerOpen = !DisplayInfo.IsDrawerOpen;
    }
}
