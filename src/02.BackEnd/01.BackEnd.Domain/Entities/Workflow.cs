namespace EBVL.BackEnd.Domain.Entities;

public sealed class WorkflowCase : ModifiableEntity
{
    public required string ProcessType { get; set; }
    public required string Status { get; set; }
    public DateTimeOffset StartedAt { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }
    public DateTimeOffset? DueAt { get; set; }
    public byte[] RowVersion { get; set; } = [];
    public ICollection<WorkflowAssignment> Assignments { get; set; } = new HashSet<WorkflowAssignment>();
    public ICollection<WorkflowTransition> Transitions { get; set; } = new HashSet<WorkflowTransition>();
}

public sealed class WorkflowAssignment : CreatableEntity
{
    public Guid WorkflowCaseId { get; set; }
    public required string Role { get; set; }
    public string? AssigneeUsername { get; set; }
    public DateTimeOffset AssignedAt { get; set; }
    public required string AssignedBy { get; set; }
    public DateTimeOffset? EndedAt { get; set; }
    public string? EndReason { get; set; }
    public WorkflowCase WorkflowCase { get; set; } = default!;
}

public sealed class WorkflowTransition : CreatableEntity
{
    public Guid WorkflowCaseId { get; set; }
    public required string FromStatus { get; set; }
    public required string ToStatus { get; set; }
    public required string Action { get; set; }
    public required string ActorUsername { get; set; }
    public required string ActorRole { get; set; }
    public string? Note { get; set; }
    public DateTimeOffset OccurredAt { get; set; }
    public WorkflowCase WorkflowCase { get; set; } = default!;
}
