using EBVL.FrontEnd.Logics.Modules.MasterData.Lenders.DeleteLender;

namespace EBVL.FrontEnd.WebUi.Modules.MasterData.Features.Lenders.Components;

public partial class DialogDelete
{
    [Parameter]
    public required DialogDeleteModel Model { get; set; }

    private async Task Submit()
    {
        try
        {
            _isLoading = true;

            ClearException();

            var command = new DeleteLenderCommand
            {
                LenderId = Model.LenderId
            };

            await Sender.Send(command);

            Snackbar.AddSuccess(SuccessMessageFor.Deleted(LendersDisplayTextFor.Lender, Model.LenderName));
            Dialog.Close();
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

public sealed record DialogDeleteModel
{
    public required Guid LenderId { get; init; }
    public required string LenderName { get; init; }
}
