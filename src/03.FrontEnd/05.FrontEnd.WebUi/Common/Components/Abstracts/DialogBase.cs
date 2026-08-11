namespace EBVL.FrontEnd.WebUi.Common.Components.Abstracts;

public abstract class DialogBase : CommonComponentBase
{
    [CascadingParameter]
    protected IMudDialogInstance Dialog { get; set; } = default!;

    protected void Cancel()
    {
        Dialog.Cancel();
    }
}
