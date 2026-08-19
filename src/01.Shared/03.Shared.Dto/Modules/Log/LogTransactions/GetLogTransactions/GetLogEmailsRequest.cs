namespace EBVL.Shared.Dto.Modules.Log.LogTransactions.GetLogTransactions;

public record GetLogTransactionsRequest
{
    public required Guid ProjectId { get; set; }

    public Guid? ProjectStageId { get; set; }

    public Guid? ProjectLenderId { get; set; }
}
