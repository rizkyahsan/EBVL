using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using EBVL.BackEnd.Logics.Common.Services.LogEmailDb;
using EBVL.BackEnd.Services.EmailBlast2;
using EBVL.BackEnd.Services.EmailBlast2.Model;
using EBVL.Shared.Statics.LogEmails;
using Pertamina.Services.BackgroundJob;
using Pertamina.Services.IdAMan;
using RestSharp;
using SendGrid.Helpers.Mail;

namespace EBVL.BackEnd.Infrastructure.Email;

public sealed class EmailBlast2EmailService : IEmailBlast2Service
{
    private sealed record SendEmailWithoutTemplateResponse
    {
        [JsonPropertyName("message")]
        public required string Message { get; init; }

        [JsonPropertyName("idRequest")]
        public required string RequestId { get; init; }

        [CompilerGenerated]
        [SetsRequiredMembers]
        private SendEmailWithoutTemplateResponse(SendEmailWithoutTemplateResponse original)
        {
            Message = original.Message;
            RequestId = original.RequestId;
        }

        public SendEmailWithoutTemplateResponse()
        {
        }
    }

    private sealed record IdAManAccessToken
    {
        [JsonPropertyName("access_token")]
        public required string AccessToken { get; set; }

        [JsonPropertyName("token_type")]
        public required string TokenType { get; set; }

        [JsonPropertyName("expires_in")]
        public required int ExpiresIn { get; set; }

        [CompilerGenerated]
        [SetsRequiredMembers]
        private IdAManAccessToken(IdAManAccessToken original)
        {
            AccessToken = original.AccessToken;
            TokenType = original.TokenType;
            ExpiresIn = original.ExpiresIn;
        }

        public IdAManAccessToken()
        {
        }
    }

    private readonly IdAManOptions _idAManOptions;

    private readonly EmailBlast2EmailOptions _emailBlast2EmailOptions;

    private readonly RestClient _restClient;

    private readonly RestClient _restClientToGetAccessToken;

    private readonly IBackgroundJobService _backgroundJobService;

    private readonly ILogEmailDbService _logEmailDbService;

    private readonly ILogger<EmailBlast2EmailService> _logger;

    public EmailBlast2EmailService(IOptions<IdAManOptions> idAManOptions
        , IOptions<EmailBlast2EmailOptions> emailBlast2EmailOptions
        , IBackgroundJobService backgroundJobService
        , ILogEmailDbService logEmailDbService
        , ILogger<EmailBlast2EmailService> logger, HttpClient httpClient)
    {
        _idAManOptions = idAManOptions.Value;
        _emailBlast2EmailOptions = emailBlast2EmailOptions.Value;
        _backgroundJobService = backgroundJobService;
        _logEmailDbService = logEmailDbService;
        _logger = logger;
        httpClient.BaseAddress = new Uri(_emailBlast2EmailOptions.ApiBaseUrl);
        _restClient = new RestClient(httpClient);
        _restClientToGetAccessToken = new RestClient(_idAManOptions.Authentication.AuthorityUrl);
    }

    public void SendEmails(SendEmailInput2 input)
    {
        if (input.EmailWith == EmailTemplatesEmailWith.EmailBlast)
        {
            _ = _backgroundJobService.RunImmediateJob(() => RealSendEmailAsync(input));
        }
        else if (input.EmailWith == EmailTemplatesEmailWith.TwilioSendGrid)
        {
            _ = _backgroundJobService.RunImmediateJob(() => RealSendEmailUsingTwilioSendGrid(input));
        }
    }

    public async Task RealSendEmailAsync(SendEmailInput2 input)
    {
        if (!input.Tos.Any())
        {
            try
            {
                //Write Log to database
                await _logEmailDbService.LogAsync(input, provider: EmailWith.EmailBlast
                    , success: false, message: "At least one 'To' email address must be provided.");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Unable to persist email log.");
            }

            throw new InvalidOperationException("At least one 'To' email address must be provided.");
        }

        var idAManAccessToken = await GetAccessTokenAsync();
        var toAddresses = input.Tos.Select(t => t.Address).ToList();
        var toJoinAddresses = string.Join(", ", input.Tos.Select(t => t.Address));
        var restRequest = new RestRequest(_emailBlast2EmailOptions.ResourceEndpoint.SendEmailNoTemplate, Method.Post)
        {
            AlwaysMultipartFormData = true
        };

        _ = restRequest.AddHeader("Authorization", idAManAccessToken.TokenType + " " + idAManAccessToken.AccessToken);

        if (input.Tos.Count > 0)
        {
            foreach (var t in input.Tos)
            {
                _ = restRequest.AddParameter("To", t.Address);
            }
        }

        if (input.Ccs.Count > 0)
        {
            foreach (var c in input.Ccs)
            {
                _ = restRequest.AddParameter("Cc", c.Address);
            }
        }

        if (input.Bccs.Count > 0)
        {
            foreach (var b in input.Bccs)
            {
                _ = restRequest.AddParameter("Bcc", b.Address);
            }
        }

        _ = restRequest.AddParameter("Subject", input.Subject);
        _ = restRequest.AddParameter("Body", input.Body);

        var restResponse = await _restClient.ExecuteAsync<SendEmailWithoutTemplateResponse>(restRequest);
        if (!restResponse.IsSuccessful)
        {
            _logger.LogInformation(
                "Unable to send e-mail using {EmailProvider} ({RestRequest}). Error message: {Message}",
                "EmailBlast2",
                _restClient.BuildUri(restRequest),
                restResponse.ErrorMessage);

            try
            {
                //Write Log to database
                await _logEmailDbService.LogAsync(input, provider: EmailWith.EmailBlast
                    , success: false, message: restResponse.ErrorMessage
                    , externalMessageId: restResponse.Data?.RequestId);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Unable to persist email log.");
            }

            throw new InvalidOperationException(
                $"Unable to send e-mail using EmailBlast2 ({_restClient.BuildUri(restRequest)}). " +
                $"Error message: {restResponse.ErrorMessage}");
        }

        if (restResponse.Data is not null && _logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation(
                "Successfully sent email to {ToAddresses} using {EmailProvider} with Request ID {RequestId} and message: {Message}.",
                toAddresses,
                "Pertamina EmailBlast2",
                restResponse.Data.RequestId,
                restResponse.Data.Message);

            try
            {
                //Write Log to database
                await _logEmailDbService.LogAsync(input, provider: EmailWith.EmailBlast
                    , success: true, message: restResponse.Data?.Message
                    , externalMessageId: restResponse.Data?.RequestId);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Unable to persist email log.");
            }
        }
    }

    public async Task RealSendEmailUsingTwilioSendGrid(SendEmailInput2 input)
    {
        if (!input.Tos.Any())
        {
            try
            {
                //Write Log to database
                await _logEmailDbService.LogAsync(input, provider: EmailWith.TwilioSendGrid
                    , success: false, message: "At least one 'To' email address must be provided.");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Unable to persist email log.");
            }

            throw new InvalidOperationException("At least one 'To' email address must be provided.");
        }

        var toAddresses = input.Tos.Select(t => t.Address).ToList();
        var toJoinAddresses = string.Join(", ", input.Tos.Select(t => t.Address));
        var client = new SendGrid.SendGridClient(_emailBlast2EmailOptions.SendGridApiKey);
        var from = new EmailAddress(input.DefaultFrom, input.DefaultFrom);
        var tos = input
            .Tos.Select(t => new EmailAddress(t.Address.Trim().ToLowerInvariant()))
            .Distinct().ToList();

        var msg = MailHelper.CreateSingleEmailToMultipleRecipients(
            from,
            tos,
            input.Subject,
            input.Body,
            $"<p>{input.Body}</p>"
        );

        // Track every email already added
        var recipients =
            new HashSet<string>(input
            .Tos.Select(x => x.Address.Trim().ToLowerInvariant())
            .Distinct());

        // Add CCs (skip duplicates)
        foreach (var cc in input.Ccs)
        {
            if (string.IsNullOrWhiteSpace(cc.Address))
            {
                continue;
            }

            var email = cc.Address.Trim().ToLowerInvariant();

            if (recipients.Add(email))
            {
                msg.AddCc(new EmailAddress(cc.Address, cc.Name));
            }
        }

        // Add BCCs (skip duplicates)
        foreach (var bcc in input.Bccs)
        {
            if (string.IsNullOrWhiteSpace(bcc.Address))
            {
                continue;
            }

            var email = bcc.Address.Trim().ToLowerInvariant();

            if (recipients.Add(email))
            {
                msg.AddBcc(new EmailAddress(bcc.Address, bcc.Name));
            }
        }

        // Attachment
        foreach (var attachment in input.Attachments)
        {
            if (attachment.Content == null || attachment.Content.Length == 0)
            {
                continue;
            }

            msg.AddAttachment(
                attachment.FileName,
                Convert.ToBase64String(attachment.Content),
                attachment.ContentType);
        }

        try
        {
            var response = await client.SendEmailAsync(msg);

            if (response.StatusCode is System.Net.HttpStatusCode.Accepted or System.Net.HttpStatusCode.OK)
            {
                _logger.LogInformation("Fallback via SendGrid succeeded. Email sent to {ToAddresses}.", toAddresses);

                try
                {
                    //Write Log to database
                    await _logEmailDbService.LogAsync(input, provider: EmailWith.TwilioSendGrid
                        , success: true, message: $"Fallback via SendGrid succeeded. Email sent with status code {response.StatusCode}");
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Unable to persist email log.");
                }
            }
            else
            {
                _logger.LogError("Fallback via SendGrid failed. StatusCode: {StatusCode}", response.StatusCode);

                try
                {
                    //Write Log to database
                    await _logEmailDbService.LogAsync(input, provider: EmailWith.TwilioSendGrid
                        , success: false, message: $"SendGrid fallback failed with status code {response.StatusCode}");
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Unable to persist email log.");
                }

                throw new InvalidOperationException($"SendGrid fallback failed with status {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while sending email via SendGrid fallback.");

            try
            {
                //Write Log to database
                await _logEmailDbService.LogAsync(input, provider: EmailWith.TwilioSendGrid
                    , success: false, message: $"Exception occurred while sending email via SendGrid fallback with error: {ex.Message}");
            }
            catch (Exception error)
            {
                _logger.LogWarning(error, "Unable to persist email log.");
            }

            throw;
        }
    }

    private async Task<IdAManAccessToken> GetAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        var request = new RestRequest(_emailBlast2EmailOptions.TokenEndpoint, Method.Post);
        _ = request.AddParameter("grant_type", "client_credentials");
        _ = request.AddParameter("client_id", _emailBlast2EmailOptions.ClientId);
        _ = request.AddParameter("client_secret", _emailBlast2EmailOptions.ClientSecret);
        _ = request.AddParameter("scope", _emailBlast2EmailOptions.Scope);

        return (await _restClientToGetAccessToken.ExecuteAsync<IdAManAccessToken>(request, cancellationToken)).Data ?? throw new InvalidOperationException("Failed to deserialize JSON Content into IdAManAccessToken.");
    }
}
