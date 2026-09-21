namespace EBVL.BackEnd.Infrastructure.Database.Configurations;

public sealed class QuestionnaireConfiguration : IEntityTypeConfiguration<Questionnaire>
{
    public void Configure(EntityTypeBuilder<Questionnaire> builder)
    {
        _ = builder.ToTable(nameof(IDatabaseService.Questionnaires));
        builder.ConfigureModifiableProperties();
        _ = builder.Property(x => x.Code).HasColumnType(ColumnTypeFor.Nvarchar(50));
        _ = builder.Property(x => x.BusinessProcess).HasColumnType(ColumnTypeFor.Nvarchar(100));
        _ = builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
        _ = builder.Property(x => x.PublishedBy).HasMaxLength(200);
        _ = builder.Property(x => x.RowVersion).IsRowVersion();
        _ = builder.HasIndex(x => new { x.QuestionnaireSeriesId, x.Version }).IsUnique();
        _ = builder.HasIndex(x => x.Code, "IX_Questionnaires_OneSeriesPerCode").IsUnique().HasFilter("[Version] = 1 AND [IsDeleted] = 0");
        _ = builder.HasIndex(x => x.PreviousVersionId);
        _ = builder.HasIndex(x => x.QuestionnaireSeriesId, "IX_Questionnaires_OneDraftPerSeries").IsUnique().HasFilter("[Status] = 'Draft' AND [IsDeleted] = 0");
        _ = builder.HasIndex(x => x.QuestionnaireSeriesId, "IX_Questionnaires_OnePublishPerSeries").IsUnique().HasFilter("[Status] = 'Publish' AND [IsDeleted] = 0");
        _ = builder.HasOne(x => x.PreviousVersion).WithMany().HasForeignKey(x => x.PreviousVersionId).OnDelete(DeleteBehavior.Restrict);
        _ = builder.HasMany(x => x.Sections).WithOne(x => x.Questionnaire).HasForeignKey(x => x.QuestionnaireId).OnDelete(DeleteBehavior.Cascade);
    }
}
public sealed class QuestionnaireSectionConfiguration : IEntityTypeConfiguration<QuestionnaireSection>
{
    public void Configure(EntityTypeBuilder<QuestionnaireSection> builder)
    {
        _ = builder.ToTable(nameof(IDatabaseService.QuestionnaireSections));
        builder.ConfigureModifiableProperties();
        _ = builder.Property(x => x.Code).HasMaxLength(50);
        _ = builder.Property(x => x.Title).HasMaxLength(300);
        _ = builder.Property(x => x.CompanyType).HasConversion<string>().HasMaxLength(40);
        _ = builder.HasIndex(x => new { x.QuestionnaireId, x.Code }).IsUnique();
        _ = builder.HasIndex(x => new { x.QuestionnaireId, x.Order });
        _ = builder.HasMany(x => x.Questions).WithOne(x => x.QuestionnaireSection).HasForeignKey(x => x.QuestionnaireSectionId).OnDelete(DeleteBehavior.Cascade);
    }
}
public sealed class QuestionnaireQuestionConfiguration : IEntityTypeConfiguration<QuestionnaireQuestion>
{
    public void Configure(EntityTypeBuilder<QuestionnaireQuestion> builder)
    {
        _ = builder.ToTable(nameof(IDatabaseService.QuestionnaireQuestions));
        builder.ConfigureModifiableProperties();
        _ = builder.Property(x => x.Code).HasMaxLength(50);
        _ = builder.Property(x => x.Label).HasMaxLength(1000);
        _ = builder.Property(x => x.Hint).HasMaxLength(1000);
        _ = builder.Property(x => x.Placeholder).HasMaxLength(1000);
        _ = builder.Property(x => x.Type).HasConversion<string>().HasMaxLength(30);
        _ = builder.Property(x => x.CompanyType).HasConversion<string>().HasMaxLength(40);
        _ = builder.Property(x => x.AnswerRule).HasConversion<string>().HasMaxLength(30);
        _ = builder.HasIndex(x => new { x.QuestionnaireSectionId, x.Code }).IsUnique();
        _ = builder.HasIndex(x => new { x.QuestionnaireSectionId, x.Order });
        _ = builder.HasMany(x => x.Options).WithOne(x => x.QuestionnaireQuestion).HasForeignKey(x => x.QuestionnaireQuestionId).OnDelete(DeleteBehavior.Cascade);
    }
}
public sealed class QuestionnaireOptionConfiguration : IEntityTypeConfiguration<QuestionnaireOption>
{
    public void Configure(EntityTypeBuilder<QuestionnaireOption> builder)
    {
        _ = builder.ToTable(nameof(IDatabaseService.QuestionnaireOptions));
        builder.ConfigureModifiableProperties();
        _ = builder.Property(x => x.Code).HasMaxLength(50);
        _ = builder.Property(x => x.Label).HasMaxLength(300);
        _ = builder.HasIndex(x => new { x.QuestionnaireQuestionId, x.Code }).IsUnique();
        _ = builder.HasIndex(x => new { x.QuestionnaireQuestionId, x.Order });
    }
}
public sealed class QuestionnaireRuleConfiguration : IEntityTypeConfiguration<QuestionnaireRule>
{
    public void Configure(EntityTypeBuilder<QuestionnaireRule> builder)
    {
        _ = builder.ToTable(nameof(IDatabaseService.QuestionnaireRules));
        builder.ConfigureModifiableProperties();
        _ = builder.Property(x => x.Operator).HasConversion<string>().HasMaxLength(30);
        _ = builder.Property(x => x.Action).HasConversion<string>().HasMaxLength(30);
        _ = builder.Property(x => x.Value).HasMaxLength(1000);
        _ = builder.HasOne(x => x.Questionnaire).WithMany(x => x.Rules).HasForeignKey(x => x.QuestionnaireId).OnDelete(DeleteBehavior.Cascade);
        _ = builder.HasIndex(x => new { x.QuestionnaireId, x.SourceQuestionId, x.TargetQuestionId });
    }
}
