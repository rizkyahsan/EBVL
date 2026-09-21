using EBVL.FrontEnd.WebUi.Modules.MasterData.Features.Questionnaires.Models;

namespace EBVL.FrontEnd.WebUi.Modules.MasterData.Features.Questionnaires.Components;

public partial class DialogBusinessProcess
{
    [Parameter, EditorRequired] public required BusinessProcessModel Model { get; set; }

    private MudForm _form = default!;

    private async Task Submit()
    {
        await _form.Validate();
        if (_form.IsValid)
        {
            Dialog.Close(DialogResult.Ok(Model));
        }
    }
}
