namespace EBVL.Shared.Dto.Modules.Log.LogEmails.GetLogEmails;

public sealed record LogEmailItem
{
    public required Guid Id { get; init; }
    public required string Module { get; set; }
    public required string Action { get; set; }
    public required string Provider { get; set; }
    public string To { get; set; } = string.Empty;
    public string Cc { get; set; } = string.Empty;
    public required string Subject { get; set; }
    public required string Content { get; set; }
    public DateTimeOffset? SentAt { get; set; }
    public bool IsSuccessful { get; set; }
    public int RetryCount { get; set; }
    public string? Message { get; set; }
    public string? ExternalMessageId { get; set; } // Provider message id
    public string? CorrelationId { get; set; } // Optional
}
