using EBVL.FrontEnd.Infrastructure.Authentication;
using EBVL.FrontEnd.Logics.Modules.Authentication.ExternalUsers.LoginExternalUser;
using MediatR;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Pages;

public partial class Login
{
    #region Dependencies and Parameters

    [Inject]
    public required NavigationManager NavigationManager { get; init; }

    [Inject]
    public required ISender Sender { get; init; }

    [Inject]
    public required IHttpContextAccessor HttpContextAccessor { get; init; }

    [Parameter]
    [SupplyParameterFromQuery]
    public string? ReturnUrl { get; set; }

    #endregion

    #region State

    protected bool _isLoading;
    protected Exception? _exception;
    private bool _showPassword;

    private LoginExternalUserCommand _model = default!;

    #endregion

    #region Lifecycle

    protected override void OnInitialized()
    {
        _model = new()
        {
            Username = string.Empty,
            Password = string.Empty
        };

        if (!IsSafeLocalReturnUrl(ReturnUrl))
        {
            ReturnUrl = null;
        }
    }

    #endregion

    #region Event Handlers

    private void TogglePasswordVisibility()
    {
        _showPassword = !_showPassword;
    }

    private async Task ExecuteLogin()
    {
        if (_isLoading)
        {
            return;
        }

        try
        {
            _isLoading = true;
            _exception = null;

            var response = await Sender.Send(_model);

            if (!string.IsNullOrWhiteSpace(response.Item.ErrorMessage))
            {
                throw new Exception("Invalid username or password.");
            }

            if (!response.Item.Succeeded || string.IsNullOrWhiteSpace(response.Item.UserToken))
            {
                throw new Exception("Invalid username or password.");
            }

            var httpContext = HttpContextAccessor.HttpContext ?? throw new InvalidOperationException();
            var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var userAgent = httpContext.Request.Headers.UserAgent.ToString();
            var sessionId = UserTokenStore.CreateSession(response.Item.UserToken, ipAddress, userAgent);
            var url = string.IsNullOrWhiteSpace(ReturnUrl)
                ? AuthenticationRouteFor.LocalLoginHandler($"{sessionId}")
                : AuthenticationRouteFor.LocalLoginHandler($"{sessionId}", ReturnUrl);
            NavigationManager.NavigateTo(url, forceLoad: true);
        }
        catch (Exception exception)
        {
            _exception = exception;
        }
        finally
        {
            _isLoading = false;
        }
    }

    private void ExecuteReturn()
    {
        try
        {
            NavigationManager.NavigateTo(MainRouteFor.Index, forceLoad: true);
        }
        catch (Exception exception)
        {
            _exception = exception;
        }
        finally
        {
            _isLoading = false;
        }
    }

    #endregion

    #region Helpers

    private static bool IsSafeLocalReturnUrl(string? returnUrl)
    {
        return !string.IsNullOrWhiteSpace(returnUrl)
            && returnUrl[0] == '/'
            && !returnUrl.StartsWith("//", StringComparison.Ordinal)
            && !returnUrl.StartsWith("/\\", StringComparison.Ordinal)
            && !returnUrl.Contains('\\')
            && Uri.IsWellFormedUriString(returnUrl, UriKind.Relative);
    }

    #endregion
}
