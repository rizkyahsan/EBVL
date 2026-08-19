using EBVL.Shared.Dto.Modules.MasterData.Lenders.GetLender;

namespace EBVL.FrontEnd.WebUi.Modules.MasterData.Features.Lenders.Components;

public partial class DrawerDetails
{
    [Parameter, EditorRequired]
    public LenderItem? Lender { get; set; }
}
