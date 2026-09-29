namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.Brand.Registers.Components;

public partial class DialogDisposition
{
    protected List<DispositionEmployee> Employees { get; } =
    [
        new("John Doe", "john.doe@pertamina.com", "Analis Junior II"),
        new("Jane Doe", "jane.doe@pertamina.com", "Analis Senior I"),
        new("Ludwig", "ludwig@pertamina.com", "Analis Junior I"),
        new("Carter", "carter@pertamina.com", "Analis Junior II"),
        new("Loki", "loki@pertamina.com", "Analis Senior I"),
        new("Thor Odinson", "thor.odinson@pertamina.com", "Analis Senior I"),
        new("Peter Parker", "peter.parker@pertamina.com", "Analis Senior I"),
        new("Tony Stark", "tony.stark@pertamina.com", "Analis Senior II")
    ];

    protected void Select(DispositionEmployee employee)
    {
        Dialog.Close(DialogResult.Ok(employee));
    }

    protected sealed record DispositionEmployee(string Name, string Email, string Position);
}
