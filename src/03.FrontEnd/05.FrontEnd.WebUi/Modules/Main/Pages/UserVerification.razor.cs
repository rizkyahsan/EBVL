using EBVL.FrontEnd.Logics.Modules.MasterData.Users.CheckVerificationUser;
using EBVL.FrontEnd.Logics.Modules.MasterData.Users.SendOtpUser;
using EBVL.FrontEnd.Logics.Modules.MasterData.Users.VerifiedUser;
using MediatR;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Pages;

public partial class UserVerification
{
    [Inject]
    public required NavigationManager NavigationManager { get; init; }

    [Inject]
    public required ISender Sender { get; init; }

    [Inject]
    public required ISnackbar Snackbar { get; init; }

    [Parameter]
    public string Id { get; set; } = default!;

    [SupplyParameterFromQuery]
    public string Token { get; set; } = default!;

    private bool _isVerifing = true;
    private bool _isLoading = false;
    private bool _isSuccess = false;
    protected Exception? _exception;
    private bool _showPassword;
    private bool _showOtp;
    protected bool _isAllowAccess;
    protected string _username = string.Empty;

    private VerifiedUserCommand _model = default!;

    protected override async Task OnInitializedAsync()
    {
        if (string.IsNullOrEmpty(Id) || string.IsNullOrEmpty(Token))
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

            var query = new CheckVerificationUserQuery()
            {
                UserId = new Guid(Id),
                Token = Token
            };

            var response = await Sender.Send(query);

            _isAllowAccess = response.Item.IsVerified;

            await Task.Delay(3000);

            if (!_isAllowAccess)
            {
                NavigationManager.NavigateTo(MainRouteFor.Landing, forceLoad: true);
            }

            _username = response.Item.Username;
            _model = new()
            {
                UserId = new Guid(Id),
                Token = Token,
                VerificationCode = string.Empty,
                Password = string.Empty
            };
        }
        catch (Exception exception)
        {
            _exception = exception;
        }
        finally
        {
            _isVerifing = false;
            StateHasChanged();
        }
    }

    private async Task SendVerificationCode()
    {
        try
        {
            _isLoading = true;

            ClearException();

            var command = new SendOtpUserCommand()
            {
                UserId = new Guid(Id),
                Token = Token
            };

            var response = await Sender.Send(command);

            Snackbar.AddSuccess(response.Item.Message);
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

    private void TogglePasswordVisibility()
    {
        _showPassword = !_showPassword;
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

            if (!string.IsNullOrWhiteSpace(response.Item.ErrorMessage))
            {
                throw new Exception(response.Item.ErrorMessage);
            }

            if (response.Item.Succeeded)
            {
                _isSuccess = true;
                _isLoading = false;

                Snackbar.AddSuccess(SuccessMessageFor.Activated(UsersDisplayTextFor.User, _username));
                StateHasChanged();

                await Task.Delay(5000);

                NavigationManager.NavigateTo(MainRouteFor.Landing, forceLoad: true);
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
}
