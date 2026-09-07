namespace EBVL.BackEnd.Infrastructure.Database.Configurations;

public sealed class QuestionnaireConfiguration : IEntityTypeConfiguration<Questionnaire>
{
    public void Configure(EntityTypeBuilder<Questionnaire> builder)
    {
        _ = builder.ToTable(nameof(IDatabaseService.Questionnaires));
        builder.ConfigureModifiableProperties();
        _ = builder.Property(x => x.Code).HasColumnType(ColumnTypeFor.Nvarchar(50));
        _ = builder.Property(x => x.BusinessProcess).HasColumnType(ColumnTypeFor.Nvarchar(100));
        _ = builder.HasIndex(x => x.Code).IsUnique();
        _ = builder.HasMany(x => x.Versions).WithOne(x => x.Questionnaire).HasForeignKey(x => x.QuestionnaireId).OnDelete(DeleteBehavior.Cascade);
        _ = builder.HasOne(x => x.PublishedVersion).WithMany().HasForeignKey(x => x.PublishedVersionId).OnDelete(DeleteBehavior.Restrict);
    }
}
public sealed class QuestionnaireVersionConfiguration : IEntityTypeConfiguration<QuestionnaireVersion>
{
    public void Configure(EntityTypeBuilder<QuestionnaireVersion> builder)
    {
        _ = builder.ToTable(nameof(IDatabaseService.QuestionnaireVersions));
        builder.ConfigureModifiableProperties();
        _ = builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(30);
        _ = builder.Property(x => x.RowVersion).IsRowVersion();
        _ = builder.HasIndex(x => new { x.QuestionnaireId, x.Version }).IsUnique();
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
        _ = builder.HasIndex(x => new { x.QuestionnaireVersionId, x.Code }).IsUnique();
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
        _ = builder.HasOne(x => x.QuestionnaireVersion).WithMany(x => x.Rules).HasForeignKey(x => x.QuestionnaireVersionId).OnDelete(DeleteBehavior.Cascade);
        _ = builder.HasIndex(x => new { x.QuestionnaireVersionId, x.SourceQuestionId, x.TargetQuestionId });
    }
}
