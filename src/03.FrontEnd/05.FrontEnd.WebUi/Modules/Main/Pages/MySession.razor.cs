using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using EBVL.FrontEnd.Infrastructure.Authentication.Statics;
using EBVL.FrontEnd.WebUi.Common.Services.Clipboard;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Pages;

public partial class MySession
{
    [Inject]
    public required AuthenticationStateProvider AuthenticationStateProvider { get; init; }

    [Inject]
    public required ClipboardService ClipboardService { get; init; }

    private ClaimsPrincipal _user = default!;
    private IEnumerable<string> _roles = [];
    private IEnumerable<string> _permissions = [];
    private string? _accessToken;
    private string? _refreshToken;

    protected override async Task OnInitializedAsync()
    {
        LoadBreadcrumbs();

        _user = (await AuthenticationStateProvider.GetAuthenticationStateAsync()).User;
        _roles = _user.GetRoles();
        _permissions = _user.GetPermissions();

        var httpContext = new HttpContextAccessor().HttpContext;

        if (httpContext is not null)
        {
            _accessToken = await httpContext.GetTokenAsync(TokenNameFor.AccessToken);
            _refreshToken = await httpContext.GetTokenAsync(TokenNameFor.RefreshToken);
        }
    }

    protected override void LoadBreadcrumbs()
    {
        _breadcrumbItems =
        [
            MainBreadcrumbFor.Home,
            CommonBreadcrumbFor.Active(MainDisplayTextFor.MySession)
        ];
    }

    private async Task CopyToClipBoard(string content)
    {
        await ClipboardService.WriteTextAsync(content);

        Snackbar.AddSuccess($"The content has been copied to the clipboard.");
    }
}
