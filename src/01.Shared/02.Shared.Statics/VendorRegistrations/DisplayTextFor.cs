using EBVL.Shared.Statics.Common.Extensions;

namespace EBVL.Shared.Statics.VendorRegistrations;

public static class DisplayTextFor
{
    public static readonly string VendorRegistration = nameof(VendorRegistration).SplitWords();
    public static readonly string SapVendorNumber = nameof(SapVendorNumber).SplitWords();
    public static readonly string CompanyName = nameof(CompanyName).SplitWords();
    public static readonly string CompanyEmail = nameof(CompanyEmail).SplitWords();
    public static readonly string PicEmail = nameof(PicEmail).SplitWords();
    public static readonly string CompanyPhoneNumber = nameof(CompanyPhoneNumber).SplitWords();
    public static readonly string PicPhoneNumber = nameof(PicPhoneNumber).SplitWords();
    public const string Website = nameof(Website);
    public static readonly string CompanyService = nameof(CompanyService).SplitWords();
    public static readonly string FactoryCountry = nameof(FactoryCountry).SplitWords();
    public static readonly string FactoryAddress = nameof(FactoryAddress).SplitWords();
    public static readonly string BrandRepresentative = nameof(BrandRepresentative).SplitWords();
    public static readonly string CompanyStatus = nameof(CompanyStatus).SplitWords();
    public static readonly string IsRepresentativeInIndonesia = nameof(IsRepresentativeInIndonesia).SplitWords();
}
