using EBVL.FrontEnd.Logics.Modules.Authentication.ExternalUsers.LoginExternalUser;
using MediatR;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Pages;

public partial class Login
{
    [Inject]
    public required NavigationManager NavigationManager { get; init; }

    [Inject]
    public required ISender Sender { get; init; }

    [Parameter]
    [SupplyParameterFromQuery]
    public string? ReturnUrl { get; set; }

    protected bool _isLoading;
    protected Exception? _exception;
    private bool _showPassword;

    private LoginExternalUserCommand _model = default!;

    protected override void OnInitialized()
    {
        _model = new()
        {
            Username = string.Empty,
            Password = string.Empty
        };

        if (!Uri.IsWellFormedUriString(ReturnUrl, UriKind.Relative))
        {
            ReturnUrl = MainRouteFor.Landing;
        }
    }

    private void TogglePasswordVisibility()
    {
        _showPassword = !_showPassword;
    }

    private async Task ExecuteLogin()
    {
        try
        {
            _isLoading = true;
            _exception = null;

            var response = await Sender.Send(_model);

            if (!string.IsNullOrWhiteSpace(response.Item.ErrorMessage))
            {
                throw new Exception("Invalid username or password.");
            }

            if (response.Item.RequireOtp && response.Item.ExternalLoginId.HasValue)
            {
                NavigationManager.NavigateTo(MainRouteFor.LoginOtp($"{response.Item.ExternalLoginId}"), forceLoad: true);
            }
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
}
