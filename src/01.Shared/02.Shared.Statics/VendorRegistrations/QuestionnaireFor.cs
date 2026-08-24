namespace EBVL.Shared.Statics.VendorRegistrations;

public sealed record VendorQuestionDefinition(
    int Number,
    string Label,
    string Section,
    bool IsRequired = true,
    string? Hint = null,
    string? Placeholder = null,
    bool IsFileUpload = false);

public static class QuestionnaireFor
{
    public const string GeneralInformation = "A. GENERAL INFORMATION";
    public const string VendorRepresentativeOffice = "A1. Vendor / Rep. Office";
    public const string SoleAgent = "A2. Sole Agent";
    public const string ProductQualityGeneral = "B1. General";
    public const string ProductQualitySpecific = "B2. Specific";
    public const string ProductPositioning = "C. PRODUCT POSITIONING & TECH. SUPPORT";

    public static readonly IReadOnlyList<VendorQuestionDefinition> All =
    [
        new(1, "Name of Brand", GeneralInformation),
        new(2, "Name of Product", GeneralInformation),
        new(3, "Name of Vendor / Company", GeneralInformation),
        new(4, "Company Establishment", GeneralInformation, Hint: "year: yyyy"),
        new(5, "Initial Production", GeneralInformation, Hint: "year and nation"),
        new(6, "Headquarter Address", GeneralInformation, Hint: "Country, Building, Street, Number, City, Tel, Fax, Email, website"),
        new(7, "Manufacturing Address", GeneralInformation, Hint: "Country, Building, Street, Number, City, Tel, Fax, Email, website"),
        new(8, "Rep. Office Address", GeneralInformation, false, "Country, Building, Street, Number, City, Tel, Fax, Email, website"),
        new(9, "Sole Agent Address", GeneralInformation, false, "Country, Building, Street, Number, City, Tel, Fax, Email, website"),
        new(10, "Vendor Comp. Profile", GeneralInformation),
        new(11, "Rep. Office Comp. Profile", GeneralInformation, false),
        new(12, "Sole Agent Comp. Profile", GeneralInformation, false),
        new(13, "Latest Company Annual Report", GeneralInformation),

        new(14, "Business Card Name (Director)", VendorRepresentativeOffice, IsFileUpload: true),
        new(15, "ID Card/Passport for authorized Signatories", VendorRepresentativeOffice),
        new(16, "Deed of Company Establishment", VendorRepresentativeOffice, Hint: "ACTA / Akta Pendirian Perusahaan"),
        new(17, "Certificate of Company Domicile - CoD", VendorRepresentativeOffice, Hint: "Surat Keterangan Domisili"),
        new(18, "Company Tax Identification Number", VendorRepresentativeOffice, Hint: "TIN/NPWP"),
        new(19, "Certificate of Company Registration (TDP)", VendorRepresentativeOffice),

        new(20, "Business Card Name (Director)", SoleAgent, IsFileUpload: true),
        new(21, "Company Business Licenses", SoleAgent, Hint: "SIUP", Placeholder: "example: SIUP No. XXXX"),
        new(22, "STP Kemendag / Trade Ministry", SoleAgent, Hint: "Surat Tanda Pendaftaran", Placeholder: "example: STP No. XXXX"),
        new(23, "Agency Agreement from Vendor", SoleAgent),
        new(24, "SKUP Migas/ESDM", SoleAgent, Hint: "Surat Kemampuan Usaha Penunjang", Placeholder: "example: SKUP No. XXXX"),
        new(25, "SKT from Pertamina", SoleAgent, false, "Surat Keterangan Terdaftar", "example: SKT No. XXXX"),
        new(26, "CSMS Pertamina", SoleAgent, Hint: "Contractor Safety Mgt. System - SA Provide service for product Only", Placeholder: "example: CSMS Cert. No. XXXX"),

        new(27, "Brand Certificate", ProductQualityGeneral),
        new(28, "Patent / License Certificate", ProductQualityGeneral, Hint: "Patent/License Certificate and/or International Product Design Standard Certificate", Placeholder: "Patent Certificate No. XXXX and/or API XX"),
        new(29, "Component Supplier/Expert List", ProductQualityGeneral),
        new(30, "Sample Component Mill Certificate", ProductQualityGeneral, Placeholder: "example: Mill Cert. No. XXXX"),
        new(31, "User Satisfaction Letter (Testimony)", ProductQualityGeneral),
        new(32, "Local Content Cert. %", ProductQualityGeneral, Hint: "TKDN from Kemenperin", Placeholder: "example: XX %"),

        new(33, "Latest Product Catalogue", ProductQualitySpecific),
        new(34, "Latest Product Experience List", ProductQualitySpecific, Hint: "Patent/License Certificate and/or International Product Design Standard Certificate", Placeholder: "Patent Certificate No. XXXX and/or API XX"),
        new(35, "Manufacturing Type", ProductQualitySpecific, Hint: "Fully (Design & Manufacturer) Packager/Integrator", Placeholder: "example: Fully Manufacturing Valve"),
        new(36, "International Product Manufacturing Standard Cert.", ProductQualitySpecific, Placeholder: "example: ISO 9001 / 14001 / OHSAS Cert."),
        new(37, "Quality Management System", ProductQualitySpecific),
        new(38, "Sample of Inspection Test Plan", ProductQualitySpecific),
        new(39, "Sample of FAT Report", ProductQualitySpecific, Hint: "Factory Acceptance Test"),
        new(40, "Sample Conformity Certificate", ProductQualitySpecific),
        new(41, "Sample of SAT Report", ProductQualitySpecific, false, "Site Acceptance Test"),
        new(42, "Quality Guarantee Letter from HQ", ProductQualitySpecific),
        new(43, "Product Obsolence Letter", ProductQualitySpecific, false),
        new(44, "Sample of TASA Program", ProductQualitySpecific, false, "Technical Assistance Services Agreement"),

        new(45, "Market Share (%) in the world", ProductPositioning, false),
        new(46, "Country of Origin / Factory Location", ProductPositioning),
        new(47, "Product Regional Supply", ProductPositioning),
        new(48, "Brand Product Competitor", ProductPositioning),
        new(49, "Product Technology Leader/Follower", ProductPositioning, false, Placeholder: "example: Leader Technology (Not License from other Brand)"),
        new(50, "After sales service office/vendor", ProductPositioning, Placeholder: "example: Local owned by PT. Contromatic Prima Mandiri")
    ];
}
