using EBVL.Shared.Statics.Common.Extensions;

namespace EBVL.Shared.Statics.Users;

public static class DisplayTextFor
{
    public const string User = nameof(User);
    public const string Users = nameof(Users);
    public const string Profile = nameof(Profile);
    public static readonly string UserProfile = nameof(UserProfile).SplitWords();
    public static readonly string MyUser = nameof(MyUser).SplitWords();
    public static readonly string MyProfile = nameof(MyProfile).SplitWords();
    public static readonly string ResetPassword = nameof(ResetPassword).SplitWords();

    public const string Username = nameof(Username);
    public const string Name = nameof(Name);
    public const string Lender = nameof(Lender);
    public static readonly string PhoneNumber = nameof(PhoneNumber).SplitWords();
    public static readonly string EmailAddress = nameof(EmailAddress).SplitWords();
    public const string Otp = nameof(Otp);
    public static readonly string IsPic = nameof(IsPic).SplitWords();
    public static readonly string UserPic = nameof(UserPic).SplitWords();
}
