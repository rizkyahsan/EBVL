using EBVL.FrontEnd.WebUi.Common.Components.Abstracts;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.Vendor.RequestRegistrations.Components;

public partial class DialogDocumentPreview : DialogBase
{
    [Parameter, EditorRequired]
    public required string FileName { get; init; }
}
