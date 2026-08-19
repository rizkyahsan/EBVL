namespace EBVL.BackEnd.Domain.Entities;

public sealed class Project : ModifiableEntity
{
    public required string Title { get; set; }
    public required string Desc { get; set; }
    public required string Objective { get; set; }
    public required string FinanceType { get; set; }

    public required Guid StatusId { get; set; }

    public ICollection<ProjectLender> ProjectLenders { get; set; } = new HashSet<ProjectLender>();
    public ICollection<ProjectStage> ProjectStages { get; set; } = new HashSet<ProjectStage>();
    public ICollection<ProjectAttachment> ProjectAttachments { get; set; } = new HashSet<ProjectAttachment>();
    public ICollection<ProjectReq> ProjectReqs { get; set; } = new HashSet<ProjectReq>();
    public ICollection<ProjectLenderReq> ProjectLenderReqs { get; set; } = new HashSet<ProjectLenderReq>();
    public ICollection<ProjectLenderReqFile> ProjectLenderReqFiles { get; set; } = new HashSet<ProjectLenderReqFile>();
    public ICollection<ProjectLenderHistory> ProjectLenderHistories { get; set; } = new HashSet<ProjectLenderHistory>();
    public ICollection<ProjectFile> ProjectFiles { get; set; } = new HashSet<ProjectFile>();
    public ICollection<LogTransaction> LogTransactions { get; set; } = new HashSet<LogTransaction>();
}
