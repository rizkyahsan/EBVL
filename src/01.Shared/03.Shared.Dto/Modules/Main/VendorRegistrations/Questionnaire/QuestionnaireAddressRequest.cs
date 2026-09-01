namespace EBVL.Shared.Dto.Modules.Main.VendorRegistrations.Questionnaire;

public sealed record QuestionnaireAddressRequest
{
    public string Country { get; set; } = string.Empty;
    public string Building { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string Number { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Fax { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;
}
