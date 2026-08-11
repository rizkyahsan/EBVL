using EBVL.Shared.Dto.Common.Audits;

namespace EBVL.FrontEnd.WebUi.Common.Components.Audits;

public partial class DialogAuditProperties
{
    [Parameter]
    public required AuditItemBase Audit { get; init; }
}
