namespace EBVL.Shared.Dto.Modules.Main;

public static class Permissions
{
    public const string ClaimsRead = "ebvl.cr";
    public const string MainPage = "ebvl.mp";
    public const string MainPageMyProfile = "ebvl.mp.po";

    public static readonly string[] All =
    [
        ClaimsRead,
        MainPage,
        MainPageMyProfile,
        .. MasterData.Questionnaires.QuestionnairePermissions.All
    ];
}
