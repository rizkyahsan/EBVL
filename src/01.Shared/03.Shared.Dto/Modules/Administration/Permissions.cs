namespace EBVL.Shared.Dto.Modules.Administration;

public static class Permissions
{
    public const string Administration = "ebvl.audit.index";
    public const string AdministrationApiCallsRead = "ebvl.ad.ac.Read"; // Digunakan juga untuk memunculkan submenu ApiCalls di Administration
    public const string AdministrationAuditsRead = "ebvl.audit.index";
    public const string AdministrationAuditsView = "ebvl.audit.view";
    public const string AdministrationConfigurationsRead = "ebvl.ad.co.Read"; // Digunakan juga untuk memunculkan submenu Configurations di Administration
    public const string AdministrationConfigurationsWrite = "ebvl.ad.co.Write";

    public static readonly string[] All =
    [
        Administration,
        AdministrationApiCallsRead,
        AdministrationAuditsView,
        AdministrationConfigurationsRead,
        AdministrationConfigurationsWrite
    ];
}
