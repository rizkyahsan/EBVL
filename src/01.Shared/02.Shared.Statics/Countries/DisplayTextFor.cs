using EBVL.Shared.Statics.Common.Extensions;

namespace EBVL.Shared.Statics.Countries;

public static class DisplayTextFor
{
    public const string Country = nameof(Country);
    public const string Countries = nameof(Countries);
    public static readonly string PhoneCode = nameof(PhoneCode).SplitWords();
    public static readonly string CurrencyCode = nameof(CurrencyCode).SplitWords();
    public const string Region = nameof(Region);
}
