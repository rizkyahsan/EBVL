namespace EBVL.Shared.Statics.VendorRegistrations;

public sealed record DocumentEvidenceDefinition(
    string Key,
    string Name,
    string Instruction,
    bool IsPrimaryCertificate = false);

public static class DocumentEvidenceFor
{
    public static readonly IReadOnlyList<DocumentEvidenceDefinition> All =
    [
        new("BrandRegistrationLetter", "Brand Registration Letter", "Klik untuk upload dokumen brand"),
        new("CompanyProfile", "Company Profile", "Upload Company Profile (PDF)"),
        new("ProductCatalog", "Product Catalog", "Upload Katalog Produk"),
        new("ProductExperienceList", "Product Experience List", "Upload Daftar Pengalaman Proyek"),
        new("CompanyTaxCard", "NPWP Perusahaan", "Upload Kartu NPWP"),
        new("PrimaryCertificate", "Brand Cert/STP/Authorized", "Pilih Sertifikat/Surat Utama", true)
    ];
}
