using EBVL.FrontEnd.WebUi.Common.Components.Abstracts;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.MyProfile.Components;

public partial class DialogViewQrCode : DialogBase
{
    [Parameter]
    public required string DataUri { get; init; }
}
