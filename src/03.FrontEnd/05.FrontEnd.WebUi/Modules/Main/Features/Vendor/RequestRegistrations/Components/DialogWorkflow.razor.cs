using EBVL.FrontEnd.WebUi.Common.Components.Abstracts;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.Vendor.RequestRegistrations.Components;

public partial class DialogWorkflow : DialogBase
{
    [Parameter, EditorRequired]
    public required string Title { get; init; }

    [Parameter, EditorRequired]
    public required string Message { get; init; }

    [Parameter]
    public string ConfirmText { get; init; } = "Back";

    [Parameter]
    public bool ShowCancel { get; init; }

    private void Confirm()
    {
        Dialog.Close(DialogResult.Ok(true));
    }

    private void CloseDialog()
    {
        Dialog.Cancel();
    }
}
