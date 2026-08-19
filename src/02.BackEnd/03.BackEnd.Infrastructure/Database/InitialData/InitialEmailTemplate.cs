using System.Text;

namespace EBVL.BackEnd.Infrastructure.Database.InitialData;

public static class InitialEmailTemplate
{
    public static string BuildSendMyVerificationEmailBody()
    {
        var body = new StringBuilder();
        _ = body.AppendLine($"<p>Dear {{DisplayName}},</p>");
        _ = body.AppendLine("<p>Your verification code is:</p>");
        _ = body.AppendLine($"<div style=\"margin: 12px 0px; padding: 12px; border: solid 1px black; font-size: 36px; font-weight: bold;\">{{VerificationCode}}</div>");
        _ = body.AppendLine("<p>If you did not request this code, please ignore this email.</p>");
        _ = body.AppendLine($"<p>Thank you.</p>");
        _ = body.AppendLine("<br />");
        _ = body.AppendLine($"<p>Best regards,</p>");
        _ = body.AppendLine($"<p>{{FrontendLink}}</p>");
        _ = body.AppendLine($"This is an automated email. Please do not reply to this message.");

        return body.ToString();
    }

    public static string BuildSendVerificationEmailBody()
    {
        var body = new StringBuilder();
        _ = body.AppendLine($"<p>Dear {{DisplayName}},</p>");
        _ = body.AppendLine("<p>Your access registration request has been successfully submitted and is currently under review.</p>");
        _ = body.AppendLine($"<p>Request Details:");
        _ = body.AppendLine($"<p>•\tUsername: <b>{{Username}}</b></p>");
        _ = body.AppendLine($"<p>•\tLender: <b>{{LenderName}}</b></p>");
        _ = body.AppendLine($"<p>•\tSubmission Date: <b>{{RegistrationDate}}</b></p>");
        _ = body.AppendLine($"<p>•\tExpired Date: <b>{{ExpiredDate}}</b></p>");
        _ = body.AppendLine($"<p>•\tConfirmation Link: {{VerificationLink}}</p>");
        _ = body.AppendLine($"<p>If you have any questions or require assistance regarding your registration, please contact the system administrator at {{EmailGroup}}</p>");
        _ = body.AppendLine($"<p>Thank you.</p>");
        _ = body.AppendLine("<br />");
        _ = body.AppendLine($"<p>Best regards,</p>");
        _ = body.AppendLine($"<p>{{FrontendLink}}</p>");
        _ = body.AppendLine($"This is an automated email. Please do not reply to this message.");

        return body.ToString();
    }

    public static string BuildSendResetPasswordEmailBody()
    {
        var body = new StringBuilder();
        _ = body.AppendLine($"<p>Dear {{DisplayName}},</p>");
        _ = body.AppendLine("<p>This is email for reset password, please click link for continue.</p>");
        _ = body.AppendLine($"<p>Account Detail:");
        _ = body.AppendLine($"<p>•\tUsername: <b>{{Username}}</b></p>");
        _ = body.AppendLine($"<p>•\tLender: <b>{{LenderName}}</b></p>");
        _ = body.AppendLine($"<p>•\tRequest Date: <b>{{RequestDate}}</b></p>");
        _ = body.AppendLine($"<p>•\tExpired Date: <b>{{ExpiredDate}}</b></p>");
        _ = body.AppendLine($"<p>•\tReset Password Link: {{VerificationLink}}</p>");
        _ = body.AppendLine($"<p>If you have any questions or require assistance regarding your reset password, please contact the system administrator at {{EmailGroup}}</p>");
        _ = body.AppendLine($"<p>Thank you.</p>");
        _ = body.AppendLine("<br />");
        _ = body.AppendLine($"<p>Best regards,</p>");
        _ = body.AppendLine($"<p>{{FrontendLink}}</p>");
        _ = body.AppendLine($"This is an automated email. Please do not reply to this message.");

        return body.ToString();
    }

    public static string BuildSubject(string friendlyAction)
    {
        return string.Format("ebvl ・{0}", friendlyAction);
    }

    public static string BuildBody(string friendlyAction)
    {
        if (friendlyAction == CommonActionFor.MainProjectsCreateProject)
        {
            var body = new StringBuilder();
            _ = body.AppendLine($"<p>Dear All,</p>");
            _ = body.AppendLine($"<p>We are pleased to announce the launch of <b>{{ProjectName}}</b>, a strategic initiative designed to support our business objectives and drive long-term growth.</p>");
            _ = body.AppendLine($"<p>Project Overview:</p>");
            _ = body.AppendLine($"<p>•\tProject Name: <b>{{ProjectName}}</b></p>");
            _ = body.AppendLine($"<p>•\tProject Objectives: <b>{{Objectives}}</b></p>");
            _ = body.AppendLine($"<p>•\tType Of Financing: <b>{{FinancingType}}</b></p>");
            _ = body.AppendLine("<br />");
            _ = body.AppendLine($"<p>We appreciate your commitment and support in ensuring the successful delivery of this project.</p>");
            _ = body.AppendLine($"<p>Should you have any questions, please contact the system administrator at {{EmailGroup}}</p>");
            _ = body.AppendLine($"<p>Thank you for your support.</p>");
            _ = body.AppendLine("<br />");
            _ = body.AppendLine($"<p>Best regards,</p>");
            _ = body.AppendLine($"<p>{{FrontendLink}}</p>");
            _ = body.AppendLine($"This is an automated email. Please do not reply to this message.");

            return body.ToString();
        }
        else if (friendlyAction == CommonActionFor.MainProjectsPublishProjectStageAdmin)
        {
            var body = new StringBuilder();
            _ = body.AppendLine($"<p>Dear {{AdminName}},</p>");
            _ = body.AppendLine($"<p><b>{{ProjectName}} - {{ProjectStageName}}</b> already published to all participate lender</p>");
            _ = body.AppendLine($"<p>through the application no later than <b>{{DueDate}}</b>.</p>");
            _ = body.AppendLine($"<p>Documents Submission:</p>");
            _ = body.AppendLine($"{{ListReqProject}}");
            _ = body.AppendLine("<br />");
            _ = body.AppendLine($"<p>Thank you for your cooperation.</p>");
            _ = body.AppendLine("<br />");
            _ = body.AppendLine($"<p>Best regards,</p>");
            _ = body.AppendLine($"<p>{{FrontendLink}}</p>");
            _ = body.AppendLine($"This is an automated email. Please do not reply to this message.");

            return body.ToString();
        }
        else if (friendlyAction == CommonActionFor.MainProjectsPublishProjectStage)
        {
            var body = new StringBuilder();
            _ = body.AppendLine($"<p>Dear {{LenderName}},</p>");
            _ = body.AppendLine($"<p>Please submit the required document(s) for <b>{{ProjectName}} - {{ProjectStageName}}</b></p>");
            _ = body.AppendLine($"<p>through the application no later than <b>{{DueDate}}</b>.</p>");
            _ = body.AppendLine($"<p>Documents Submission:</p>");
            _ = body.AppendLine($"{{ListReqProject}}");
            _ = body.AppendLine("<br />");
            _ = body.AppendLine($"<p>Should you have any questions, please contact the system administrator at {{EmailGroup}}</p>");
            _ = body.AppendLine($"<p>Thank you for your cooperation.</p>");
            _ = body.AppendLine("<br />");
            _ = body.AppendLine($"<p>Best regards,</p>");
            _ = body.AppendLine($"<p>{{FrontendLink}}</p>");
            _ = body.AppendLine($"This is an automated email. Please do not reply to this message.");

            return body.ToString();
        }
        else if (friendlyAction == CommonActionFor.MainMyProjectsUpdateMyProjectStage)
        {
            var body = new StringBuilder();
            _ = body.AppendLine($"<p>Dear {{AdminName}},</p>");
            _ = body.AppendLine($"<br />");
            _ = body.AppendLine($"<p>Please review this submitted requirement <b>{{ProjectName}} - {{ProjectStageName}}</b>.</p>");
            _ = body.AppendLine($"<br />");
            _ = body.AppendLine($"<p>•\tLender: <b>{{LenderName}}</b>.</p>");
            _ = body.AppendLine($"<p>•\tNote: {{Note}}</p>");
            _ = body.AppendLine($"<p>Thank you for your cooperation.</p>");
            _ = body.AppendLine($"<br />");
            _ = body.AppendLine($"<p>Best regards,</p>");
            _ = body.AppendLine($"<p>{{FrontendLink}}</p>");
            _ = body.AppendLine($"This is an automated email. Please do not reply to this message.");

            return body.ToString();
        }
        else if (friendlyAction == CommonActionFor.MainProjectsRevisionProjectLenderReq)
        {
            var body = new StringBuilder();
            _ = body.AppendLine($"<p>Dear {{LenderName}},</p>");
            _ = body.AppendLine($"<p>Thank you for submitting the required documents for <b>{{ProjectName}} - {{ProjectStageName}}</b>.</p>");
            _ = body.AppendLine($"<p>Following our review, we have identified items that require revision before the review process can proceed further.</p>");
            _ = body.AppendLine($"<br />");
            _ = body.AppendLine($"<p>•\tReview Status: <b>Revision Required</b></p>");
            _ = body.AppendLine($"<p>•\tNote: {{Note}}</p>");
            _ = body.AppendLine($"<p>Please log in to the application, review the comments provided by the reviewer, and upload the revised documents by <b>{{DueDate}}</b>.</p>");
            _ = body.AppendLine($"<p>Please note that the review process will resume once the revised documents have been successfully submitted.</p>");
            _ = body.AppendLine($"<p>If you have any questions regarding the requested revisions, please contact the review team or system administrator at {{EmailGroup}}.</p>");
            _ = body.AppendLine($"<p>Thank you for your cooperation.</p>");
            _ = body.AppendLine($"<br />");
            _ = body.AppendLine($"<p>Best regards,</p>");
            _ = body.AppendLine($"<p>{{FrontendLink}}</p>");
            _ = body.AppendLine($"This is an automated email. Please do not reply to this message.");

            return body.ToString();
        }
        else if (friendlyAction == CommonActionFor.MainProjectsReviewProjectStage)
        {
            var body = new StringBuilder();
            _ = body.AppendLine($"<p>Dear {{LenderName}},</p>");
            _ = body.AppendLine($"<p>Thank you for submitting the required loan documents for <b>{{ProjectName}} - {{ProjectStageName}}</b>.</p>");
            _ = body.AppendLine($"<p>We would like to inform you that your submitted documents have been successfully received and are currently under review by the relevant team.</p>");
            _ = body.AppendLine($"<br />");
            _ = body.AppendLine($"<p>Submission Details</p>");
            _ = body.AppendLine($"<p>•\tStage : <b>{{ProjectStageName}}</b></p>");
            _ = body.AppendLine($"<p>•\tCurrent Status: <b>Under Review</b></p>");
            _ = body.AppendLine($"<p>Our review team is currently assessing the submitted documents to ensure completeness and compliance with the applicable requirements.</p>");
            _ = body.AppendLine($"<p>You will receive a follow-up notification once the review process has been completed. If additional information or supporting documents are required, we will contact you accordingly.</p>");
            _ = body.AppendLine($"<p>No further action is required from you at this stage.</p>");
            _ = body.AppendLine($"<p>Thank you for your patience and cooperation.</p>");
            _ = body.AppendLine($"<br />");
            _ = body.AppendLine($"<p>Best regards,</p>");
            _ = body.AppendLine($"<p>{{FrontendLink}}</p>");
            _ = body.AppendLine($"This is an automated email. Please do not reply to this message.");

            return body.ToString();
        }
        else if (friendlyAction == CommonActionFor.MainProjectsCompleteProjectStage)
        {
            var body = new StringBuilder();
            _ = body.AppendLine($"<p>Dear {{AdminName}},</p>");
            _ = body.AppendLine($"<p><b>{{ProjectName}} - {{ProjectStageName}}</b> already complete,</p>");
            _ = body.AppendLine($"<p>please continue to next process this project.</p>");
            _ = body.AppendLine($"<br />");
            _ = body.AppendLine($"<p>Thank you for your cooperation.</p>");
            _ = body.AppendLine($"<br />");
            _ = body.AppendLine($"<p>Best regards,</p>");
            _ = body.AppendLine($"<p>{{FrontendLink}}</p>");
            _ = body.AppendLine($"This is an automated email. Please do not reply to this message.");

            return body.ToString();
        }
        else if (friendlyAction == CommonActionFor.MainProjectsCompleteProjectStageAccept)
        {
            var body = new StringBuilder();
            _ = body.AppendLine($"<p>Dear {{LenderName}},</p>");
            _ = body.AppendLine($"<p>We are pleased to inform you that your submission for <b>{{ProjectName}}</b></p>");
            _ = body.AppendLine($"<p>has accepted at the <b>{{ProjectStageName}}</b> review.</p>");
            _ = body.AppendLine($"<br />");
            _ = body.AppendLine($"<p>You will receive further notifications regarding the next step.</p>");
            _ = body.AppendLine($"<p>Thank you for your cooperation throughout the process.</p>");
            _ = body.AppendLine($"<br />");
            _ = body.AppendLine($"<p>Best regards,</p>");
            _ = body.AppendLine($"<p>{{FrontendLink}}</p>");
            _ = body.AppendLine($"This is an automated email. Please do not reply to this message.");

            return body.ToString();
        }
        else if (friendlyAction == CommonActionFor.MainProjectsCompleteProjectStageReject)
        {
            var body = new StringBuilder();
            _ = body.AppendLine($"<p>Dear {{LenderName}},</p>");
            _ = body.AppendLine($"<p>We regret to inform you that your submission for <b>{{ProjectName}}</b></p>");
            _ = body.AppendLine($"<p>has rejected at the <b>{{ProjectStageName}}</b> review.</p>");
            _ = body.AppendLine($"<br />");
            _ = body.AppendLine($"<p>You will receive further notifications regarding the next step.</p>");
            _ = body.AppendLine($"<p>Thank you for your cooperation throughout the process.</p>");
            _ = body.AppendLine($"<br />");
            _ = body.AppendLine($"<p>Best regards,</p>");
            _ = body.AppendLine($"<p>{{FrontendLink}}</p>");
            _ = body.AppendLine($"This is an automated email. Please do not reply to this message.");

            return body.ToString();
        }
        else if (friendlyAction == CommonActionFor.MainProjectsCompleteProjectLose)
        {
            var body = new StringBuilder();
            _ = body.AppendLine($"<p>Dear {{LenderName}},</p>");
            _ = body.AppendLine($"<br />");
            _ = body.AppendLine($"<p>We appreciate your participation. However, we regret to inform you that you was not selected for financing <b>{{ProjectName}}</b></p>");
            _ = body.AppendLine($"<p>Please find the attached file for your reference.</p>");
            _ = body.AppendLine($"<br />");
            _ = body.AppendLine($"<p>Best regards,</p>");
            _ = body.AppendLine($"<p>{{FrontendLink}}</p>");
            _ = body.AppendLine($"This is an automated email. Please do not reply to this message.");

            return body.ToString();
        }
        else if (friendlyAction == CommonActionFor.MainProjectsCompleteProjectWin)
        {
            var body = new StringBuilder();
            _ = body.AppendLine($"<p>Dear {{LenderName}},</p>");
            _ = body.AppendLine($"<br />");
            _ = body.AppendLine($"<p>Congratulations!</p>");
            _ = body.AppendLine($"<p>We're pleased to inform you that you have been successfully selected for financing <b>{{ProjectName}}</b></p>");
            _ = body.AppendLine($"<p>Please find the attached file for your reference.</p>");
            _ = body.AppendLine($"<br />");
            _ = body.AppendLine($"<p>Best regards,</p>");
            _ = body.AppendLine($"<p>{{FrontendLink}}</p>");
            _ = body.AppendLine($"This is an automated email. Please do not reply to this message.");

            return body.ToString();
        }
        else if (friendlyAction == CommonActionFor.MainProjectsCompleteProject)
        {
            var body = new StringBuilder();
            _ = body.AppendLine($"<p>Dear {{AdminName}},</p>");
            _ = body.AppendLine($"<br />");
            _ = body.AppendLine($"<p><b>{{ProjectName}}</b> already complete,</p>");
            _ = body.AppendLine($"<p>For more details, you can check on Archive Project.</p>");
            _ = body.AppendLine($"<br />");
            _ = body.AppendLine($"<p>Best regards,</p>");
            _ = body.AppendLine($"<p>{{FrontendLink}}</p>");
            _ = body.AppendLine($"This is an automated email. Please do not reply to this message.");

            return body.ToString();
        }
        else
        {
            return "Comming Soon Email Template";
        }
    }

    public static readonly EmailTemplate SendOtp = new()
    {
        Id = new Guid("d00620de-4fec-4f0c-a0e9-22eedd315ecd"),
        Module = CommonModuleFor.ExternalUsers,
        Action = CommonActionFor.SendOtp,
        Subject = BuildSubject($"Verification Code"),
        Content = BuildSendMyVerificationEmailBody(),
        Created = InitialValueFor.Created,
        CreatedBy = InitialValueFor.CreatedBy
    };

    public static readonly EmailTemplate SendVerificationCode = new()
    {
        Id = new Guid("b6fd1047-731b-4eea-8e85-34bbbf414230"),
        Module = CommonModuleFor.ExternalUsers,
        Action = CommonActionFor.SendVerificationCode,
        Subject = BuildSubject($"Registration"),
        Content = BuildSendVerificationEmailBody(),
        Created = InitialValueFor.Created,
        CreatedBy = InitialValueFor.CreatedBy
    };

    public static readonly EmailTemplate SendResetPassword = new()
    {
        Id = new Guid("23efa61c-ecbb-4aa7-ba84-3ebcf0077016"),
        Module = CommonModuleFor.ExternalUsers,
        Action = CommonActionFor.SendResetPassword,
        Subject = BuildSubject($"Reset Password"),
        Content = BuildSendResetPasswordEmailBody(),
        Created = InitialValueFor.Created,
        CreatedBy = InitialValueFor.CreatedBy
    };

    public static readonly EmailTemplate MainMyProjectsUpdateMyProjectStage = new()
    {
        Id = new Guid("89756f43-9b1f-4580-957c-ad485b060b14"),
        Module = CommonModuleFor.MyProjects,
        Action = CommonActionFor.MainMyProjectsUpdateMyProjectStage,
        Subject = BuildSubject($"Submitted Document"),
        Content = BuildBody(CommonActionFor.MainMyProjectsUpdateMyProjectStage),
        Created = InitialValueFor.Created,
        CreatedBy = InitialValueFor.CreatedBy
    };

    public static readonly EmailTemplate MainProjectsCreateProject = new()
    {
        Id = new Guid("b3df4129-03d6-4fa9-866b-5ab2428a1089"),
        Module = CommonModuleFor.Projects,
        Action = CommonActionFor.MainProjectsCreateProject,
        Subject = BuildSubject($"Project Announcement"),
        Content = BuildBody(CommonActionFor.MainProjectsCreateProject),
        Created = InitialValueFor.Created,
        CreatedBy = InitialValueFor.CreatedBy
    };

    public static readonly EmailTemplate MainProjectsPublishProjectStage = new()
    {
        Id = new Guid("c4849d62-fa50-49f8-bc28-bb0ed764a93d"),
        Module = CommonModuleFor.Projects,
        Action = CommonActionFor.MainProjectsPublishProjectStage,
        Subject = BuildSubject($"Document Submission"),
        Content = BuildBody(CommonActionFor.MainProjectsPublishProjectStage),
        Created = InitialValueFor.Created,
        CreatedBy = InitialValueFor.CreatedBy
    };

    public static readonly EmailTemplate MainProjectsPublishProjectStageAdmin = new()
    {
        Id = new Guid("dec18254-6dda-4097-b36b-1444cf7db760"),
        Module = CommonModuleFor.Projects,
        Action = CommonActionFor.MainProjectsPublishProjectStageAdmin,
        Subject = BuildSubject($"Project Stage Published"),
        Content = BuildBody(CommonActionFor.MainProjectsPublishProjectStageAdmin),
        Created = InitialValueFor.Created,
        CreatedBy = InitialValueFor.CreatedBy
    };

    public static readonly EmailTemplate MainProjectsReviewProjectStage = new()
    {
        Id = new Guid("8892fe26-987c-4776-af3d-f6897fbc56e6"),
        Module = CommonModuleFor.Projects,
        Action = CommonActionFor.MainProjectsReviewProjectStage,
        Subject = BuildSubject($"Progress Review"),
        Content = BuildBody(CommonActionFor.MainProjectsReviewProjectStage),
        Created = InitialValueFor.Created,
        CreatedBy = InitialValueFor.CreatedBy
    };

    public static readonly EmailTemplate MainProjectsRevisionProjectLenderReq = new()
    {
        Id = new Guid("cd24c34c-07ff-48bb-bbcc-0c292a98c679"),
        Module = CommonModuleFor.Projects,
        Action = CommonActionFor.MainProjectsRevisionProjectLenderReq,
        Subject = BuildSubject($"Document Revision Required"),
        Content = BuildBody(CommonActionFor.MainProjectsRevisionProjectLenderReq),
        Created = InitialValueFor.Created,
        CreatedBy = InitialValueFor.CreatedBy
    };

    public static readonly EmailTemplate MainProjectsCompleteProjectStage = new()
    {
        Id = new Guid("bcba7d81-0ebf-4a97-be42-dd8359a70c3a"),
        Module = CommonModuleFor.Projects,
        Action = CommonActionFor.MainProjectsCompleteProjectStage,
        Subject = BuildSubject($"Project Stage Has Complete"),
        Content = BuildBody(CommonActionFor.MainProjectsCompleteProjectStage),
        Created = InitialValueFor.Created,
        CreatedBy = InitialValueFor.CreatedBy
    };

    public static readonly EmailTemplate MainProjectsCompleteProjectStageAccept = new()
    {
        Id = new Guid("01ff4694-ac9c-479d-ab70-96d17fdd0174"),
        Module = CommonModuleFor.Projects,
        Action = CommonActionFor.MainProjectsCompleteProjectStageAccept,
        Subject = BuildSubject($"Project Stage Submission Has Accepted"),
        Content = BuildBody(CommonActionFor.MainProjectsCompleteProjectStageAccept),
        Created = InitialValueFor.Created,
        CreatedBy = InitialValueFor.CreatedBy
    };

    public static readonly EmailTemplate MainProjectsCompleteProjectStageReject = new()
    {
        Id = new Guid("f46d2124-c853-47fe-8b62-e4dbfc32cc7c"),
        Module = CommonModuleFor.Projects,
        Action = CommonActionFor.MainProjectsCompleteProjectStageReject,
        Subject = BuildSubject($"Project Stage Submission Has Rejected"),
        Content = BuildBody(CommonActionFor.MainProjectsCompleteProjectStageReject),
        Created = InitialValueFor.Created,
        CreatedBy = InitialValueFor.CreatedBy
    };

    public static readonly EmailTemplate MainProjectsCompleteProjectWin = new()
    {
        Id = new Guid("2cad9a36-794f-41e5-8ebc-f18f4c7945da"),
        Module = CommonModuleFor.Projects,
        Action = CommonActionFor.MainProjectsCompleteProjectWin,
        Subject = BuildSubject($"Notification of Mandate Letter"),
        Content = BuildBody(CommonActionFor.MainProjectsCompleteProjectWin),
        Created = InitialValueFor.Created,
        CreatedBy = InitialValueFor.CreatedBy
    };

    public static readonly EmailTemplate MainProjectsCompleteProjectLose = new()
    {
        Id = new Guid("f0a5556c-60a3-420f-830f-61e67cc03492"),
        Module = CommonModuleFor.Projects,
        Action = CommonActionFor.MainProjectsCompleteProjectLose,
        Subject = BuildSubject($"Notification of Mandate Letter"),
        Content = BuildBody(CommonActionFor.MainProjectsCompleteProjectLose),
        Created = InitialValueFor.Created,
        CreatedBy = InitialValueFor.CreatedBy
    };

    public static readonly EmailTemplate MainProjectsCompleteProject = new()
    {
        Id = new Guid("ab7ee422-c568-4e16-99e7-1fb5a47e8a37"),
        Module = CommonModuleFor.Projects,
        Action = CommonActionFor.MainProjectsCompleteProject,
        Subject = BuildSubject($"Notification of Project Complete"),
        Content = BuildBody(CommonActionFor.MainProjectsCompleteProject),
        Created = InitialValueFor.Created,
        CreatedBy = InitialValueFor.CreatedBy
    };

    public static readonly EmailTemplate[] All =
    [
        SendOtp,
        SendVerificationCode,
        SendResetPassword,

        MainMyProjectsUpdateMyProjectStage,

        MainProjectsCreateProject,

        MainProjectsPublishProjectStage,
        MainProjectsPublishProjectStageAdmin,

        MainProjectsReviewProjectStage,
        MainProjectsRevisionProjectLenderReq,

        MainProjectsCompleteProjectStage,
        MainProjectsCompleteProjectStageAccept,
        MainProjectsCompleteProjectStageReject,

        MainProjectsCompleteProject,
        MainProjectsCompleteProjectWin,
        MainProjectsCompleteProjectLose,
    ];
}
