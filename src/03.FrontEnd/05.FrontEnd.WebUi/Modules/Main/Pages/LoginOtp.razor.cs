using EBVL.FrontEnd.Infrastructure.Authentication;
using EBVL.FrontEnd.Logics.Modules.Authentication.ExternalUsers.CheckExternalUser;
using EBVL.FrontEnd.Logics.Modules.Authentication.ExternalUsers.SendOtpExternalUser;
using EBVL.FrontEnd.Logics.Modules.Authentication.ExternalUsers.VerifiedExternalUser;
using MediatR;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Pages;

public partial class LoginOtp
{
    [Inject]
    public required NavigationManager NavigationManager { get; init; }

    [Inject]
    public required IHttpContextAccessor HttpContextAccessor { get; init; }

    [Inject]
    public required ISender Sender { get; init; }

    [Inject]
    public required ISnackbar Snackbar { get; init; }

    [Parameter]
    public string Id { get; set; } = default!;

    [Parameter]
    [SupplyParameterFromQuery]
    public string? ReturnUrl { get; set; }

    private bool _isVerifing = true;
    private bool _isLoading = false;
    private bool _isSuccess = false;
    protected Exception? _exception;
    private bool _showOtp;
    protected bool _isAllowAccess;
    private string _url = string.Empty;

    private VerifiedExternalUserCommand _model = default!;

    protected override async Task OnInitializedAsync()
    {
        if (string.IsNullOrEmpty(Id))
        {
            NavigationManager.NavigateTo(MainRouteFor.Landing, forceLoad: true);
        }

        await ValidateAccess();
    }

    protected void ClearException()
    {
        _exception = null;
    }

    private async Task ValidateAccess()
    {
        try
        {
            _isVerifing = true;
            _exception = null;

            var query = new CheckExternalUserQuery()
            {
                ExternalLoginId = new Guid(Id)
            };

            var response = await Sender.Send(query);

            _isAllowAccess = response.Item.IsVerified;

            await Task.Delay(5000);

            if (!_isAllowAccess)
            {
                NavigationManager.NavigateTo(MainRouteFor.Landing, forceLoad: true);
            }

            _model = new()
            {
                ExternalLoginId = new Guid(Id),
                VerificationCode = string.Empty
            };
        }
        catch (Exception exception)
        {
            _exception = exception;
        }
        finally
        {
            _isVerifing = false;
            await InvokeAsync(StateHasChanged);
        }
    }

    private async Task SendVerificationCode()
    {
        try
        {
            _isLoading = true;

            ClearException();

            var command = new SendOtpExternalUserCommand()
            {
                ExternalLoginId = new Guid(Id),
            };

            var response = await Sender.Send(command);
            Snackbar.AddSuccess(response.Item.Message);

            await Task.Delay(500);
        }
        catch (Exception exception)
        {
            _exception = exception;
        }
        finally
        {
            _isLoading = false;
            await InvokeAsync(StateHasChanged);
        }
    }

    private void ToggleOtpVisibility()
    {
        _showOtp = !_showOtp;
    }

    private async Task ExecuteUserVerification()
    {
        try
        {
            _isLoading = true;
            _exception = null;

            var response = await Sender.Send(_model);

            if (response.Item.Succeeded)
            {
                _isSuccess = true;
                _isLoading = false;

                Snackbar.AddSuccess("Success to Login, page will automatic redirect...");

                await InvokeAsync(StateHasChanged);

                #region Create Session User Token
                var httpContext = HttpContextAccessor.HttpContext ?? throw new InvalidOperationException();
                var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                var userAgent = httpContext.Request.Headers.UserAgent.ToString();
                var sessionId = UserTokenStore.CreateSession(response.Item.UserToken!, ipAddress, userAgent);

                if (!string.IsNullOrWhiteSpace(ReturnUrl))
                {
                    _url = AuthenticationRouteFor.LocalLoginHandler($"{sessionId}", ReturnUrl);
                }
                else
                {
                    _url = AuthenticationRouteFor.LocalLoginHandler($"{sessionId}");

                }
                #endregion

                await Task.Delay(5000);

                NavigationManager.NavigateTo(_url, forceLoad: true);
            }
        }
        catch (Exception exception)
        {
            _exception = exception;
        }
        finally
        {
            _isLoading = false;
            await InvokeAsync(StateHasChanged);
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
}
