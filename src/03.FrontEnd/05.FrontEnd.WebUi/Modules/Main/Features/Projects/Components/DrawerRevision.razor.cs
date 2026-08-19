using EBVL.FrontEnd.Logics.Modules.Main.Projects.RevisionProjectLenderReq;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.Projects.Components;

public partial class DrawerRevision
{
    [Parameter, EditorRequired]
    public required string StageName { get; init; }

    [Parameter, EditorRequired]
    public required string LenderName { get; init; }

    [Parameter, EditorRequired]
    public EventCallback OnRevision { get; set; }

    [Parameter, EditorRequired]
    public required RevisionProjectLenderReqCommand? Model { get; set; }

    private MudForm _form = default!;
    private RevisionProjectLenderReqCommandValidator _validator = default!;

    protected override void OnInitialized()
    {
        _validator = new();
    }

    private async Task SendRevision()
    {
        ClearException();

        if (Model is null)
        {
            return;
        }

        try
        {
            _isLoading = true;

            await _form.RunValidation();
            await Sender.Send(Model);

            Snackbar.AddSuccess($"Success to send request revision {StageName} to {LenderName}");

            await IsOpenChanged.InvokeAsync(false);
            await OnRevision.InvokeAsync();
        }
        catch (Exception exception)
        {
            _exception = exception;
            Snackbar.AddErrors(_exception.GetAllErrorMessages());
        }
        finally
        {
            _isLoading = false;
        }
    }
}
