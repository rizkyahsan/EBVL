using MediatR;

namespace EBVL.FrontEnd.WebUi.Common.Components.Abstracts;

public abstract class CommonComponentBase : ComponentBase
{
    [Inject]
    public required NavigationManager NavigationManager { get; init; }

    [Inject]
    public required ISnackbar Snackbar { get; init; }

    [Inject]
    public required IDialogService DialogService { get; init; }

    [Inject]
    public required ISender Sender { get; init; }

    [Inject]
    public required IOptions<AppConfigFrontEndOptions> AppConfigFrontEndOptions { get; init; }

    protected bool _isLoading;
    protected Exception? _exception;

    protected void ClearException()
    {
        _exception = null;
    }
}
