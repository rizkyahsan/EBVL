namespace EBVL.Shared.Enums;

public enum AuditActionType
{
    [DisplayText("Add")]
    Add = 100,

    [DisplayText("Edit")]
    Edit = 200,

    [DisplayText("Activate")]
    Activate = 300,

    [DisplayText("Deactivate")]
    Deactivate = -300,

    [DisplayText("Delete")]
    Delete = -100
}
