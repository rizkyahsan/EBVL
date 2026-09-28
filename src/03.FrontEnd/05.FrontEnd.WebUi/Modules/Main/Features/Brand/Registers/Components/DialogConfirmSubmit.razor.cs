namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.Brand.Registers.Components;

public partial class DialogConfirmSubmit
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
