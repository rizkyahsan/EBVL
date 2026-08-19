namespace EBVL.Shared.Dto.Modules.Administration;

public static class Permissions
{
    public const string Administration = "fino.ad"; // Digunakan untuk memunculkan menu Administration
    public const string AdministrationApiCallsRead = "fino.ad.ac.Read"; // Digunakan juga untuk memunculkan submenu ApiCalls di Administration
    public const string AdministrationAuditsRead = "fino.ad.ad.Read"; // Digunakan juga untuk memunculkan submenu Audits di Administration
    public const string AdministrationConfigurationsRead = "fino.ad.co.Read"; // Digunakan juga untuk memunculkan submenu Configurations di Administration
    public const string AdministrationConfigurationsWrite = "fino.ad.co.Write";

    public static readonly string[] All =
    [
        Administration,
        AdministrationApiCallsRead,
        AdministrationAuditsRead,
        AdministrationConfigurationsRead,
        AdministrationConfigurationsWrite
    ];
}
