using EBVL.Shared.Dto.Modules.Log.LogEmails.GetLogEmail;

namespace EBVL.FrontEnd.WebUi.Modules.Log.Features.LogEmails.Components;

public partial class DrawerDetails
{
    [Parameter, EditorRequired]
    public LogEmailItem? LogEmail { get; set; }
}
