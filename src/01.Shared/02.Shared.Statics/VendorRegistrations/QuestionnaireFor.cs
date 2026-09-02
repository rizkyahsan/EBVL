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
        new(4, "Company Establishment", GeneralInformation),
        new(5, "Initial Production", GeneralInformation),
        new(6, "Headquarter Address", GeneralInformation),
        new(7, "Manufacturing Address", GeneralInformation),
        new(8, "Rep. Office Address", GeneralInformation, false),
        new(9, "Sole Agent Address", GeneralInformation, false),
        new(10, "Vendor Company Profile", GeneralInformation, IsFileUpload: true),
        new(11, "Representative Office Company Profile", GeneralInformation, false, IsFileUpload: true),
        new(12, "Sole Agent Company Profile", GeneralInformation, false, IsFileUpload: true),
        new(13, "The Latest Company Annual Report", GeneralInformation, false, IsFileUpload: true),

        new(14, "Business Card Name (Director)", VendorRepresentativeOffice, IsFileUpload: true),
        new(15, "ID Card/Passport for authorized Signatories", VendorRepresentativeOffice, false, IsFileUpload: true),
        new(16, "Deed of Company Establishment (ACTA/ Akta Pendirian Perusahaan)", VendorRepresentativeOffice, IsFileUpload: true),
        new(17, "Certificate of Company Domicile (COD/Surat Keterangan Domisili)", VendorRepresentativeOffice, IsFileUpload: true),
        new(18, "Tax Identification Number (TIN/NPWP)-Company", VendorRepresentativeOffice, IsFileUpload: true),
        new(19, "Certificate of Company Registration (TDP)", VendorRepresentativeOffice, IsFileUpload: true),

        new(20, "Business Card Name (Director)", SoleAgent, IsFileUpload: true),
        new(21, "Company Business Licenses (SIUP)", SoleAgent, IsFileUpload: true),
        new(22, "STP (Surat Tanda Pendaftaran) Kemendag/ Trade Ministry", SoleAgent, IsFileUpload: true),
        new(23, "Agency Agreement from Vendor", SoleAgent, IsFileUpload: true),
        new(24, "SKUP (Surat Kemampuan Usaha Penunjang) Migas/ESDM (Base on National Regulation)", SoleAgent, IsFileUpload: true),
        new(25, "SKT (Surat Keterangan Terdaftar) Pertamina", SoleAgent, IsFileUpload: true),
        new(26, "CSMS (Contractor Safety Management System) Certificate Pertamina (Only for SA Provide service for product)", SoleAgent, IsFileUpload: true),

        new(27, "Brand Certificate", ProductQualityGeneral, false, IsFileUpload: true),
        new(28, "Patent/License Certificate and/or International Product Design Standard Certificate", ProductQualityGeneral, false, IsFileUpload: true),
        new(29, "Component Supplier/Expert List", ProductQualityGeneral, false, IsFileUpload: true),
        new(30, "Sample Component Mill Certificate", ProductQualityGeneral, false, IsFileUpload: true),
        new(31, "User Satisfaction Letter (Testimony)", ProductQualityGeneral, false, IsFileUpload: true),
        new(32, "Local Content Certificate (TKDN from Kemenperin)", ProductQualityGeneral, false, IsFileUpload: true),

        new(33, "Latest Product Catalogue", ProductQualitySpecific, IsFileUpload: true),
        new(34, "Latest Product Experience List", ProductQualitySpecific, IsFileUpload: true),
        new(35, "Manufacturing Type - Fully (Design & Manufacturer) Packager/ Integrator", ProductQualitySpecific, IsFileUpload: true),
        new(36, "International Product Manufacturing Standard Certificate", ProductQualitySpecific, IsFileUpload: true),
        new(37, "Quality Management System", ProductQualitySpecific, IsFileUpload: true),
        new(38, "Sample of Inspection Test Plan", ProductQualitySpecific, IsFileUpload: true),
        new(39, "Sample of FAT Report", ProductQualitySpecific, false, IsFileUpload: true),
        new(40, "Sample Conformity Certificate", ProductQualitySpecific, false, IsFileUpload: true),
        new(41, "Sample of Site Acceptance Test (SAT) Report", ProductQualitySpecific, false, IsFileUpload: true),
        new(42, "Quality Guarantee Letter from HQ", ProductQualitySpecific, IsFileUpload: true),
        new(43, "Product Obsolescence Letter", ProductQualitySpecific, false, IsFileUpload: true),
        new(44, "Sample of TASA (Technical Assistance Services Agreement) Program", ProductQualitySpecific, false, IsFileUpload: true),

        new(45, "Market Share (%) in the world", ProductPositioning, false, IsFileUpload: true),
        new(46, "Country of Origin / Factory Location", ProductPositioning, IsFileUpload: true),
        new(47, "Product Regional Supply", ProductPositioning, IsFileUpload: true),
        new(48, "Brand Product Competitor", ProductPositioning, false, IsFileUpload: true),
        new(49, "Product Technology Leader/Follower", ProductPositioning, Placeholder: "example: Leader Technology (Not License from other Brand)"),
        new(50, "After sales service office/vendor", ProductPositioning, false, Placeholder: "example: Local owned by PT. Contromatic Prima Mandiri")
    ];

    public static IReadOnlyList<VendorQuestionDefinition> GetSection(string section, IReadOnlyList<int> order)
    {
        var positions = order.Select((number, index) => new { number, index }).ToDictionary(item => item.number, item => item.index);
        return [.. All.Where(question => question.Section == section).OrderBy(question => positions[question.Number])];
    }
}
