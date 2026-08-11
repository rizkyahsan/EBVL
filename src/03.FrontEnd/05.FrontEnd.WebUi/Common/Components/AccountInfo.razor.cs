using Microsoft.AspNetCore.Components.Routing;

namespace EBVL.FrontEnd.WebUi.Common.Components;

public sealed partial class AccountInfo : IDisposable
{
    [Inject]
    public required NavigationManager NavigationManager { get; init; }

    [Inject]
    public required IDialogService DialogService { get; init; }

    [Inject]
    public required ISnackbar Snackbar { get; init; }

    private bool _isProfileOpen = false;
    private string _loginRoute = AuthenticationRouteFor.Login();
    private string _logoutRoute = AuthenticationRouteFor.Logout();

    protected override void OnInitialized()
    {
        SetupRoutes(NavigationManager.Uri);
        NavigationManager.LocationChanged += OnLocationChanged;
    }

    private void ShowProfile()
    {
        _isProfileOpen = true;
    }

    private void HideProfile()
    {
        _isProfileOpen = false;
    }

    private void GoToMyProfile()
    {
        HideProfile();
        NavigationManager.NavigateTo(MainRouteFor.MySession);
    }

    private async Task ShowDialogSwitchPosition()
    {
        var dialog = await DialogService.ShowAsync<DialogSwitchPosition>($"Switch {CommonDisplayTextFor.Position}");
        var result = await dialog.Result;

        if (result is not null && !result.Canceled && result.Data is not null)
        {
            var positionId = (string)result.Data;

            if (string.IsNullOrWhiteSpace(positionId))
            {
                Snackbar.AddError($"Selected {CommonDisplayTextFor.PositionId} is NULL.");

                return;
            }

            HideProfile();

            var uri = new Uri(NavigationManager.Uri);

            NavigationManager.NavigateTo(AuthenticationRouteFor.SwitchPosition(positionId, uri.AbsolutePath), forceLoad: true);
        }
    }

    private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        SetupRoutes(e.Location);
    }

    private void SetupRoutes(string currentUrl)
    {
        var uri = new Uri(currentUrl);

        _loginRoute = AuthenticationRouteFor.Login(uri.AbsolutePath);
        _logoutRoute = AuthenticationRouteFor.Logout(uri.AbsolutePath);

        StateHasChanged();
    }

    public void Dispose()
    {
        NavigationManager.LocationChanged -= OnLocationChanged;

        GC.SuppressFinalize(this);
    }
}
