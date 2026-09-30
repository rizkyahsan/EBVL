namespace EBVL.Shared.Statics;

public static class RoleNameFor
{
    public const string Lender = nameof(Lender);
    public const string Admin = nameof(Admin);
    public const string SuperAdmin = nameof(SuperAdmin);

    public const string Viewer = "Viewer";
    public const string ItAdministrator = "IT Administrator";
    public const string Administrator = "Administrator";
    public const string AdminMsai = "Admin MSAI";
    public const string AnalisReliability = "Analis Reliability";
    public const string Vendor = "Vendor";
    public const string ChiefSection = "Chief Section";
    public const string VpSection = "VP Section";
    public const string VpReliability = "VP Reliability";
    public const string AnalisMsai = "Analis MSAI";
    public const string SeniorManagerMsai = "Senior Manager MSAI";
    public const string SpecialistSection = "Specialist Section";

    public static readonly string[] EbvlRoles =
    [
        Viewer,
        ItAdministrator,
        Administrator,
        AdminMsai,
        AnalisReliability,
        Vendor,
        ChiefSection,
        VpSection,
        VpReliability,
        AnalisMsai,
        SeniorManagerMsai,
        SpecialistSection
    ];
}
