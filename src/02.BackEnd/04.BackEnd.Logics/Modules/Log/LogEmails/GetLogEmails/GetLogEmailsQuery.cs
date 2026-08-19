using EBVL.Shared.Dto.Modules.Log.LogEmails.GetLogEmails;
using EBVL.Shared.Statics.Common;

namespace EBVL.BackEnd.Logics.Modules.Log.LogEmails.GetLogEmails;

[AuthorizeRequest]
public sealed record GetLogEmailsQuery : GetLogEmailsRequest, IRequest<GetLogEmailsResponse> { }

public sealed class GetLogEmailsQueryHandler(IDatabaseService databaseService)
    : IRequestHandler<GetLogEmailsQuery, GetLogEmailsResponse>
{
    public async Task<GetLogEmailsResponse> Handle(GetLogEmailsQuery request, CancellationToken cancellationToken)
    {
        var start = TimeZoneInfo.ConvertTime((DateTimeOffset)request.StartDatetime!, TimezoneFor.WibTimeZone);
        var end = TimeZoneInfo.ConvertTime((DateTimeOffset)request.EndDatetime!, TimezoneFor.WibTimeZone);

        var logEmail = await databaseService.LogEmails
            .AsNoTracking()
            .Where(x => x.Created >= start
                && x.Created <= end)
            .OrderByDescending(x => x.Created)
            .Select(x => new LogEmailItem
            {
                Id = x.Id,
                Module = x.Module,
                Action = x.Action,
                Provider = x.Provider,
                To = x.To,
                Cc = x.Cc,
                Subject = x.Subject,
                Content = x.Content,
                SentAt = x.SentAt,
                IsSuccessful = x.IsSuccessful,
                RetryCount = x.RetryCount,
                Message = x.Message,
                ExternalMessageId = x.ExternalMessageId,
                CorrelationId = x.CorrelationId
            })
            .ToListAsync(cancellationToken);

        return new GetLogEmailsResponse
        {
            Items = logEmail
        };
    }
}
