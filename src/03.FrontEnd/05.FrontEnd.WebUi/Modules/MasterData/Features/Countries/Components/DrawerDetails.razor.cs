using EBVL.Shared.Dto.Modules.MasterData.Countries.GetCountry;

namespace EBVL.FrontEnd.WebUi.Modules.MasterData.Features.Countries.Components;

public partial class DrawerDetails
{
    [Parameter, EditorRequired]
    public CountryItem? Country { get; set; }
}
