using EBVL.Shared.Statics.Common.Extensions;

namespace EBVL.Shared.Statics.Lenders;

public static class DisplayTextFor
{
    public const string Lender = nameof(Lender);
    public const string Lenders = nameof(Lenders);

    public const string Username = nameof(Username);
    public const string Name = nameof(Name);
    public const string Address = nameof(Address);
    public const string Country = nameof(Country);
    public static readonly string CountryPhoneCode = nameof(CountryPhoneCode).SplitWords();
    public static readonly string PhoneNumber = nameof(PhoneNumber).SplitWords();
    public static readonly string EmailAddress = nameof(EmailAddress).SplitWords();
    public const string Website = nameof(Website);
}
