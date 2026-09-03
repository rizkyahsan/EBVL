namespace EBVL.Shared.Dto.Modules.Main;

public static class Permissions
{
    public const string ClaimsRead = "fino.cr";
    public const string MainPage = "fino.mp";
    public const string MainPageMyProfile = "fino.mp.po";

    public static readonly string[] All =
    [
        MainPage,
        MainPageMyProfile,
        .. EBVL.Shared.Dto.Modules.MasterData.Questionnaires.QuestionnairePermissions.All
    ];
}
