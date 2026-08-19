using EBVL.Shared.Dto.Modules.MasterData.Users.GetUser;

namespace EBVL.FrontEnd.WebUi.Modules.MasterData.Features.Users.Components;

public partial class DrawerDetails
{
    [Parameter, EditorRequired]
    public UserItem? User { get; set; }
}
