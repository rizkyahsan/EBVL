namespace EBVL.Shared.Dto.Modules.Main;

public static class Permissions
{
    public const string ClaimsRead = "ebvl.cr";
    public const string MainPage = "ebvl.mp";
    public const string MainPageMyProfile = "ebvl.mp.po";
    public const string BrandRegistrationCreate = "ebvl.brand-registration.create";
    public const string BrandRegistrationSelf = "ebvl.brand-registration.self";
    public const string BrandRegistrationSubmit = "ebvl.brand-registration.submit";
    public const string BrandRegistrationIndex = "ebvl.brand-registration.index";
    public const string BrandRegistrationView = "ebvl.brand-registration.view";
    public const string BrandRegistrationReview = "ebvl.brand-registration.review";
    public const string BrandRegistrationDisposition = "ebvl.brand-registration.disposition";
    public const string BrandRegistrationApprove = "ebvl.brand-registration.approve";
    public const string BrandRegistrationReject = "ebvl.brand-registration.reject";
    public const string BrandRegistrationRevise = "ebvl.brand-registration.revise";
    public const string BrandRegistrationObject = "ebvl.brand-registration.object";
    public const string BrandRegistrationInvite = "ebvl.brand-registration.invite";
    public const string BrandRegistrationRespondInvitation = "ebvl.brand-registration.invitation-response";
    public const string BrandRegistrationEvaluate = "ebvl.brand-registration.evaluate";
    public const string BrandRegistrationPresentationReview = "ebvl.brand-registration.presentation-review";
    public const string BrandRegistrationClose = "ebvl.brand-registration.close";
    public const string BrandRegistrationListPolicy = "BrandRegistrationList";
    public const string BrandRegistrationViewPolicy = "BrandRegistrationView";
    public const string BrandRegistrationTransition = "BrandRegistrationTransition";

    public static readonly string[] All =
    [
        ClaimsRead,
        MainPage,
        MainPageMyProfile,
        .. MasterData.Questionnaires.QuestionnairePermissions.All,
        BrandRegistrationCreate,
        BrandRegistrationSelf,
        BrandRegistrationSubmit,
        BrandRegistrationIndex,
        BrandRegistrationView,
        BrandRegistrationReview,
        BrandRegistrationDisposition,
        BrandRegistrationApprove,
        BrandRegistrationReject,
        BrandRegistrationRevise,
        BrandRegistrationObject,
        BrandRegistrationInvite,
        BrandRegistrationRespondInvitation,
        BrandRegistrationEvaluate,
        BrandRegistrationPresentationReview,
        BrandRegistrationClose
    ];
}
