using EBVL.BackEnd.Services.EmailBlast2.Model;
using EBVL.Shared.Statics.Common;

namespace EBVL.BackEnd.Logics.Common.Services.LogEmailDb;

public sealed class LogEmailDbService(IDatabaseService databaseService) : ILogEmailDbService
{
    public async Task LogAsync(SendEmailInput2 input, string provider, bool success, string? message = null,
        string? externalMessageId = null, CancellationToken cancellationToken = default)
    {
        var sentAt = TimeZoneInfo.ConvertTime(DateTimeOffset.Now, TimezoneFor.WibTimeZone);

        var entity = new LogEmail
        {
            Module = input.Module,
            Action = input.Action,
            Provider = provider,
            To = string.Join(';', input.Tos.Select(x => x.Address)),
            Cc = string.Join(';', input.Ccs.Select(x => x.Address)),
            Subject = input.Subject,
            Content = input.Body,
            SentAt = success ? sentAt : null,
            IsSuccessful = success,
            Message = message,
            ExternalMessageId = externalMessageId
        };

        _ = await databaseService.LogEmails.AddAsync(entity, cancellationToken);
        _ = await databaseService.SaveAsync(nameof(LogEmailDbService), cancellationToken);
    }
}
