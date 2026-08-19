namespace EBVL.Shared.Dto.Modules.Main;

public static class Permissions
{
    public const string ClaimsRead = "fino.cr";
    public const string MainPage = "fino.mp";
    public const string MainPageMyProfile = "fino.mp.po";
    public const string MainPageProject = "fino.mp.pr";
    public const string MainPageProjectRead = "fino.mp.pr.read";
    public const string MainPageProjectWrite = "fino.mp.pr.write";
    public const string MainPageProjectDownload = "fino.mp.pr.download";
    public const string MainPageProjectUpload = "fino.mp.pr.upload";
    public const string MainPageProjectVerify = "fino.mp.pr.verify";
    public const string MainPageMyProject = "fino.mp.mp";
    public const string MainPageMyProjectRead = "fino.mp.mp.read";
    public const string MainPageMyProjectWrite = "fino.mp.mp.write";
    public const string MainPageMyProjectDownload = "fino.mp.mp.download";
    public const string MainPageMyProjectUpload = "fino.mp.mp.upload";

    public static readonly string[] All =
    [
        MainPage,
        MainPageMyProfile,
        MainPageProject,
        MainPageProjectRead,
        MainPageProjectWrite,
        MainPageProjectDownload,
        MainPageProjectUpload,
        MainPageProjectVerify,
        MainPageMyProject,
        MainPageMyProjectRead,
        MainPageMyProjectWrite,
        MainPageMyProjectDownload,
        MainPageMyProjectUpload
    ];
}
