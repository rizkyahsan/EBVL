namespace EBVL.BackEnd.Infrastructure.Database.Configurations;

public sealed class WorkflowCaseConfiguration : IEntityTypeConfiguration<WorkflowCase>
{
    public void Configure(EntityTypeBuilder<WorkflowCase> builder)
    {
        _ = builder.ToTable(nameof(IDatabaseService.WorkflowCases));
        builder.ConfigureModifiableProperties();
        _ = builder.Property(x => x.ProcessType).HasMaxLength(100);
        _ = builder.Property(x => x.Status).HasMaxLength(100);
        _ = builder.Property(x => x.RowVersion).IsRowVersion();
        _ = builder.HasIndex(x => new { x.ProcessType, x.Status });
        _ = builder.HasIndex(x => x.DueAt);
    }
}

public sealed class WorkflowAssignmentConfiguration : IEntityTypeConfiguration<WorkflowAssignment>
{
    public void Configure(EntityTypeBuilder<WorkflowAssignment> builder)
    {
        _ = builder.ToTable(nameof(IDatabaseService.WorkflowAssignments));
        builder.ConfigureCreatableProperties();
        _ = builder.Property(x => x.Role).HasMaxLength(100);
        _ = builder.Property(x => x.AssigneeUsername).HasMaxLength(320);
        _ = builder.Property(x => x.AssignedBy).HasMaxLength(320);
        _ = builder.Property(x => x.EndReason).HasMaxLength(500);
        _ = builder.HasIndex(x => x.WorkflowCaseId).IsUnique().HasFilter("[EndedAt] IS NULL");
        _ = builder.HasOne(x => x.WorkflowCase).WithMany(x => x.Assignments).HasForeignKey(x => x.WorkflowCaseId).OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class WorkflowTransitionConfiguration : IEntityTypeConfiguration<WorkflowTransition>
{
    public void Configure(EntityTypeBuilder<WorkflowTransition> builder)
    {
        _ = builder.ToTable(nameof(IDatabaseService.WorkflowTransitions));
        builder.ConfigureCreatableProperties();
        _ = builder.Property(x => x.FromStatus).HasMaxLength(100);
        _ = builder.Property(x => x.ToStatus).HasMaxLength(100);
        _ = builder.Property(x => x.Action).HasMaxLength(100);
        _ = builder.Property(x => x.ActorUsername).HasMaxLength(320);
        _ = builder.Property(x => x.ActorRole).HasMaxLength(100);
        _ = builder.Property(x => x.Note).HasMaxLength(2000);
        _ = builder.HasIndex(x => new { x.WorkflowCaseId, x.OccurredAt });
        _ = builder.HasOne(x => x.WorkflowCase).WithMany(x => x.Transitions).HasForeignKey(x => x.WorkflowCaseId).OnDelete(DeleteBehavior.Cascade);
    }
}
