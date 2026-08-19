namespace EBVL.Shared.Statics.Common;

public static class ActionFor
{
    #region action for External User
    public const string SendOtp = "SendOtp";
    public const string SendVerificationCode = "SendVerificationCode";
    public const string SendResetPassword = "SendResetPassword";
    #endregion

    #region action for Main
    #region action for MyProjects
    public const string MainMyProjectsGetMyProject = "GetMyProject";
    public const string MainMyProjectsGetMyProjects = "GetMyProjects";
    public const string MainMyProjectsGetMyProjectStage = "GetMyProjectStage";
    public const string MainMyProjectsUpdateMyProjectStage = "UpdateMyProjectStage";
    public const string MainMyProjectsSubmitMyProjectStage = "SubmitMyProjectStage";
    #endregion

    #region action for Projects
    public const string MainProjectsCompleteProject = "CompleteProject";
    public const string MainProjectsCompleteProjectWin = "CompleteProjectWin";
    public const string MainProjectsCompleteProjectLose = "CompleteProjectLose";

    public const string MainProjectsCompleteProjectStage = "CompleteProjectStage";
    public const string MainProjectsCompleteProjectStageAccept = "CompleteProjectStageAccept";
    public const string MainProjectsCompleteProjectStageReject = "CompleteProjectStageReject";

    public const string MainProjectsCreateProject = "CreateProject";
    public const string MainProjectsCreateProjectStage = "CreateProjectStage";
    public const string MainProjectsDeleteProject = "DeleteProject";
    public const string MainProjectsDeleteFileProject = "DeleteProjectFile";
    public const string MainProjectsGetLastProjectStage = "GetLastProjectStage";
    public const string MainProjectsGetProject = "GetProject";
    public const string MainProjectsGetProjects = "GetProjects";
    public const string MainProjectsGetProjectStage = "GetProjectStage";
    public const string MainProjectsGetProjectVerifies = "GetProjectVerifies";
    public const string MainProjectsGetProjectVerify = "GetProjectVerify";
    public const string MainProjectsPublishProjectStage = "PublishProjectStage";
    public const string MainProjectsPublishProjectStageAdmin = "PublishProjectStageAdmin";
    public const string MainProjectsReviewProjectStage = "ReviewProjectStage";
    public const string MainProjectsRevisionProjectLenderReq = "RevisionProjectLenderReq";
    public const string MainProjectsUpdateProject = "UpdateProject";
    public const string MainProjectsUpdateProjectStage = "UpdateProjectStage";
    public const string MainProjectsUploadProjectFile = "UploadProjectFile";
    #endregion

    #region action for Users
    public const string MainCreateMyUser = "CreateMyUser";
    public const string MainGetMyUser = "GetMyUser";
    public const string MainReloadMyUser = "ReloadMyUser";
    public const string MainSendMyVerificationCode = "SendMyVerificationCode";
    public const string MainUpdateMyUser = "UpdateMyUser";
    public const string MainVerifyMyUser = "VerifyMyUser";
    #endregion
    #endregion

    #region action for Master Data
    #region action for Country
    public const string MasterDataAddCountry = "AddCountry";
    public const string MasterDataDeleteCountry = "DeleteCountry";
    public const string MasterDataGetCountries = "Countries";
    public const string MasterDataGetCountry = "GetCountry";
    public const string MasterDataUpdateCountry = "UpdateCountry";
    #endregion
    #region action for Email Templates
    public const string MasterDataAddEmailTemplate = "AddEmailTemplate";
    public const string MasterDataDeleteEmailTemplate = "DeleteEmailTemplate";
    public const string MasterDataGetEmailTemplate = "GetEmailTemplate";
    public const string MasterDataGetEmailTemplates = "GetEmailTemplates";
    public const string MasterDataUpdateEmailTemplate = "UpdateEmailTemplate";
    #endregion
    #region action for Email Templates
    public const string MasterDataAddLender = "AddLender";
    public const string MasterDataDeleteLender = "DeleteLender";
    public const string MasterDataGetLender = "GetLender";
    public const string MasterDataGetLenders = "GetLenders";
    public const string MasterDataUpdateLender = "UpdateLender";
    public const string MasterDataAddLenderFriendlyName = "AddLender";
    public const string MasterDataDeleteLenderFriendlyName = "DeleteLender";
    public const string MasterDataGetLenderFriendlyName = "GetLender";
    public const string MasterDataGetLendersFriendlyName = "GetLenders";
    public const string MasterDataUpdateLenderFriendlyName = "UpdateLender";
    #endregion
    #region action for Users
    public const string MasterDataAddUser = "AddUser";
    public const string MasterDataCheckVerificationUser = "CheckVerificationUser";
    public const string MasterDataDeleteUser = "DeleteUser";
    public const string MasterDataSendMyVerificationCode = "SendVerificationUser";
    public const string MasterDataUpdateUser = "UpdateUser";
    public const string MasterDataUpdateUserPic = "UpdateUserPic";
    public const string MasterDataVerifiedUser = "VerifiedUser";
    #endregion
    #endregion

    // Helper to split PascalCase into words
    public static string ToFriendlyNameSplitByPascalCase(string action)
    {
        // Insert spaces before capital letters
        return System.Text.RegularExpressions.Regex.Replace(action, "([a-z])([A-Z])", "$1 $2");
    }

    public static string ToFriendlyName(string action)
    {
        // Insert spaces before capital letters
        return action;
    }
}
