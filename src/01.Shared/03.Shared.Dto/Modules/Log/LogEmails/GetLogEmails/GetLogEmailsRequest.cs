namespace EBVL.Shared.Dto.Modules.Log.LogEmails.GetLogEmails;

public record GetLogEmailsRequest
{
    public DateTimeOffset? StartDatetime { get; set; }

    public DateTimeOffset? EndDatetime { get; set; }
}
