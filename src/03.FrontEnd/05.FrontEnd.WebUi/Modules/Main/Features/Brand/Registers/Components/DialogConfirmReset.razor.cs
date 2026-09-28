namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.Brand.Registers.Components;

public partial class DialogConfirmReset
{
    private void Confirm()
    {
        Dialog.Close(DialogResult.Ok(true));
    }

    private void CloseDialog()
    {
        Dialog.Cancel();
    }
}
