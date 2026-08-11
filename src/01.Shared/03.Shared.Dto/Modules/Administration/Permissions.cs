namespace EBVL.Shared.Dto.Modules.Administration;

public static class Permissions
{
    public const string Administration = "SolTem2.Administration"; // Digunakan untuk memunculkan menu Administration
    public const string AdministrationApiCallsRead = "SolTem2.Administration.ApiCalls.Read"; // Digunakan juga untuk memunculkan submenu ApiCalls di Administration
    public const string AdministrationAuditsRead = "SolTem2.Administration.Audits.Read"; // Digunakan juga untuk memunculkan submenu Audits di Administration
    public const string AdministrationConfigurationsRead = "SolTem2.Administration.Configurations.Read"; // Digunakan juga untuk memunculkan submenu Configurations di Administration
    public const string AdministrationConfigurationsWrite = "SolTem2.Administration.Configurations.Write";

    public static readonly string[] All =
    [
        Administration,
        AdministrationApiCallsRead,
        AdministrationAuditsRead,
        AdministrationConfigurationsRead,
        AdministrationConfigurationsWrite
    ];
}
