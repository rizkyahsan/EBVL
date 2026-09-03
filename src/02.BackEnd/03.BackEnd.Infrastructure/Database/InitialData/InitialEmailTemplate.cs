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

    public static readonly EmailTemplate[] All =
    [
        SendOtp,
        SendVerificationCode,
        SendResetPassword
    ];
}
